using AXA.WebEngine;
using AXA.WebEngine.Web;


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