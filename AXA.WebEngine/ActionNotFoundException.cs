using System;
using System.Collections.Generic;
using System.Text;

namespace AXA.WebEngine
{
    /// <summary>
    /// The keyword-action is not found.
    /// </summary>
    public class ActionNotFoundException : Exception
    {
        /// <summary>
        /// Initializing an instance of ActionNotFoundException
        /// </summary>
        public ActionNotFoundException() { }


        /// <summary>
        /// Initializing an instance of ActionNotFoundException with detailed error message.
        /// </summary>
        /// <param name="message">error message</param>
        public ActionNotFoundException(string message): base(message){}
    }
}
