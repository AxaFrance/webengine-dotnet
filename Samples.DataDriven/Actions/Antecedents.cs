using AXA.WebEngine;
using AXA.WebEngine.Web;


namespace Samples.DataDriven.Actions
{
    public class Antecedents : SharedActionWeb
    {
        public override Variable[]? RequiredParameters => null;

        public override void DoAction()
        {
            var model = new PageModels.PageUnderWriting(Browser);
            var antecedent = GetParameter(ParameterList.HasAntecedents);
            model.AntecedentsRadioGroup.CheckByValue(antecedent);

            // If the data provided by antecedent, there is nothing else to do.
            // otherwise, fill the details of the antecedent.
            if(antecedent == "yes")
            {
                model.AntecedentType.SelectByText(GetParameter(ParameterList.AccidentType));
                model.AntecedentDate.SendKeys(GetParameter(ParameterList.AccidentDate));
                model.AntecedentResponsability.CheckByValue(GetParameter(ParameterList.AccidentResponsability));
            }

            model.NextStep.Click();
        }

        public override bool DoCheckpoint()
        {
            return true;
        }
    }
}
