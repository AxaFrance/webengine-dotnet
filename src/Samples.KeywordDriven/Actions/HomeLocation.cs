using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;


namespace Samples.KeywordDriven.Actions
{
    public class HomeLocation : SharedActionWeb
    {
        public override Variable[]? RequiredParameters => null;

        public override void DoAction()
        {
            var model = new PageModels.PageUnderWriting(Browser);
            model.StreetNumber.SetValue("1");
            model.StreetName.SetValue("boulvard bidon");
            model.City.SetValue("Fauxville");
            model.PostCode.SetValue("99130");
            model.Region.SetValue("Celt");
            model.Country.SelectByText("France");
        }

        public override bool DoCheckpoint()
        {
            return true;
        }
    }
}