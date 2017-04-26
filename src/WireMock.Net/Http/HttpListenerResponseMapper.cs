//using System.Linq;
//using System.Net;
//using System.Text;
//#if NET45

//#else
//using System.Net.Http;
//#endif

//namespace WireMock.Http
//{
//    /// <summary>
//    /// The http listener response mapper.
//    /// </summary>
//    public class HttpListenerResponseMapperOld
//    {
//        private readonly Encoding _utf8NoBom = new UTF8Encoding(false);

//        /// <summary>
//        /// The map.
//        /// </summary>
//        /// <param name="responseMessage">
//        /// The response.
//        /// </param>
//        /// <param name="listenerResponse">The listenerResponse.</param>
//        public void MapAsync(ResponseMessage responseMessage, HttpListenerResponse listenerResponse)
//        {
//            listenerResponse.StatusCode = responseMessage.StatusCode;

//            responseMessage.Headers.ToList().ForEach(pair => listenerResponse.AddHeader(pair.Key, pair.Value));

//            if (responseMessage.Body == null)
//                return;

//            var encoding = responseMessage.BodyEncoding ?? _utf8NoBom;
//            byte[] buffer = encoding.GetBytes(responseMessage.Body);

//            listenerResponse.ContentEncoding = encoding;
//            listenerResponse.ContentLength64 = buffer.Length;
//            listenerResponse.OutputStream.Write(buffer, 0, buffer.Length);
//            listenerResponse.OutputStream.Flush();
//        }
//    }
//}