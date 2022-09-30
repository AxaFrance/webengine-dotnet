using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;


namespace Samples.DataDriven.Actions
{
    public class Logout : SharedActionWeb
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