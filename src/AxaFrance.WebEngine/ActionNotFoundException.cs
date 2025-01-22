using System;

namespace AxaFrance.WebEngine
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
        public ActionNotFoundException(string message) : base(message) { }
    }
}
