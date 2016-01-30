using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web.Http;
using ImageResizer;

namespace rposbo.clienthints.api.Controllers
{
    public class ImageController : ApiController
    {
        public HttpResponseMessage Get(string id)
        {
            var pixelDensity = Request.Headers.GetValues("DPR").FirstOrDefault() != null ? int.Parse(Request.Headers.GetValues("DPR").FirstOrDefault()) : 1;
            var width = (Request.Headers.GetValues("Width").FirstOrDefault() != null ? double.Parse(Request.Headers.GetValues("Width").FirstOrDefault()) : 0);
            var newWidth = width * pixelDensity;

            var imageBytes = GetOriginalImage(id);
            var supportsClientHints = width > 0;

            var newImageBytes = supportsClientHints ? ResizeImage(imageBytes, newWidth) : imageBytes;
            var response = BuildImageResponse(newImageBytes, supportsClientHints);

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

        private static HttpResponseMessage BuildImageResponse(MemoryStream memoryStream, bool supportsClientHints)
        {
            var httpResponseMessage = new HttpResponseMessage { Content = new ByteArrayContent(memoryStream.ToArray()) };
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue($"image/jpg");
            httpResponseMessage.Content.Headers.Add("ResizedViaClientHints", (supportsClientHints ? "YES!" : "Nope"));
            httpResponseMessage.StatusCode = HttpStatusCode.OK;

            return httpResponseMessage;
        }
    }
}
