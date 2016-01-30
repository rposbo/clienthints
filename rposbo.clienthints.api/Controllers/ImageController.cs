using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using ImageResizer;

namespace rposbo.clienthints.api.Controllers
{
    public class ImageController : ApiController
    {
        private double _pixelDensity;

        public HttpResponseMessage Get(string id)
        {
            var imageBytes = GetOriginalImage(id);

            _pixelDensity = double.Parse(Request.Headers.GetValues("DPR") != null ? Request.Headers.GetValues("DPR").First() : "1");
            var width =  double.Parse(Request.Headers.GetValues("Width") != null ? Request.Headers.GetValues("Width").First() : "0");

            var supportsClientHints = width > 0;
            var newImageBytes = supportsClientHints ? ResizeImage(imageBytes, width) : imageBytes;
            var response = BuildImageResponse(newImageBytes);

            return response;
        }

        private static MemoryStream GetOriginalImage(string imageName)
        {
            // ignore the filename for the sake of the demo
            using (var wc = new WebClient())
            {
                return new MemoryStream(wc.DownloadData("https://otomotech.blob.core.windows.net/demo/inle_lake.jpg"));
            }
        }

        private static MemoryStream ResizeImage(Stream inputStream, double width)
        {
            var memoryStream = new MemoryStream();
            var i = new ImageJob(inputStream, memoryStream, new Instructions($"width={width}"));
            i.Build();
            return memoryStream;
        }

        private HttpResponseMessage BuildImageResponse(MemoryStream memoryStream)
        {
            var httpResponseMessage = new HttpResponseMessage { Content = new ByteArrayContent(memoryStream.ToArray()) };
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue($"image/jpg");
            httpResponseMessage.Content.Headers.Add("Content-DPR", _pixelDensity.ToString(CultureInfo.InvariantCulture));
            httpResponseMessage.StatusCode = HttpStatusCode.OK;

            return httpResponseMessage;
        }
    }
}
