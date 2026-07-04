using OpenQA.Selenium;

namespace AxaFrance.WebEngine.Mcp.Tools.Selenium;

internal static class HtmlCleaner
{
    // Clones the full document, strips noise, and returns clean HTML.
    // Runs entirely in the browser — never touches the live DOM.
    //
    // Cleaning strategy:
    //   - Remove <head> entirely (meta, fonts, preload links — never useful for locators)
    //   - Remove script/style/noscript/template/iframe/link tags
    //   - Remove [hidden] elements
    //   - Collapse large inline SVGs
    //   - Replace base64 image sources
    //   - Strip ALL attributes except an explicit locator whitelist
    //     (id, name, type, value, aria-*, role, placeholder, for, href, checked, disabled,
    //      required, data-testid/cy/qa/id/mcp-ref, alt, title, etc.)
    //   - Remove HTML comments
    //
    // This typically reduces SPA pages from 200 KB to 15–30 KB by eliminating
    // class attributes, data-* noise, and event handlers.
    private const string PageScript = """
        return (function() {
            var root = document.documentElement.cloneNode(true);

            // Remove entire <head> — meta, fonts, preload, SEO tags are never needed
            var head = root.querySelector('head');
            if (head && head.parentNode) head.parentNode.removeChild(head);

            // Remove noise tags entirely
            root.querySelectorAll('script, style, link, noscript, template, iframe')
                .forEach(function(el) { el.parentNode && el.parentNode.removeChild(el); });

            // Remove explicitly hidden elements
            root.querySelectorAll('[hidden]')
                .forEach(function(el) { el.parentNode && el.parentNode.removeChild(el); });

            // Replace base64-encoded image sources with a placeholder
            root.querySelectorAll('img[src^="data:"]')
                .forEach(function(el) { el.setAttribute('src', '[base64]'); });

            // Collapse large inline SVGs to an empty element
            root.querySelectorAll('svg')
                .forEach(function(el) { if (el.innerHTML.length > 200) el.innerHTML = ''; });

            // Keep ONLY locator-relevant attributes; strip everything else
            // (removes class, data-* noise, style, event handlers, tabindex, autocomplete, …)
            var KEEP = {
                id:1, name:1, type:1, value:1, href:1, src:1,
                action:1, method:1, placeholder:1, for:1,
                checked:1, selected:1, disabled:1, required:1, multiple:1, readonly:1,
                alt:1, title:1, target:1,
                role:1,
                'aria-label':1, 'aria-labelledby':1, 'aria-describedby':1,
                'aria-expanded':1, 'aria-checked':1, 'aria-selected':1,
                'aria-disabled':1, 'aria-required':1, 'aria-invalid':1,
                'aria-controls':1, 'aria-haspopup':1, 'aria-hidden':1,
                'data-testid':1, 'data-test-id':1, 'data-test':1,
                'data-cy':1, 'data-qa':1, 'data-id':1, 'data-mcp-ref':1
            };
            root.querySelectorAll('*').forEach(function(el) {
                Array.from(el.attributes).forEach(function(a) {
                    if (!KEEP[a.name]) el.removeAttribute(a.name);
                });
            });

            // Remove HTML comments
            (function removeComments(node) {
                var i = node.childNodes.length;
                while (i--) {
                    var child = node.childNodes[i];
                    if (child.nodeType === 8) node.removeChild(child);
                    else if (child.childNodes.length) removeComments(child);
                }
            })(root);

            return root.outerHTML;
        })();
        """;

    // Same cleaning for a single element fragment.
    private const string ElementScript = """
        return (function(el) {
            var clone = el.cloneNode(true);

            clone.querySelectorAll('script, style, link, noscript, template')
                .forEach(function(n) { n.parentNode && n.parentNode.removeChild(n); });

            clone.querySelectorAll('img[src^="data:"]')
                .forEach(function(n) { n.setAttribute('src', '[base64]'); });

            clone.querySelectorAll('svg')
                .forEach(function(n) { if (n.innerHTML.length > 200) n.innerHTML = ''; });

            var KEEP = {
                id:1, name:1, type:1, value:1, href:1, src:1,
                action:1, method:1, placeholder:1, for:1,
                checked:1, selected:1, disabled:1, required:1, multiple:1, readonly:1,
                alt:1, title:1, target:1,
                role:1,
                'aria-label':1, 'aria-labelledby':1, 'aria-describedby':1,
                'aria-expanded':1, 'aria-checked':1, 'aria-selected':1,
                'aria-disabled':1, 'aria-required':1, 'aria-invalid':1,
                'aria-controls':1, 'aria-haspopup':1, 'aria-hidden':1,
                'data-testid':1, 'data-test-id':1, 'data-test':1,
                'data-cy':1, 'data-qa':1, 'data-id':1, 'data-mcp-ref':1
            };
            clone.querySelectorAll('*').forEach(function(n) {
                Array.from(n.attributes).forEach(function(a) {
                    if (!KEEP[a.name]) n.removeAttribute(a.name);
                });
            });
            // Also clean the root element itself
            Array.from(clone.attributes).forEach(function(a) {
                if (!KEEP[a.name]) clone.removeAttribute(a.name);
            });

            var wrap = document.createElement('div');
            wrap.appendChild(clone);
            return wrap.innerHTML;
        })(arguments[0]);
        """;

    public static string CleanPage(IWebDriver driver)
        => ((IJavaScriptExecutor)driver).ExecuteScript(PageScript)?.ToString() ?? string.Empty;

    public static string CleanElement(IWebDriver driver, IWebElement element)
        => ((IJavaScriptExecutor)driver).ExecuteScript(ElementScript, element)?.ToString() ?? string.Empty;

    // Returns a structural skeleton: only landmark/form/heading elements and their actionable
    // content survive.  Pure container tags (div, span, p, ul, li, …) are *unwrapped* — their
    // children are hoisted to the parent so no content is lost, but all the wrapper noise is gone.
    //
    // Typical reduction: 70–80 % smaller than CleanPage on SPA pages.
    //
    // Algorithm (runs inside the browser, never touches the live DOM):
    //   1. Apply the same cleaning as PageScript (head removal, attr whitelist, comments, …).
    //   2. Post-order traversal: process deepest nodes first so children are already pruned
    //      before we decide whether to keep or unwrap their parent.
    //   3. Any element NOT in the KEEP_TAGS set and without an explicit locator attribute is
    //      unwrapped: all its children (elements + text nodes) are inserted before it in the
    //      parent, then the empty shell is removed.
    private const string CompactPageScript = """
        return (function() {
            var root = document.documentElement.cloneNode(true);

            // ── Phase 1: same noise removal as PageScript ────────────────────────
            var head = root.querySelector('head');
            if (head && head.parentNode) head.parentNode.removeChild(head);

            root.querySelectorAll('script, style, link, noscript, template, iframe')
                .forEach(function(el) { el.parentNode && el.parentNode.removeChild(el); });

            root.querySelectorAll('[hidden]')
                .forEach(function(el) { el.parentNode && el.parentNode.removeChild(el); });

            root.querySelectorAll('img[src^="data:"]')
                .forEach(function(el) { el.setAttribute('src', '[base64]'); });

            root.querySelectorAll('svg')
                .forEach(function(el) { if (el.innerHTML.length > 200) el.innerHTML = ''; });

            var KEEP_ATTR = {
                id:1, name:1, type:1, value:1, href:1, src:1,
                action:1, method:1, placeholder:1, for:1,
                checked:1, selected:1, disabled:1, required:1, multiple:1, readonly:1,
                alt:1, title:1, target:1,
                role:1,
                'aria-label':1, 'aria-labelledby':1, 'aria-describedby':1,
                'aria-expanded':1, 'aria-checked':1, 'aria-selected':1,
                'aria-disabled':1, 'aria-required':1, 'aria-invalid':1,
                'aria-controls':1, 'aria-haspopup':1, 'aria-hidden':1,
                'data-testid':1, 'data-test-id':1, 'data-test':1,
                'data-cy':1, 'data-qa':1, 'data-id':1, 'data-mcp-ref':1
            };
            root.querySelectorAll('*').forEach(function(el) {
                Array.from(el.attributes).forEach(function(a) {
                    if (!KEEP_ATTR[a.name]) el.removeAttribute(a.name);
                });
            });

            (function removeComments(node) {
                var i = node.childNodes.length;
                while (i--) {
                    var child = node.childNodes[i];
                    if (child.nodeType === 8) node.removeChild(child);
                    else if (child.childNodes.length) removeComments(child);
                }
            })(root);

            // ── Phase 2: structural pruning ──────────────────────────────────────
            // Elements in this set are always kept; everything else is unwrapped.
            var KEEP_TAG = {
                // Structural roots (never removed)
                html:1, body:1,
                // Form structure and controls
                form:1, fieldset:1, legend:1,
                input:1, button:1, select:1, option:1, optgroup:1, textarea:1, label:1,
                // Links and images (common click targets)
                a:1, img:1,
                // Table structure (must keep to preserve row/cell relationships)
                table:1, thead:1, tbody:1, tfoot:1, tr:1, th:1, td:1,
                // Landmark / semantic sections
                nav:1, main:1, header:1, footer:1, aside:1, section:1, article:1, dialog:1,
                // Headings (spatial orientation)
                h1:1, h2:1, h3:1, h4:1, h5:1, h6:1,
                // Interactive disclosure
                summary:1, details:1
            };

            // An element with any locator attribute is always kept regardless of tag
            // (handles custom components: <my-btn id="x" role="button">…</my-btn>)
            var LOCATOR_ATTR = [
                'id','name','role','aria-label','data-testid','data-test-id',
                'data-test','data-cy','data-qa','data-id','data-mcp-ref'
            ];
            function hasLocator(el) {
                for (var i = 0; i < LOCATOR_ATTR.length; i++) {
                    if (el.getAttribute(LOCATOR_ATTR[i])) return true;
                }
                return false;
            }

            // Post-order: process deepest nodes first so hoisted children are already
            // pruned before we evaluate whether to unwrap their new parent.
            function prune(el) {
                // Snapshot children before recursing (tree will change during pruning)
                Array.from(el.children).forEach(prune);

                var tag = el.tagName.toLowerCase();
                if (KEEP_TAG[tag] || hasLocator(el)) return;

                // Unwrap: hoist all child nodes (elements + text) to parent, then remove shell
                var parent = el.parentNode;
                if (!parent) return;
                while (el.firstChild) parent.insertBefore(el.firstChild, el);
                parent.removeChild(el);
            }

            var body = root.querySelector('body');
            if (body) Array.from(body.children).forEach(prune);

            return root.outerHTML;
        })();
        """;

    public static string CleanPageCompact(IWebDriver driver)
        => ((IJavaScriptExecutor)driver).ExecuteScript(CompactPageScript)?.ToString() ?? string.Empty;
}
