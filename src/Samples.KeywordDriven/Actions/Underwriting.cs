using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;

namespace Samples.KeywordDriven.Actions
{
    public class Underwriting : SharedActionWeb
    {
        bool result = false;
        public override Variable[]? RequiredParameters => null;

        public override void DoAction()
        {
            var resultLocation = DoActionWithCheckpoint(typeof(HomeLocation), Browser, ContextValues, this, Parameters);
            if (resultLocation)
            {
                var resultDetails = DoActionWithCheckpoint(typeof(HomeDetails), Browser, ContextValues, this, Parameters);
                if (resultDetails)
                {
                    result = DoActionWithCheckpoint(typeof(Antecedents), Browser, ContextValues, this, Parameters);
                }
            }
        }

        public override bool DoCheckpoint()
        {
            return result;
        }
    }
}
