// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using AxaFrance.WebEngine.Report;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace AxaFrance.WebEngine
{
    /// <summary>
    /// Base class of the shared actions. All shared actions must be derived from this class and implement DoAction, DoCheckpoint method.
    /// In order to verify if all parameters are given when calling DoAction() and DoCheckpoint() methods, please specify RequiredParameters Property
    /// </summary>
    public abstract class SharedActionBase
    {

        /// <summary>
        /// The DateTime where the last shared action has been executed. This DateTime is considered the time of last operation of test framework.
        /// if an test case blocks somewhere and last for more than x minutes, the test framework will terminates a test case.
        /// </summary>
        public static DateTime UpdateTime { get; set; }
        private static Stack<int> ThreadStack = new Stack<int>();

        /// <summary>
        /// Implement DoAction() method to interact with your application (Click the links, Fill forms, Click buttons, ...)
        /// Of course, you can do any actions needed.
        /// </summary>
        /// <param name="Context">Application Context used for the action, for LeanFT actions, it will be an object of type AppModelBase</param>
        public abstract void DoAction(object Context);

        /// <summary>
        /// This method should be called after calling DoAction() Method in order to check whether the action is correctly running.
        /// </summary>
        /// <param name="Context">Application Context used for the checkpoint, for LeanFT actions, it will be an object of type AppModelBase</param>
        /// <returns>True if the checkpoint is passed, or False if the checkpoint fails. if the return value is false, the test will be marked as failed. </returns>
        /// <remarks>
        /// <para>If the return value is True and the ActionResult = Failed or Warning, Test will continue and the Test case will be marked as Failed.</para>
        /// <para>If the checkpoint returns False, test will be stop and will be considered as Failed.</para>
        /// <para>When calling another action from an action, please considering promote errors and result to above level.</para>
        /// </remarks>
        public abstract bool DoCheckpoint(object Context);

        /// <summary>
        /// Provide screen-shot in the report
        /// </summary>
        protected abstract void Screenshot(string name);


        /// <summary>
        /// Provide required parameters for this Action.
        /// when provide a Value of a Parameter, it will be considered as its default value. Missing such parameter will lead the usage of default value for the test.
        /// when provide a null Value of a Parameter, Missing such parameter will lead NullArgumentException
        /// </summary>
        public abstract Variable[] RequiredParameters { get; }


        internal static int Indent { get; set; } = 2;

        /// <summary>
        /// Stores the test result for the current action. the information of execution will be updated.
        /// </summary>
        protected ActionReport ActionReport { get; set; }

        protected TestCase testCase;

        public static SharedActionBase DoAction(Type sharedActionType, TestCase relatedTestCase, object Context, List<Variable> ContextValues, ActionReport actionReport, params Variable[] parameters)
        {
            Indent += 2;
            DebugLogger.WriteLine(new string(' ', Indent) + "Action Start:" + sharedActionType.Name);
            actionReport.Name = Utilities.GetDescriptionByType(sharedActionType);

            DateTime StartTime = DateTime.Now;
            actionReport.StartTime = StartTime;
            UpdateTime = DateTime.Now;

            if (TestCase.IgnoreAllSteps)
            {
                DebugLogger.WriteWarning("[DEBUG] Abort Requested, Ignoring current action:  " + sharedActionType.Name);
                return null;
            }
            List<Variable> DefaultValues = new List<Variable>();
            SharedActionBase obj = (SharedActionBase)Activator.CreateInstance(sharedActionType);
            obj.ActionReport = actionReport;
            obj.ContextValues = ContextValues;
            obj.testCase = relatedTestCase;
            List<string> missingParameters = new List<string>();
            if (obj.RequiredParameters != null)
            {
                foreach (var p in obj.RequiredParameters)
                {
                    //Test if the value is given
                    var param = Array.Find(parameters, x => x.Name == p.Name);
                    if (param == null)
                    {
                        if (p.Value != null)
                        {
                            //if not given, try to use the default value.
                            DefaultValues.Add(p);
                        }
                        else
                        {
                            //if no default value, show error message.
                            missingParameters.Add(p.Name);
                        }
                    }
                }
                if (missingParameters.Count > 0)
                {
                    string information = "Following required parameters are missing: " + string.Join(", ", missingParameters.ToArray());
                    DebugLogger.WriteError(information);
                    obj.ActionReport.Result = Result.Failed;
                    obj.ActionResult = Result.Failed;
                    obj.Information.AppendLine(information);
                    throw new ArgumentNullException(information);
                }
            }
            List<Variable> parameterList = new List<Variable>(parameters);
            parameterList.AddRange(DefaultValues);
            obj.Parameters = parameterList.ToArray();



            Exception ThreadException = null;
            Thread thread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    obj.DoAction(Context);
                }
                catch (Exception ex)
                {
                    obj.Screenshot("Exception");
                    DebugLogger.WriteError("[Exception] + " + ex.ToString());
                    ThreadException = ex;
                }
            }));
            thread.Name = "_action_" + sharedActionType.Name;
            thread.Start();
            thread.Join();
            if (ThreadException != null)
            {
                throw new WebEngineGeneralException("Exception occurs when running the Action. ", ThreadException);
            }
            DebugLogger.WriteLine(new string(' ', Indent) + "Action End  :" + sharedActionType.Name);


            if (obj.ContextValues != null && obj.ContextValues.Count > 0)
            {
                DebugLogger.WriteLine(new string(' ', Indent) + "- Context Parameters:");
                foreach (var v in obj.ContextValues)
                {
                    DebugLogger.WriteLine(new string(' ', Indent) + "- " + v.ToString());
                }
            }


            actionReport.ContextValues = obj.ContextValues.ToList();
            actionReport.EndTime = DateTime.Now;
            actionReport.Result = obj.ActionResult;
            actionReport.Screenshots = obj.Screenshots;
            return obj;
        }

        /// <summary>
        /// Static function to run this shared action. the action:
        /// 1. Check if all the required parameters are given, or by default using it's default value.
        /// 2. for all those who the parameter has no default value (Parameter.Value == null), throw exception
        /// 3. Create an instance of the action and run it.
        /// </summary>
        /// <param name="sharedActionType">the object type of sharedAction</param>
        /// <param name="Context">The browser object</param>
        /// <param name="ContextValues">The stored context content to be shared and be used with other actions.</param>
        /// <param name="actionReport">The testaction report to use for generating </param>
        /// <param name="parameters">Test parameters</param>
        /// <returns>A SharedActionBase object which is created during the execution. this object can be used later to call DoCheckpoint()</returns>
        public static SharedActionBase DoAction(Type sharedActionType, object Context, List<Variable> ContextValues, ActionReport actionReport, params Variable[] parameters)
        {
            return DoAction(sharedActionType, null, Context, ContextValues, actionReport, parameters);
        }

        /// <summary>
        /// Funaction to run an sub action from the current one. 
        /// This action reuses the <see cref="DoAction(Type, object, List{Variable}, ActionReport, Variable[])"/> and generates 
        /// a <see cref="ActionReport"/> attached to <see cref="ActionReport.SubActionReports"/>
        /// </summary>
        /// <param name="sharedActionType">the object type of sharedAction</param>
        /// <param name="Context">The browser object</param>
        /// <param name="ContextValues">The stored context content to be shared and be used with other actions.</param>
        /// <param name="parameters">Test parameters</param>
        /// <returns>A SharedActionBase object which is created during the execution. this object can be used later to call DoCheckpoint()</returns>
        protected SharedActionBase DoAction(Type sharedActionType, object Context, List<Variable> ContextValues, params Variable[] parameters)
        {
            ActionReport ar = new ActionReport();
            ActionReport.SubActionReports.Add(ar);
            var action = DoAction(sharedActionType, Context, ContextValues, ar, parameters);
            return action;
        }

        /// <summary>
        /// Static function to execute and check the Shared Action
        /// <para>This action calls internally DoAction() and DoCheckpoint() together. it is recommended to call this function within an action.</para>
        /// </summary>
        /// <param name="sharedActionType">the Type of the SharedAction to run</param>
        /// <param name="Context">The Browser object</param>
        /// <param name="ContextValues">The stored context content to be shared and be used with other actions</param>
        /// <param name="actionReport">The report object of the target action to be used.</param>
        /// <param name="parameters">Given parameters (test data dedicated to the current test case)</param>
        /// <returns>Boolean value indicates if the Action runs well and the checkpoint passes.</returns>
        /// <remarks>
        /// When calling DoAction, DoCheckpoint or DoActionWithCheckpoint in another action, please consider promote the result to above level.
        /// </remarks>
        public static bool DoActionWithCheckpoint(Type sharedActionType, object Context, List<Variable> ContextValues, ActionReport actionReport, params Variable[] parameters)
        {
            bool returnvalue = true;
            SharedActionBase action = DoAction(sharedActionType, Context, ContextValues, actionReport, parameters);
            if (action.ActionResult == Result.CriticalError) returnvalue = false;
            else returnvalue = action.DoCheckpoint(Context);
            if (!returnvalue)
            {
                action.Screenshot("Auto screenshot for failure");
            }
            actionReport.ContextValues = action.ContextValues.ToList();
            actionReport.EndTime = DateTime.Now;
            actionReport.Result = action.ActionResult == Result.None? Result.Passed : Result.Failed;
            actionReport.Screenshots = action.Screenshots;
            return returnvalue;
        }

        /// <summary>
        /// Static function to execute and check the Shared Action
        /// <para>This action calls internally DoAction() and DoCheckpoint() together. it is recommended to call this function within an action.</para>
        /// </summary>
        /// <param name="sharedActionType">the Type of the SharedAction to run</param>
        /// <param name="Context">The Browser object</param>
        /// <param name="ContextValues">The stored context content to be shared and be used with other actions</param>
        /// <param name="CallingAction">The SharedAction object which calls the sub-action to be executed.</param>
        /// <param name="parameters">Given parameters (test data dedicated to the current test case)</param>
        /// <returns>Boolean value indicates if the Action runs well and the checkpoint passes.</returns>
        /// <remarks>
        /// When calling DoAction, DoCheckpoint or DoActionWithCheckpoint in another action, please consider promote the result to above level.
        /// </remarks>
        public static bool DoActionWithCheckpoint(Type sharedActionType, object Context, List<Variable> ContextValues, SharedActionBase CallingAction, params Variable[] parameters)
        {
            ActionReport action = new ActionReport();
            CallingAction.ActionReport.SubActionReports.Add(action);
            return DoActionWithCheckpoint(sharedActionType, Context, ContextValues, action, parameters);  
        }

        /// <summary>
        /// Get the value of the parameter from giving list. The value can be empty, but will never be null (unless it's value is intendedly changed by yourself)
        /// If the parameter does not exists, an exception will be thrown, to avoid this make sure the parameter is listed in RequiredParameters.
        /// </summary>
        /// <param name="source">the list of parameter where the parameter is in.</param>
        /// <param name="name">the name of the parameter to find</param>
        /// <returns>the value of the giving parameter</returns>
        public static string GetParameter(IEnumerable<Variable> source, string name)
        {
            try
            {
                return source.First(x => x.Name == name).Value;
            }
            catch (Exception ex)
            {
                throw new WebEngineGeneralException(name + " is not found in the parameters list", ex);
            }
        }

        /// <summary>
        /// Get the value of the parameter from giving list. The value can be empty, but will never be null (unless it's value is intendedly changed by yourself)
        /// If the parameter does not exists, an exception will be thrown, to avoid this make sure the parameter is listed in RequiredParameters.
        /// </summary>
        /// <param name="name">the name of the parameter to find</param>
        /// <returns>the value of the giving parameter</returns>
        public string GetParameter(string name)
        {
            try
            {
                return Parameters.First(x => x.Name == name).Value;
            }
            catch (Exception ex)
            {
                throw new WebEngineGeneralException(name + " is not found in the parameters list", ex);
            }
        }

        /// <summary>
        /// Given parameters (Test Data) used to run the current test action
        /// </summary>
        public Variable[] Parameters { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="_parameters">the parameters to use during the test</param>

        protected SharedActionBase(Variable[] _parameters)
        {
            Parameters = _parameters;
            ContextValues = new List<Variable>();
        }

        private List<Variable> contextValues;

        /// <summary>
        /// Values can be stored and shared with other actions within the same test case.
        /// </summary>
        public List<Variable> ContextValues
        {
            get
            {
                return contextValues;
            }
            internal set
            {
                if (value != null)
                {
                    contextValues = value;
                }
                else
                {
                    contextValues = new List<Variable>();
                }
            }
        }


        /// <summary>
        /// All the information you want to pass to upper level.
        /// the information will be reported in TestStep object and be written in the XML report.
        /// Use AppendLine() operator to append information text if necessary.
        /// </summary>
        public StringBuilder Information { get; internal set; } = new StringBuilder();


        /// <summary>
        /// Get the Type of an SharedActionBase by its name.
        /// </summary>
        /// <param name="action">the name of the shared action</param>
        /// <returns>Type object of the action.</returns>
        public Type GetType(string action)
        {
            foreach (Assembly ass in GlobalConstants.LoadedAssemblies)
            {
                Type[] types = ass.GetTypes();
                Type t;

                t = Array.Find(types, x => x.Name == action));
                if (t != null)
                {
                    return t;
                }
            }
            throw new WebEngineGeneralException(string.Format("The Type {0} can not be found.", action));
        }

        /// <summary>
        /// The Global level context test values.
        /// <para>Variables in this list will be conserved between different test cases. if necessary you need to clear the list at the beginning of the test </para>
        /// </summary>
        public static List<Variable> Global { get; set; } = new List<Variable>();

        /// <summary>
        /// The indicator if the current action is correctly executed. it's default value is Passed
        /// </summary>
        /// <remarks>
        /// If an actions is Failed the test case runner will continue run following test steps and actions.
        /// If critical error have happend and the test case must stop, please use the result <see cref="Result.CriticalError"/>
        /// You can provide more detailed information to Information property.
        /// </remarks>
        public Result ActionResult { get; protected set; }

        /// <summary>
        /// The binary of the screenshots taken within this action
        /// </summary>
        public List<ScreenshotReport> Screenshots { get; set; } = new List<ScreenshotReport>();
    }
}
