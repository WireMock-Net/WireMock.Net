using System.Collections.Generic;

namespace WireMock.Admin.Mappings
{
    /// <summary>
    /// RequestModel
    /// </summary>
    public class RequestModel
    {
        /// <summary>
        /// Gets or sets the ClientIP. (Can be a string or a ClientIPModel)
        /// </summary>
        /// <value>
        /// The ClientIP.
        /// </value>
        public object ClientIP { get; set; }

        /// <summary>
        /// Gets or sets the Path. (Can be a string or a PathModel)
        /// </summary>
        /// <value>
        /// The Path.
        /// </value>
        public object Path { get; set; }

        /// <summary>
        /// Gets or sets the Url. (Can be a string or a UrlModel)
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public object Url { get; set; }

        /// <summary>
        /// The methods
        /// </summary>
        public string[] Methods { get; set; }

        /// <summary>
        /// Gets or sets the Headers.
        /// </summary>
        /// <value>
        /// The Headers.
        /// </value>
        public IList<HeaderModel> Headers { get; set; }

        /// <summary>
        /// Gets or sets the Cookies.
        /// </summary>
        /// <value>
        /// The Cookies.
        /// </value>
        public IList<CookieModel> Cookies { get; set; }

        /// <summary>
        /// Gets or sets the Params.
        /// </summary>
        /// <value>
        /// The Headers.
        /// </value>
        public IList<ParamModel> Params { get; set; }
        
        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public BodyModel Body { get; set; }
    }
}