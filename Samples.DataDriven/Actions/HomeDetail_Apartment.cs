using AXA.WebEngine;
using AXA.WebEngine.Web;


namespace Samples.DataDriven.Actions
{
    public class HomeDetail_Apartment : SharedActionWeb
    {
        public override Variable[]? RequiredParameters => null;

        public override void DoAction()
        {
            var model = new PageModels.PageUnderWriting(Browser);
            model.ApptTotalFloors.SelectByText(GetParameter(ParameterList.ApptTotalFloors));
            model.ApptMyFloor.SetValue(GetParameter(ParameterList.ApptMyFloors));
            model.ApptHasElevatorRadioGroup.CheckByValue(GetParameter(ParameterList.ApptHasElevator));
        }

        public override bool DoCheckpoint()
        {
            return true;
        }
    }
}
