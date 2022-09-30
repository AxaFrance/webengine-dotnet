using AXA.WebEngine;
using AXA.WebEngine.Web;


namespace Samples.KeywordDriven.Actions
{
    public class HomeDetails : SharedActionWeb
    {
        public override Variable[]? RequiredParameters => null;

        public override void DoAction()
        {
            var model = new PageModels.PageUnderWriting(Browser);
            string hometype = "apartment";
            
            model.TypeOfHomeRadioGroup.CheckByValue(hometype);
            model.NumberOfRoomSelect.SelectByValue("3");
            model.HomeSurface.SetValue("67.31");

            var result = DoActionWithCheckpoint(typeof(HomeDetail_Apartment), Browser, ContextValues, this, Parameters);
            if (result)
            {
                model.NextStep.Click();
            }
            else
            {
                this.ActionResult = Result.Failed;
                this.Information.AppendLine("Action HomeDetail_Apartment failed");
            }
        }

        public override bool DoCheckpoint()
        {
            return true;
        }
    }
}