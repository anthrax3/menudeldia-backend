using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Channels;
using System.Web;
using System.Web.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace MenuDelDia.API.API.Site
{
    public class FileApiController : ApiBaseController
    {
        [HttpPost]
        [Authorize]
        [Route("api/site/file/upload")]
        public HttpResponseMessage Upload()
        {
            var request = HttpContext.Current.Request;

            if (request.Files.Count == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var restaurant = CurrentAppContext.Restaurants.FirstOrDefault(r => r.Id == CurrentRestaurantId);

            if (restaurant == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var file = request.Files[0];
            var extension = file.FileName.Substring(file.FileName.LastIndexOf('.') + 1);
            var name = string.Format("{0}_{1}", Guid.NewGuid(), extension);

            using (Stream stream = file.InputStream)
            {
                UploadFile(name, stream);
            }

            restaurant.LogoExtension = extension;
            restaurant.LogoName = name;
            restaurant.LogoPath = name;

            CurrentAppContext.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK);
        }


        [HttpGet]
        [Route("api/site/file/view/{imageName}")]
        public HttpResponseMessage View(string imageName)
        {
            try
            {

                if (string.IsNullOrEmpty(imageName))
                    return null;

                var extension = imageName.Substring(imageName.LastIndexOf('_') + 1);
                var temp = DownloadFile(imageName);

                var result = new HttpResponseMessage(HttpStatusCode.OK);
                Stream stream = new MemoryStream(temp);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = imageName.Replace("_",".");
                result.Content.Headers.ContentType = new MediaTypeHeaderValue(string.Format("image/{0}", extension.ToLower()));
                result.Content.Headers.ContentLength = stream.Length;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }












        public void UploadFile(string fileName, Stream file)
        {
            var connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("restaurantimages");
            container.CreateIfNotExists();

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            // Create or overwrite the "myblob" blob with contents from a local file.
            blockBlob.UploadFromStream(file);
        }

        public byte[] DownloadFile(string fileName)
        {
            var connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("restaurantimages");
            container.CreateIfNotExists();

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            // Save blob contents to a file.
            if (blockBlob.Exists())
            {
                blockBlob.FetchAttributes();
                long fileByteLength = blockBlob.Properties.Length;
                Byte[] myByteArray = new Byte[fileByteLength];
                blockBlob.DownloadToByteArray(myByteArray, 0);
                return myByteArray;
            }
            return new byte[0];
        }
    }
}