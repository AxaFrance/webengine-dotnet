using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;


namespace Samples.DataDriven.Actions
{
    public class HomeLocation : SharedActionWeb
    {
        public override Variable[] RequiredParameters => null;

        public override void DoAction()
        {
            var model = new PageModels.PageUnderWriting(Browser);
            model.StreetNumber.SetValue(GetParameter(ParameterList.StreetNumber));
            model.StreetName.SetValue(GetParameter(ParameterList.SteetName));
            model.City.SetValue(GetParameter(ParameterList.City));
            model.PostCode.SetValue(GetParameter(ParameterList.PostCode));
            model.Region.SetValue(GetParameter(ParameterList.Region));
            model.Country.SelectByText(GetParameter(ParameterList.Country));
        }

        public override bool DoCheckpoint()
        {
            return true;
        }
    }
}