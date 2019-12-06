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
        public object ClientIP { get; set; }

        /// <summary>
        /// Gets or sets the Path. (Can be a string or a PathModel)
        /// </summary>
        public object Path { get; set; }

        /// <summary>
        /// Gets or sets the Url. (Can be a string or a UrlModel)
        /// </summary>
        public object Url { get; set; }

        /// <summary>
        /// The methods
        /// </summary>
        public string[] Methods { get; set; }

        /// <summary>
        /// Gets or sets the Headers.
        /// </summary>
        public IList<HeaderModel> Headers { get; set; }

        /// <summary>
        /// Gets or sets the Cookies.
        /// </summary>
        public IList<CookieModel> Cookies { get; set; }

        /// <summary>
        /// Gets or sets the Params.
        /// </summary>
        public IList<ParamModel> Params { get; set; }
        
        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public BodyModel Body { get; set; }
    }
}