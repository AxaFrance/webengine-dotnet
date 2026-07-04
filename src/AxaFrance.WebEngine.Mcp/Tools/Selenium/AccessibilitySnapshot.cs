using OpenQA.Selenium;

namespace AxaFrance.WebEngine.Mcp.Tools.Selenium;

internal static class AccessibilitySnapshot
{
    // Walks the live DOM and produces a compact indented accessibility tree,
    // similar to Playwright's browser_snapshot but running inside Selenium via JS.
    //
    // Output format (one element per line):
    //   [role] "accessible name" value="current value" [state] id="x" name="y" ref=N
    //
    // Each interactive element is tagged with data-mcp-ref=N on the live DOM.
    // Pass ref=N back via ElementDescriptor.Ref to locate elements with zero guessing.
    // Falls back gracefully on legacy apps with poor ARIA by still printing
    // structural nodes (table/row/cell, headings, form groups) so the LLM can
    // orient itself even without explicit ARIA attributes.
    internal const string Script = """
        return (function() {
            var lines = [];
            var refCounter = 0;

            // Clear stale refs from any previous snapshot
            document.querySelectorAll('[data-mcp-ref]').forEach(function(el) {
                el.removeAttribute('data-mcp-ref');
            });

            // --- stable locator hints (used with ElementDescriptor fields) --------
            var TEST_ATTRS = ['data-testid','data-test-id','data-test','data-cy','data-qa','data-id'];
            function getLocatorHint(el) {
                var parts = [];
                var id = el.getAttribute('id');
                if (id) parts.push('id="' + id + '"');
                var name = el.getAttribute('name');
                if (name) parts.push('name="' + name + '"');
                for (var i = 0; i < TEST_ATTRS.length; i++) {
                    var v = el.getAttribute(TEST_ATTRS[i]);
                    if (v) { parts.push(TEST_ATTRS[i] + '="' + v + '"'); break; }
                }
                return parts.length ? parts.join(' ') : null;
            }

            // --- role resolution -------------------------------------------------
            var TAG_ROLE = {
                a:        function(el) { return el.href ? 'link' : null; },
                button:   'button',
                input:    function(el) {
                              var t = (el.type || 'text').toLowerCase();
                              if (t === 'hidden')   return null;
                              if (t === 'checkbox') return 'checkbox';
                              if (t === 'radio')    return 'radio';
                              if (t === 'range')    return 'slider';
                              if (t === 'search')   return 'searchbox';
                              if (t === 'submit' || t === 'reset' || t === 'button') return 'button';
                              return 'textbox';
                          },
                select:   function(el) { return el.multiple ? 'listbox' : 'combobox'; },
                textarea: 'textbox',
                h1: 'heading', h2: 'heading', h3: 'heading',
                h4: 'heading', h5: 'heading', h6: 'heading',
                table: 'table', thead: 'rowgroup', tbody: 'rowgroup', tfoot: 'rowgroup',
                tr: 'row', th: 'columnheader', td: 'cell',
                ul: 'list', ol: 'list', li: 'listitem',
                nav: 'navigation', main: 'main', header: 'banner',
                footer: 'contentinfo', aside: 'complementary',
                section: 'region', article: 'article',
                form: 'form', fieldset: 'group', legend: 'none',
                dialog: 'dialog', img: 'img',
                summary: 'button', details: 'group'
            };

            function getRole(el) {
                var r = el.getAttribute('role');
                if (r && r !== 'presentation' && r !== 'none') return r;
                var tag = el.tagName.toLowerCase();
                var def = TAG_ROLE[tag];
                if (!def) return null;
                return typeof def === 'function' ? def(el) : def;
            }

            // --- accessible name resolution (ARIA spec order) --------------------
            function safeText(el) {
                try { return (el.innerText || el.textContent || '').trim(); } catch(e) { return ''; }
            }

            function getName(el) {
                var v;

                // 1. aria-label
                v = el.getAttribute('aria-label');
                if (v && v.trim()) return v.trim();

                // 2. aria-labelledby
                v = el.getAttribute('aria-labelledby');
                if (v) {
                    var parts = v.split(' ').map(function(id) {
                        var ref = document.getElementById(id);
                        return ref ? safeText(ref) : '';
                    }).filter(Boolean);
                    if (parts.length) return parts.join(' ');
                }

                // 3. <label for="id">
                if (el.id) {
                    try {
                        var lf = document.querySelector('label[for="' + el.id + '"]');
                        if (lf) { v = safeText(lf); if (v) return v; }
                    } catch(e) {}
                }

                // 4. Enclosing <label> � strip the control's own text
                var pl = el.closest ? el.closest('label') : null;
                if (pl) {
                    var clone = pl.cloneNode(true);
                    clone.querySelectorAll('input,select,textarea').forEach(function(i) {
                        i.parentNode && i.parentNode.removeChild(i);
                    });
                    v = safeText(clone); if (v) return v;
                }

                // 5. alt (images)
                v = el.getAttribute('alt');
                if (v !== null) return v.trim();

                // 6. placeholder
                v = el.getAttribute('placeholder');
                if (v && v.trim()) return v.trim();

                // 7. title
                v = el.getAttribute('title');
                if (v && v.trim()) return v.trim();

                // 8. inner text for naturally-labelled elements
                var tag = el.tagName.toLowerCase();
                if ('button,a,h1,h2,h3,h4,h5,h6,th,summary,legend'.indexOf(tag) !== -1) {
                    v = safeText(el); if (v && v.length <= 150) return v;
                }

                return null;
            }

            // --- state -----------------------------------------------------------
            function getState(el) {
                var p = [];
                if (el.disabled || el.getAttribute('aria-disabled') === 'true') p.push('disabled');
                var tag = el.tagName.toLowerCase();
                if (tag === 'input' && (el.type === 'checkbox' || el.type === 'radio'))
                    p.push(el.checked ? 'checked' : 'unchecked');
                var ac = el.getAttribute('aria-checked');
                if (ac !== null) p.push(ac === 'true' ? 'checked' : 'unchecked');
                var ex = el.getAttribute('aria-expanded');
                if (ex !== null) p.push(ex === 'true' ? 'expanded' : 'collapsed');
                if (el.getAttribute('aria-selected') === 'true') p.push('selected');
                if (el.required || el.getAttribute('aria-required') === 'true') p.push('required');
                if (el.getAttribute('aria-invalid') === 'true') p.push('invalid');
                return p.length ? p.join(', ') : null;
            }

            // --- current value ---------------------------------------------------
            function getValue(el) {
                var tag = el.tagName.toLowerCase();
                if (tag === 'select') {
                    var opt = el.options[el.selectedIndex];
                    return opt ? opt.text : null;
                }
                if ((tag === 'input' && el.type !== 'password') || tag === 'textarea')
                    return el.value || null;
                var vn = el.getAttribute('aria-valuenow');
                return vn !== null ? vn : null;
            }

            // --- roles we emit ---------------------------------------------------
            var PRINT = {
                button:1, link:1, textbox:1, checkbox:1, radio:1, combobox:1,
                listbox:1, slider:1, spinbutton:1, searchbox:1,
                menuitem:1, menuitemcheckbox:1, menuitemradio:1,
                tab:1, option:1, treeitem:1, switch:1,
                heading:1, img:1,
                table:1, rowgroup:1, row:1, columnheader:1, rowheader:1, cell:1, gridcell:1,
                list:1, listitem:1,
                navigation:1, main:1, banner:1, contentinfo:1,
                complementary:1, region:1, article:1,
                form:1, group:1,
                dialog:1, alertdialog:1, alert:1, status:1
            };

            // --- visibility check ------------------------------------------------
            function isHidden(el) {
                if (el.getAttribute('aria-hidden') === 'true') return true;
                var s = window.getComputedStyle(el);
                return s.display === 'none' || s.visibility === 'hidden';
            }

            // --- recursive walk --------------------------------------------------
            function walk(el, depth) {
                if (!el || el.nodeType !== 1 || isHidden(el)) return;

                var role  = getRole(el);
                var print = !!(role && PRINT[role]);

                if (print) {
                    var ref   = ++refCounter;
                    el.setAttribute('data-mcp-ref', String(ref));

                    var name  = getName(el);
                    var state = getState(el);
                    var val   = getValue(el);
                    var hint  = getLocatorHint(el);

                    var line = '  '.repeat(depth) + '[' + role + ']';
                    if (name)  line += ' "' + name + '"';
                    if (val)   line += ' value="' + val + '"';
                    if (state) line += ' [' + state + ']';
                    if (hint)  line += ' ' + hint;
                    line += ' ref=' + ref;
                    lines.push(line);
                }

                var nextDepth = depth + (print ? 1 : 0);
                Array.from(el.children).forEach(function(c) { walk(c, nextDepth); });
            }

            walk(document.body, 0);

            return lines.length > 0
                ? lines.join('\n')
                : '(no accessibility tree found � the page likely has poor ARIA markup; use GetPageHtml as fallback)';
        })();
        """;

    internal static string Capture(IWebDriver driver)
        => ((IJavaScriptExecutor)driver).ExecuteScript(Script)?.ToString() ?? string.Empty;
}
