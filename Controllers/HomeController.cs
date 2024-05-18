using Azure.AI.Vision.ImageAnalysis;
using Azure;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplicationOCR.Models;
using Azure.Storage.Blobs;

namespace WebApplicationOCR.Controllers
{
    public class HomeController : Controller
    {
        private readonly string endpoint = "https://axs-pruebaocr.cognitiveservices.azure.com/";
        private readonly string key = "e6f144686897421db8eb396dd61152a7";
        private readonly string connectionString = "DefaultEndpointsProtocol=https;AccountName=webdemosaxs;AccountKey=5wUkTiKjGUps0l8e3waNoZGFjq71wMhzHMoR8zJYNeVNTaOtG0yAur4SO1dG0y8qnRbz7bR8nDSw+AStWCGwVw==;EndpointSuffix=core.windows.net";
        private readonly string containerName = "attachfiles";
        //private readonly string blobName = "imagen-prueba6.jpg";


        [HttpPost]
        public IActionResult AnalyzeImage(HomeModel model)
        {
            if (model.File != null && model.File.Length > 0)
            {
                return AnalyzeAndUploadImage(model.File.OpenReadStream());
            }
            else if (!string.IsNullOrEmpty(model.CapturedImage))
            {
                // parte el texto y manda lo que se necesita
                string base64Image = model.CapturedImage.Split(',')[1];
                // Convertir la cadena base64 en un array de bytes
                byte[] imageBytes = Convert.FromBase64String(base64Image);

                using (var memoryStream = new MemoryStream(imageBytes))
                {
                    return AnalyzeAndUploadImage(memoryStream);
                }
            }

            return View("Index");
        }

        private IActionResult AnalyzeAndUploadImage(Stream imageStream)
        {

            string uri = "Imagen_" + Guid.NewGuid() + ".jpg";

            // Subir la imagen al contenedor
            using (var memoryStream = new MemoryStream())
            {
                imageStream.CopyTo(memoryStream);
                memoryStream.Position = 0;

                BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
                BlobClient blob = container.GetBlobClient(uri);
                blob.Upload(memoryStream);
            }
            //regresar a la posicion inicial
            imageStream.Seek(0, SeekOrigin.Begin);

            // Analizar la imagen

            var client = new ImageAnalysisClient(new Uri(endpoint), new AzureKeyCredential(key));
            ImageAnalysisResult result = client.Analyze(
                BinaryData.FromStream(imageStream),
                VisualFeatures.Read,
                new ImageAnalysisOptions { GenderNeutralCaption = true });

            return View("Index", result);
        }

        //public IActionResult AnalyzeImage(HomeModel model)
        //{
        //    Guid g = Guid.NewGuid();
        //    String uri = "Imagen_"+g+".jpg";

        //    if (model.File != null && model.File.Length > 0)
        //    {
        //        using (var memoryStream = new MemoryStream())
        //        {

        //            model.File.CopyTo(memoryStream);
        //            memoryStream.Position = 0;

        //            BlobContainerClient container = new BlobContainerClient(connectionString, containerName);

        //            BlobClient blob = container.GetBlobClient(uri);

        //            blob.Upload(memoryStream);

        //            memoryStream.Seek(0, SeekOrigin.Begin);

        //            var client = new ImageAnalysisClient(new Uri(endpoint), new AzureKeyCredential(key));

        //            // Analizar la imagen
        //            ImageAnalysisResult result = client.Analyze(
        //                BinaryData.FromStream(memoryStream),
        //                VisualFeatures.Read,
        //                new ImageAnalysisOptions { GenderNeutralCaption = true });

        //            return View("Index", result);
        //        }
        //    }
        //    else if (!string.IsNullOrEmpty(model.CapturedImage))
        //    {
        //        // Elimina "data:image/jpeg;base64," y toma la parte restante
        //        string base64Image = model.CapturedImage.Split(',')[1];

        //        // Convertir la cadena base64 en un array de bytes
        //        byte[] imageBytes = Convert.FromBase64String(base64Image);

        //        // Crear un MemoryStream a partir del array de bytes
        //        using (var memoryStream = new MemoryStream(imageBytes))
        //        {
        //            memoryStream.Position = 0;

        //            BlobContainerClient container = new BlobContainerClient(connectionString, containerName);

        //            BlobClient blob = container.GetBlobClient(uri);

        //            blob.Upload(memoryStream);

        //            memoryStream.Seek(0, SeekOrigin.Begin);

        //            var client = new ImageAnalysisClient(new Uri(endpoint), new AzureKeyCredential(key));

        //            // Analizar la imagen
        //            ImageAnalysisResult result = client.Analyze(
        //                BinaryData.FromStream(memoryStream),
        //                VisualFeatures.Read,
        //                new ImageAnalysisOptions { GenderNeutralCaption = true });

        //            return View("Index", result);
        //        }
        //    }

        //    return View("Index");
        //}

        //public IActionResult AnalyzeImage(IFormFile image)
        //{

        //    if (image != null && image.Length > 0)
        //    {
        //        using (var memoryStream = new MemoryStream())
        //        {

        //            image.CopyTo(memoryStream);
        //            memoryStream.Position = 0;

        //            BlobContainerClient container = new BlobContainerClient(connectionString, containerName);

        //            BlobClient blob = container.GetBlobClient(blobName);

        //            blob.Upload(memoryStream);

        //            memoryStream.Seek(0, SeekOrigin.Begin);

        //            var client = new ImageAnalysisClient(new Uri(endpoint), new AzureKeyCredential(key));

        //            // Analizar la imagen
        //            ImageAnalysisResult result = client.Analyze(
        //                BinaryData.FromStream(memoryStream),
        //                VisualFeatures.Read,
        //                new ImageAnalysisOptions { GenderNeutralCaption = true });



        //            return View("Index", result);
        //        }
        //    }
        //    return RedirectToAction("Index");
        //}
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
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
