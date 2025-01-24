using System;

namespace AxaFrance.WebEngine
{

    /// <summary>
    /// A network request record, which is used to store the information of a network request initiated by the browser.
    /// </summary>
    [Serializable]
    public class NetworkRequest
    {
        /// <summary>
        /// The time stamp of the network request.
        /// </summary>
        public long TimeStamp { get; set; }

        /// <summary>
        /// The request id of the network request.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// The http method of the network request.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The url of the network request.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// If the request is cached (to ignore the calculation of downloaded size)
        /// </summary>
        public bool IsCached { get; set; }

        /// <summary>
        /// The size of the request.
        /// </summary>
        public long Request { get; set; }

        /// <summary>
        /// The size of the response.
        /// </summary>
        public long Reponse { get; set; }

        /// <summary>
        /// The status code of the network request.
        /// </summary>
        public long StatusCode { get; set; }

        /// <summary>
        /// The type of the resource.
        /// </summary>
        public string ResourceType { get; set; }

        /// <summary>
        /// The date and time when the request was sent to server
        /// </summary>
        public DateTime? Sent { get; set; }

        /// <summary>
        /// The date and time when the response was received from server
        /// </summary>
        public DateTime? Received { get; set; }

        /// <summary>
        /// The duration of the network request.
        /// </summary>
        public TimeSpan? Duration
        {
            get
            {
                if (Sent != null && Received != null)
                {
                    return Received - Sent;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
