﻿using Deque.AxeCore.Commons;
using Deque.AxeCore.Selenium;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AxaFrance.AxeExtended.Selenium
{
    /// <summary>
    /// ExtendedBuilder for Axe Core. This class is used to configure and run axe against a web page.
    /// This class is a fork of original AxeBuilder class from Deque AxeCore per https://github.com/dequelabs/axe-core-nuget/issues/146
    /// There is no possibility to use a custom configuration file with the original AxeBuilder class, so we need to create this class and expose the WithAxeConfig method.
    /// With this method, we can pass a custom configuration file to the axe engine and run custom rules in addition to default axe rules.
    /// </summary>
    public class AxeExtendedBuilder
    {
        private readonly WebDriver _webDriver;
        private readonly AxeBuilderOptions _AxeBuilderOptions;
        private readonly AxeRunContext runContext = new AxeRunContext();
        private AxeRunOptions runOptions = new AxeRunOptions();
        private string outputFilePath = null;
        private bool useLegacyMode = false;
        private JObject axeConfig;

        /// <summary>
        /// Initialize an instance of <see cref="AxeBuilder"/>
        /// </summary>
        /// <param name="webDriver">Selenium driver to use</param>
        public AxeExtendedBuilder(WebDriver webDriver) : this(webDriver, new AxeBuilderOptions { ScriptProvider = new BundledAxeScriptProvider() })
        {
        }

        /// <summary>
        /// Initialize an instance of <see cref="AxeBuilder"/>
        /// </summary>
        /// <param name="webDriver"></param>
        /// <param name="config"></param>
        public AxeExtendedBuilder(WebDriver webDriver, JObject config) : this(webDriver, new AxeBuilderOptions { ScriptProvider = new BundledAxeScriptProvider() })
        {
            axeConfig = config;
        }

        /// <summary>
        /// Initialize an instance of <see cref="AxeBuilder"/>
        /// </summary>
        /// <param name="webDriver"></param>
        /// <param name="config"></param>
        /// <param name="options"></param>
        public AxeExtendedBuilder(WebDriver webDriver, JObject config, AxeBuilderOptions options) : this(webDriver, options)
        {
            axeConfig = config;
        }

        /// <summary>
        /// Initialize an instance of <see cref="AxeBuilder"/>
        /// </summary>
        /// <param name="webDriver">Selenium driver to use</param>
        /// <param name="options">Builder options</param>
        public AxeExtendedBuilder(WebDriver webDriver, AxeBuilderOptions options)
        {
            ValidateNotNullParameter(webDriver, nameof(webDriver));
            ValidateNotNullParameter(options, nameof(options));

            _webDriver = webDriver;
            _AxeBuilderOptions = options;
        }

        /// <summary>
        /// Use frameMessenger with &lt;same_origin_only&gt;.
        /// This disables use of axe.runPartial() which is called in each frame, and
        /// axe.finishRun() which is called in a blank page. This uses axe.run() instead,
        /// but with the restriction that cross-origin frames will not be tested.
        /// </summary>
        /// <param name="legacyMode">Whether to enable or disable Legacy Mode</param>
        [Obsolete("Legacy Mode is being removed in the future. Use with caution!")]
        public AxeExtendedBuilder UseLegacyMode(bool legacyMode = true)
        {
            this.useLegacyMode = legacyMode;
            return this;
        }

        /// <summary>
        ///  Run configuration data that is passed to axe for scanning the web page.
        ///  This will override the value set by <see cref="WithRules(string[])"/>, <see cref="WithTags(string[])"/> &amp; <see cref="DisableRules(string[])"/>
        /// </summary>
        /// <param name="runOptions">run options to be used for scanning</param>
        public AxeExtendedBuilder WithOptions(AxeRunOptions runOptions)
        {
            ValidateNotNullParameter(runOptions, nameof(runOptions));

            this.runOptions = runOptions;

            return this;
        }

        /// <summary>
        /// Limit analysis to only the specified tags.
        /// Refer https://www.deque.com/axe/axe-for-web/documentation/api-documentation/#api-name-axegetrules to get the list of supported tag names
        /// Cannot be used with <see cref="WithRules(string[])"/> & <see cref="Options"/>
        /// </summary>
        /// <param name="tags">tags to be used for scanning</param>
        public AxeExtendedBuilder WithTags(params string[] tags)
        {
            ValidateParameters(tags, nameof(tags));

            runOptions.RunOnly = new RunOnlyOptions
            {
                Type = "tag",
                Values = tags.ToList()
            };
            return this;
        }

        /// <summary>
        /// Limit analysis to only the specified rules.
        /// Refer https://dequeuniversity.com/rules/axe/ to get the complete listing of available rule IDs.
        /// Cannot be used with <see cref="WithTags(string[])"/> &amp; <see cref="WithOptions(AxeRunOptions)"/>
        /// </summary>
        /// <param name="rules">rule IDs to be used for scanning</param>
        public AxeExtendedBuilder WithRules(params string[] rules)
        {
            ValidateParameters(rules, nameof(rules));

            runOptions.RunOnly = new RunOnlyOptions
            {
                Type = "rule",
                Values = rules.ToList()
            };

            return this;
        }

        /// <summary>
        ///  Set the list of rules to skip when running an analysis
        ///  Refer https://dequeuniversity.com/rules/axe/ to get the complete listing of available rule IDs.
        ///  Cannot be used with <see cref="Options"/>
        /// </summary>
        /// <param name="rules">rule IDs to be skipped from analysis</param>
        public AxeExtendedBuilder DisableRules(params string[] rules)
        {
            ValidateParameters(rules, nameof(rules));

            var rulesMap = new Dictionary<string, RuleOptions>();
            foreach (var rule in rules)
            {
                rulesMap[rule] = new RuleOptions
                {
                    Enabled = false
                };
            }
            runOptions.Rules = rulesMap;
            return this;
        }

        /// <summary>
        /// Restricts the analysis to include only the given CSS selector and its descendants, instead of the whole page.
        /// To include multiple selectors, call Include multiple times.
        /// To include a selector inside an iframe, see <see cref="Include(AxeSelector)"/> and <see cref="AxeSelector.AxeSelector(string, List{string})"/>.
        /// To include a selector inside a shadow DOM, see <see cref="Include(AxeSelector)"/> and <see cref="AxeSelector.FromFrameShadowSelectors(List{List{string}})"/>.
        /// </summary>
        /// <param name="selector">A CSS selector in the topmost frame of the page</param>
        public AxeExtendedBuilder Include(string selector) => this.Include(new AxeSelector(selector));

        /// <summary>
        /// Restricts the analysis to include only the given <see cref="AxeSelector"/> and its descendants, instead of the whole page.
        /// This overload can include selectors inside iframes (with <see cref="AxeSelector.AxeSelector(string, List{string})"/>) or shadow DOMs
        /// (<see cref="AxeSelector.FromFrameShadowSelectors(List{List{string}})"/>).
        /// To include multiple selectors, call Include multiple times.
        /// </summary>
        /// <param name="selector">An <see cref="AxeSelector"/> representing an element anywhere in the page, including nested in an iframe or shadow DOM</param>
        public AxeExtendedBuilder Include(AxeSelector selector)
        {
            ValidateNotNullParameter(selector, nameof(selector));

            runContext.Include = runContext.Include ?? new List<AxeSelector>();
            runContext.Include.Add(selector);
            return this;
        }

        /// <summary>
        /// Excludes the given CSS selector and its descendants from analysis.
        /// To exclude multiple selectors, call Exclude multiple times.
        /// To exclude a selector inside an iframe, see <see cref="Exclude(AxeSelector)"/> and <see cref="AxeSelector.AxeSelector(string, List{string})"/>.
        /// To exclude a selector inside a shadow DOM, see <see cref="Exclude(AxeSelector)"/> and <see cref="AxeSelector.FromFrameShadowSelectors(List{List{string}})"/>.
        /// </summary>
        /// <param name="selector">A CSS selector in the topmost frame of the page</param>
        public AxeExtendedBuilder Exclude(string selector) => this.Exclude(new AxeSelector(selector));

        /// <summary>
        /// Excludes the given <see cref="AxeSelector"/> and its descendants from analysis. This overload can exclude selectors inside iframes
        /// (with <see cref="AxeSelector.AxeSelector(string, List{string})"/>) or shadow DOMs (<see cref="AxeSelector.FromFrameShadowSelectors(List{List{string}})"/>).
        /// To exclude multiple selectors, call Exclude multiple times.
        /// </summary>
        /// <param name="selector">An AxeSelector representing an element anywhere in the page, including nested in an iframe or shadow DOM</param>
        public AxeExtendedBuilder Exclude(AxeSelector selector)
        {
            ValidateNotNullParameter(selector, nameof(selector));

            runContext.Exclude = runContext.Exclude ?? new List<AxeSelector>();
            runContext.Exclude.Add(selector);
            return this;
        }

        /// <summary>
        /// Causes <see cref="Analyze()"/> to write the axe results as a JSON file, in addition to returning it in object format as usual.
        /// File will be overwritten if already exists.
        /// </summary>
        /// <param name="path">Path to the output file. Will be passed as-is to the System.IO APIs.</param>
        public AxeExtendedBuilder WithOutputFile(string path)
        {
            ValidateNotNullParameter(path, nameof(path));

            outputFilePath = path;
            return this;
        }

        /// <summary>
        /// Run axe against a specific WebElement (including its descendants).
        /// </summary>
        /// <param name="context">A WebElement to test</param>
        /// <returns>An axe results document</returns>
        public AxeResult Analyze(IWebElement context)
        {
            return AnalyzeRawContext(context);
        }

        /// <summary>
        /// Run axe against the entire page.
        /// </summary>
        /// <returns>An axe results document</returns>
        public AxeResult Analyze()
        {
            bool runContextHasData = runContext.Include?.Any() == true || runContext.Exclude?.Any() == true;

            string rawContext = runContextHasData ? JsonConvert.SerializeObject(runContext, AxeJsonSerializerSettings.Default) : null;

            return AnalyzeRawContext(rawContext);
        }

        /// <summary>
        /// Runs axe via legacyScan.js at a specific context, which will be passed as-is to Selenium for legacyScan.js to interpret, and
        /// parses/handles the legacyScan.js output per the current builder options.
        /// </summary>
        /// <param name="rawContextArg">The value to pass as-is to legacyScan.js to use as the axe.run "context" argument</param>
        private AxeResult AnalyzeRawContext(object rawContextArg)
        {
            ConfigureAxe();

            var runPartialExists = (bool)_webDriver.ExecuteScript(EmbeddedResourceProvider.ReadEmbeddedFile("runPartialExists.js"));

            JObject resultObject;
            if (!runPartialExists || useLegacyMode)
            {
                resultObject = AnalyzeAxeLegacy(rawContextArg);
            }
            else
            {
                resultObject = AnalyzeAxeRunPartial(rawContextArg);
            }

            if (outputFilePath != null && resultObject.Type == JTokenType.Object)
            {
                Encoding utf8NoBOM = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
                using (var outputFileWriter = new StreamWriter(outputFilePath, append: false, encoding: utf8NoBOM))
                {
                    resultObject.WriteTo(new JsonTextWriter(outputFileWriter));
                }
            }

            return new AxeResult(resultObject);

        }

        private JObject AnalyzeAxeRunPartial(object rawContextArg)
        {
            string rawOptionsArg = SerializedRunOptions();
            var partialResults = RunPartialRecursive(rawOptionsArg, rawContextArg, true);
            return IsolatedFinishRun(partialResults.ToArray(), rawOptionsArg);
        }

        private List<object> RunPartialRecursive(
                                                        object options,
                                                        object context,
                                                        bool isTopLevel
                                                        )
        {
            if (!isTopLevel)
            {
                ConfigureAxe();
            }

            var partialResults = new List<object>();

            try
            {
                string partialRes = (string)_webDriver.ExecuteAsyncScript(EmbeddedResourceProvider.ReadEmbeddedFile("runPartial.js"), context, options);
                // Important to deserialize because we want to reserialize as an
                // array of object, not an array of strings.
                partialResults.Add(JsonConvert.DeserializeObject<object>(partialRes));
            }
            catch
            {
                if (isTopLevel)
                {
                    throw;
                }
                else
                {
                    partialResults.Add(null);
                    return partialResults;
                }
            }

            // Don't go any deeper if we are just doing top-level iframe
            if (runOptions.Iframes == false)
            {
                return partialResults;
            }

            var frameContexts = GetFrameContexts(context);
            foreach (var fContext in frameContexts)
            {
                try
                {
                    object frameContext = JsonConvert.SerializeObject(fContext.Context, AxeJsonSerializerSettings.Default);
                    object frameSelector = JsonConvert.SerializeObject(fContext.Selector, AxeJsonSerializerSettings.Default);
                    var frame = _webDriver.ExecuteScript(EmbeddedResourceProvider.ReadEmbeddedFile("shadowSelect.js"), frameSelector);
                    _webDriver.SwitchTo().Frame(frame as IWebElement);

                    partialResults.AddRange(RunPartialRecursive(options, frameContext, false));
                }
                catch (Exception)
                {
                    partialResults.Add(null);
                }
                finally
                {
                    _webDriver.SwitchTo().ParentFrame();
                }
            }



            return partialResults;
        }

        private JObject IsolatedFinishRun(object[] partialResults, object options)
        {
            // grab reference to current window
            var originalWindowHandle = _webDriver.CurrentWindowHandle;

            try
            {
                // create an isolated page
                _webDriver.ExecuteScript("window.open('about:blank', '_blank')");
                _webDriver.SwitchTo().Window(_webDriver.WindowHandles.Last());
                _webDriver.Navigate().GoToUrl("about:blank");
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"Failed to switch windows. Please make sure you have popup blockers disabled and you are using the correct browser drivers.{Environment.NewLine}Please check out https://axe-devtools-html-docs.deque.com/reference/csharp/error-handling.html",
                    e
                );
            }

            ConfigureAxe();

            var serializedPartials = JsonConvert.SerializeObject(partialResults, AxeJsonSerializerSettings.Default);
            int sizeLimit = 10_000_000;

            while (!string.IsNullOrEmpty(serializedPartials))
            {
                int chunkSize = sizeLimit;
                if (chunkSize > serializedPartials.Length)
                {
                    chunkSize = serializedPartials.Length;
                }
                String chunk = serializedPartials.Substring(0, chunkSize);
                serializedPartials = serializedPartials.Substring(chunkSize);
                _webDriver.ExecuteScript(EmbeddedResourceProvider.ReadEmbeddedFile("storeChunk.js"), chunk);
            }
            // grab result ...
            var result = _webDriver.ExecuteAsyncScript(EmbeddedResourceProvider.ReadEmbeddedFile("finishRun.js"), options);

            // ... close the new window and go back
            _webDriver.Close();
            _webDriver.SwitchTo().Window(originalWindowHandle);

            return JObject.FromObject(result);

        }

        private JObject AnalyzeAxeLegacy(object rawContextArg)
        {
            // Skip if value is set to false
            if (runOptions.Iframes != false)
            {
                _webDriver.ForEachFrameContext(() => ConfigureAxe());
            }

            string rawOptionsArg = SerializedRunOptions();
            string scanJsContent = EmbeddedResourceProvider.ReadEmbeddedFile("legacyScan.js");
            object[] rawArgs = new[] { rawContextArg, rawOptionsArg };
            var result = ((IJavaScriptExecutor)_webDriver).ExecuteAsyncScript(scanJsContent, rawArgs);

            return JObject.FromObject(result);
        }

        /// <summary>
        /// Execute `axe.utils.getFrameContexts(context)`
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private List<AxeFrameContext> GetFrameContexts(object context)
        {
            var frameContextsString = (string)_webDriver.ExecuteScript(EmbeddedResourceProvider.ReadEmbeddedFile("getFrameContexts.js"), context);
            var frameContexts = JsonConvert.DeserializeObject<List<AxeFrameContext>>(frameContextsString);
            return frameContexts;
        }

        /// <summary>
        /// Injects scripts into the current frame and configures axe
        /// </summary>
        private void ConfigureAxe()
        {
            _webDriver.ExecuteScript(_AxeBuilderOptions.ScriptProvider.GetScript());
            var runPartialExists = (bool)_webDriver.ExecuteScript(EmbeddedResourceProvider.ReadEmbeddedFile("runPartialExists.js"));

            if (!runPartialExists && !useLegacyMode)
            {
                _webDriver.ExecuteScript(
                    EmbeddedResourceProvider.ReadEmbeddedFile("allowIframeUnsafe.js")
                );
            }

            if (axeConfig == null)
            {
                _webDriver.ExecuteScript(EmbeddedResourceProvider.ReadEmbeddedFile("branding.js"));
            }
            else
            {
                _webDriver.ExecuteScript(GetConfigJS(axeConfig));
            }
        }

        private string GetConfigJS(JObject axeConfig)
        {
            string script = "const customConfig = $configJson; axe.configure(customConfig);"
                .Replace("$configJson", axeConfig.ToString());
            return script;
        }

        private string SerializedRunOptions()
        {
            return JsonConvert.SerializeObject(runOptions, AxeJsonSerializerSettings.Default);
        }

        private static void ValidateParameters(string[] parameterValue, string parameterName)
        {
            ValidateNotNullParameter(parameterValue, parameterName);

            if (parameterValue.Any(string.IsNullOrEmpty))
            {
                throw new ArgumentException("There is some items null or empty", parameterName);
            }
        }

        private static void ValidateNotNullParameter<T>(T parameterValue, string parameterName)
        {
            if (parameterValue == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        internal void WithAxeConfig(JObject config)
        {
            axeConfig = config;
        }
    }
}
