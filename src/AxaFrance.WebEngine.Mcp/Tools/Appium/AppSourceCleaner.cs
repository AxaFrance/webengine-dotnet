using System.Text;
using System.Xml;

namespace AxaFrance.WebEngine.Mcp.Tools.Appium;

/// <summary>
/// Produces a compact, LLM-friendly snapshot of an Appium XML page source.
/// Only actionable or informative elements are included; container nodes are skipped.
/// Output format (one line per element):
///   [Role] accId="…" id="…" text="…" desc="…" {state}
/// </summary>
internal static class AppSourceCleaner
{
    // Element class-name suffixes that map to human-readable roles
    private static readonly Dictionary<string, string> RoleMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Button"]        = "Button",
        ["ImageButton"]   = "Button",
        ["TextView"]      = "Text",
        ["EditText"]      = "Input",
        ["CheckBox"]      = "Checkbox",
        ["RadioButton"]   = "Radio",
        ["Switch"]        = "Switch",
        ["ToggleButton"]  = "Toggle",
        ["Spinner"]       = "Dropdown",
        ["SeekBar"]       = "Slider",
        ["ProgressBar"]   = "Progress",
        ["ImageView"]     = "Image",
        ["WebView"]       = "WebView",
        ["RecyclerView"]  = "List",
        ["ListView"]      = "List",
        ["ScrollView"]    = "ScrollView",
        // iOS XCUIElement types
        ["XCUIElementTypeButton"]            = "Button",
        ["XCUIElementTypeTextField"]         = "Input",
        ["XCUIElementTypeSecureTextField"]   = "PasswordInput",
        ["XCUIElementTypeTextView"]          = "TextArea",
        ["XCUIElementTypeStaticText"]        = "Text",
        ["XCUIElementTypeCell"]              = "Cell",
        ["XCUIElementTypeSwitch"]            = "Switch",
        ["XCUIElementTypeSlider"]            = "Slider",
        ["XCUIElementTypeCheckBox"]          = "Checkbox",
        ["XCUIElementTypeLink"]              = "Link",
        ["XCUIElementTypeImage"]             = "Image",
        ["XCUIElementTypeSearchField"]       = "SearchInput",
        ["XCUIElementTypeTable"]             = "Table",
        ["XCUIElementTypeCollectionView"]    = "List",
        ["XCUIElementTypeWebView"]           = "WebView",
        ["XCUIElementTypeSegmentedControl"]  = "SegmentedControl",
        ["XCUIElementTypeNavigationBar"]     = "NavBar",
        ["XCUIElementTypeTabBar"]            = "TabBar",
        ["XCUIElementTypeToolbar"]           = "Toolbar",
        ["XCUIElementTypeAlert"]             = "Alert",
    };

    // Purely structural containers we never emit
    private static readonly HashSet<string> ContainerSuffixes = new(StringComparer.OrdinalIgnoreCase)
    {
        "FrameLayout", "LinearLayout", "RelativeLayout", "ConstraintLayout",
        "CoordinatorLayout", "DrawerLayout", "ViewGroup", "View",
        "XCUIElementTypeApplication", "XCUIElementTypeWindow", "XCUIElementTypeOther",
        "XCUIElementTypeScrollView", "XCUIElementTypeGroup",
    };

    /// <summary>
    /// Parses the raw Appium XML page source and returns a compact, one-line-per-element
    /// snapshot containing only interactive or informative nodes.
    /// </summary>
    public static string Compact(string xmlSource)
    {
        if (string.IsNullOrWhiteSpace(xmlSource))
            return "(empty page source)";

        var doc = new XmlDocument();
        try { doc.LoadXml(xmlSource); }
        catch { return "(unable to parse page source XML)"; }

        var sb = new StringBuilder();
        Walk(doc.DocumentElement, sb, 0);
        return sb.Length == 0 ? "(no actionable elements found)" : sb.ToString();
    }

    private static void Walk(XmlNode? node, StringBuilder sb, int depth)
    {
        if (node is null) return;

        if (node.NodeType == XmlNodeType.Element)
        {
            var className = Attr(node, "class") ?? node.LocalName ?? string.Empty;
            var shortClass = ShortClass(className);
            var role = ResolveRole(shortClass);

            if (role is not null)
            {
                var line = BuildLine(node, role);
                if (line is not null)
                    sb.AppendLine(line);
            }
            // Always recurse regardless of whether we emitted the current node
        }

        foreach (XmlNode child in node.ChildNodes)
            Walk(child, sb, depth + 1);
    }

    private static string? BuildLine(XmlNode node, string role)
    {
        var accId   = Attr(node, "content-desc") ?? Attr(node, "name");
        var resId   = Attr(node, "resource-id");
        var text    = Attr(node, "text") ?? Attr(node, "label") ?? Attr(node, "value");
        var hint    = Attr(node, "hint") ?? Attr(node, "placeholder");
        var enabled = Attr(node, "enabled");
        var checked_ = Attr(node, "checked");
        var selected = Attr(node, "selected");
        var clickable = Attr(node, "clickable");
        var bounds  = Attr(node, "bounds");

        // Skip invisible or non-informative pure containers with no identifiers
        if (role is "Image" or "Progress" && accId is null && text is null && resId is null)
            return null;

        var sb = new StringBuilder();
        sb.Append($"[{role}]");

        if (!string.IsNullOrEmpty(accId))   sb.Append($" accId=\"{accId}\"");
        if (!string.IsNullOrEmpty(resId))   sb.Append($" id=\"{StripPackage(resId)}\"");
        if (!string.IsNullOrEmpty(text))    sb.Append($" text=\"{Truncate(text, 60)}\"");
        if (!string.IsNullOrEmpty(hint))    sb.Append($" hint=\"{Truncate(hint, 40)}\"");

        // States
        var states = new List<string>();
        if (enabled is "false")              states.Add("disabled");
        if (checked_ is "true")             states.Add("checked");
        if (selected is "true")             states.Add("selected");
        if (clickable is "true" && role is "Text" or "Image")
                                             states.Add("clickable");
        if (states.Count > 0)
            sb.Append($" [{string.Join(",", states)}]");

        if (!string.IsNullOrEmpty(bounds))  sb.Append($" bounds={bounds}");

        return sb.ToString();
    }

    // Returns the short class suffix (e.g. "android.widget.Button" → "Button")
    private static string ShortClass(string className)
    {
        var dot = className.LastIndexOf('.');
        return dot >= 0 ? className[(dot + 1)..] : className;
    }

    private static string? ResolveRole(string shortClass)
    {
        if (RoleMap.TryGetValue(shortClass, out var role)) return role;
        if (ContainerSuffixes.Contains(shortClass)) return null;
        // Unknown element — only emit if it has explicit clickable=true attribute (handled by caller)
        return null;
    }

    private static string? Attr(XmlNode node, string name)
    {
        var val = node.Attributes?[name]?.Value;
        return string.IsNullOrEmpty(val) ? null : val;
    }

    // "com.example.app:id/login_button" → "login_button"
    private static string StripPackage(string resId)
    {
        var slash = resId.LastIndexOf('/');
        return slash >= 0 ? resId[(slash + 1)..] : resId;
    }

    private static string Truncate(string s, int max)
        => s.Length <= max ? s : s[..max] + "…";
}
