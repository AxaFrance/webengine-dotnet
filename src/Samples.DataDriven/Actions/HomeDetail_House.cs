using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;


namespace Samples.DataDriven.Actions
{
    public class HomeDetail_House : SharedActionWeb
    {
        public override Variable[] RequiredParameters => null;

        public override void DoAction()
        {
            var model = new PageModels.PageUnderWriting(Browser);
            model.HouseFloors.SendKeys(GetParameter(ParameterList.HouseFloors));
            model.HouseBackyardSurface.SendKeys(GetParameter(ParameterList.BackyardSurface));
            model.HouseHasSwimmingPoolRadioGroup.CheckByValue(GetParameter(ParameterList.HasSwimmingPool));
        }

        public override bool DoCheckpoint()
        {
            return true;
        }
    }
}