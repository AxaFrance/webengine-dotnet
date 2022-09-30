using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;


namespace Samples.KeywordDriven.Actions
{
    public class HomeDetail_Apartment : SharedActionWeb
    {
        public override Variable[]? RequiredParameters => null;

        public override void DoAction()
        {
            var model = new PageModels.PageUnderWriting(Browser);
            model.ApptTotalFloors.SelectByText("Between 4 to 7 floors");
            model.ApptMyFloor.SetValue("3");
            model.ApptHasElevatorRadioGroup.CheckByValue("yes");
        }

        public override bool DoCheckpoint()
        {
            return true;
        }
    }
}
