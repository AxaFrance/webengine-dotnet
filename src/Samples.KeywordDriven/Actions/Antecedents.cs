using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;


namespace Samples.KeywordDriven.Actions
{
    public class Antecedents : SharedActionWeb
    {
        public override Variable[]? RequiredParameters => null;

        public override void DoAction()
        {
            var model = new PageModels.PageUnderWriting(Browser);
            model.AntecedentsRadioGroup.CheckByValue("no");
            model.NextStep.Click();
        }

        public override bool DoCheckpoint()
        {
            return true;
        }
    }
}
