using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;


namespace Samples.DataDriven.Actions
{
    public class HomeDetails : SharedActionWeb
    {
        public override Variable[] RequiredParameters => null;

        public override void DoAction()
        {
            var model = new PageModels.PageUnderWriting(Browser);
            string hometype = GetParameter(ParameterList.HomeType); //Get home type from test data
            model.TypeOfHomeRadioGroup.CheckByValue(hometype);
            model.NumberOfRoomSelect.SelectByValue(GetParameter(ParameterList.NumberOfRoom));
            model.HomeSurface.SetValue(GetParameter(ParameterList.HomeSurface));

            bool result;
            if (hometype == "house")
            {
                //if hometype is house, run action for House
                result = DoActionWithCheckpoint(typeof(HomeDetail_House), Browser, ContextValues, this, Parameters);
            }
            else
            {
                //if hometype is not house, run action for apartment
                result = DoActionWithCheckpoint(typeof(HomeDetail_Apartment), Browser, ContextValues, this, Parameters);
            }
            RunAccessibilityTest("HomeDetails");
            if (result)
            {
                model.NextStep.Click();
            }
            else
            {
                this.ActionResult = Result.Failed;
                this.Information.AppendLine("Action failed");
            }
        }

        public override bool DoCheckpoint()
        {
            return true;
        }
    }
}