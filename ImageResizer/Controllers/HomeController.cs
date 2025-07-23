using ImageResizer.Models;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using System.Diagnostics;
using ImageResizer.Entity;

namespace ImageResizer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

            // Get a list of image files and their last modified timestamps
            var imageList = GetImageList(wwwrootPath);

            return View(imageList);
        }

        private IEnumerable<ImageResizer.Entity.ImageInfo> GetImageList(string folderPath)
        {
            // Check if the folder exists
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException($"Folder not found: {folderPath}");
            }


            // Get image files and their last modified timestamps
            return Directory.GetFiles(folderPath, "*.jpg")
                .Select(filePath => new ImageResizer.Entity.ImageInfo
                {
                    FileName = Path.GetFileName(filePath),
                    LastModified = new FileInfo(filePath).LastWriteTime.Ticks
                })
                .ToList();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}