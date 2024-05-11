using Azure.AI.Vision.ImageAnalysis;
using Azure;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplicationOCR.Models;

namespace WebApplicationOCR.Controllers
{
    public class HomeController : Controller
    {
        private readonly string endpoint = "https://axs-pruebaocr.cognitiveservices.azure.com/";
        private readonly string key = "e6f144686897421db8eb396dd61152a7";

        [HttpPost]
        public IActionResult AnalyzeImage(IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    // Copiar los datos de la imagen al MemoryStream
                    image.CopyTo(memoryStream);
                    memoryStream.Position = 0; // Establecer la posición al principio

                    // Crear el cliente de análisis de imágenes
                    var client = new ImageAnalysisClient(new Uri(endpoint), new AzureKeyCredential(key));

                    // Analizar la imagen
                    ImageAnalysisResult result = client.Analyze(
                        BinaryData.FromStream(memoryStream),
                        VisualFeatures.Read,
                        new ImageAnalysisOptions { GenderNeutralCaption = true });

                    return View("Index", result);
                }
            }
            return RedirectToAction("Index");
        }
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
