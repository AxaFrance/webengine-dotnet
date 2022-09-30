using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;


namespace Samples.KeywordDriven.Actions
{
    public class HomeDetail_House : SharedActionWeb
    {
        public override Variable[]? RequiredParameters => null;

        public override void DoAction()
        {

        }

        public override bool DoCheckpoint()
        {
            return true;
        }
    }
}