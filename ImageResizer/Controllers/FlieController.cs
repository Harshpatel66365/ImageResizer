using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using SixLabors.ImageSharp;
using Image = SixLabors.ImageSharp.Image;
using SixLabors.ImageSharp.Processing;

namespace ImageResizer.Controllers
{
    public class FlieController : Controller
    {

        private readonly IFileProvider _fileProvider;

        public FlieController(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }
        [Route("/thumb/{TimeStamp}/{width}x{height}/{*url}")]
        public IActionResult ResizeImaage(string url, long TimeStamp, int width, int height)
        {
            return ResizeImage(url, TimeStamp, width, height);
        }
        [Route("/thumb/download/{SKUID}/{DataVersion}/{SKUType}/{width}x{height}/VendorAssets/{VendorID}/{*url}")]
        public IActionResult ResizeImage(string url, long TimeStamp, int width, int height, string type = "", long SKUID = 0, int DataVersion=0)
        {
            try
            {
                // Preconditions and sanitization
                // Check the original image exists
                url = $"/{url}";
                var originalPath = PathString.FromUriComponent("/" + url);
                var fileInfo = _fileProvider.GetFileInfo(originalPath);
                if (!fileInfo.Exists) { return NotFound(); }

                string resizedPath = "";
                // Replace the extension on the file (we only resize to jpg currently) 
                if (type != "none")
                    resizedPath = ReplaceExtension($"/thumb/download/{SKUID}/{DataVersion}/{width}x{height}/{TimeStamp}/{url}");
                else
                    resizedPath = ReplaceExtension($"/thumb/{width}x{height}/{TimeStamp}/{url}");

                // Use the IFileProvider to get an IFileInfo
                var resizedInfo = _fileProvider.GetFileInfo(resizedPath);
                // Create the destination folder tree if it doesn't already exist
                Directory.CreateDirectory(Path.GetDirectoryName(resizedInfo.PhysicalPath));

                using (FileStream stream = new FileStream(fileInfo.PhysicalPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (Image img1 = Image.Load(stream))
                    {
                        img1.Mutate(x => x.Resize(width, height)); // Resize the image
                        img1.Save(resizedInfo.PhysicalPath); // Save the resized image
                    }
                }
                return PhysicalFile(resizedInfo.PhysicalPath, "image/jpg");
            }
            catch (Exception ex)
            {
                // Error handling
                // ErrorMgmt.AddError(ex, "ResizeImage");
                throw;
            }
        }

        // Helper method to replace the file extension
        private string ReplaceExtension(string path)
        {
            // Replace the extension with jpg
            return Path.ChangeExtension(path, ".jpg");
        }

    }
}
