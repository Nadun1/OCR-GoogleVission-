using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Vision.v1;
using Google.Apis.Vision.v1.Data;


namespace OCRTestUsingGoogleVision
{
    class Program
    {
        private static void Main(string[] args)
        {
            Program sample = new Program();
            //string imagePath = Path.Combine("~/Images", "test.jpeg");
            string imagePath = "C:/Users/nadun/source/repos/OCRTestUsingGoogleVision/Images/test.jpeg";
            // Create a new Cloud Vision client authorized via Application
            // Default Credentials
            VisionService vision = sample.CreateAuthorizedClient();
            // Use the client to get text annotations for the given image
            IList<AnnotateImageResponse> result = sample.DetectText(
                vision, imagePath);
            // Check for valid text annotations in response
            if (result[0].TextAnnotations != null)
            {


                Console.WriteLine(result[0].TextAnnotations[0].Description);

            }
            else
            {
                if (result[0].Error == null)
                {
                    Console.WriteLine("No text found.");
                }
                else
                {
                    Console.WriteLine("Not a valid image.");
                }
            }
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        public VisionService CreateAuthorizedClient()
        {
            GoogleCredential credential =
                GoogleCredential.GetApplicationDefaultAsync().Result;
            // Inject the Cloud Vision scopes
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    VisionService.Scope.CloudPlatform
                });
            }
            return new VisionService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                GZipEnabled = false
            });
        }

        /// <summary>
        /// Detect text within an image using the Cloud Vision API.
        /// </summary>
        /// <param name="vision">an authorized Cloud Vision client.</param>
        /// <param name="imagePath">the path where the image is stored.</param>
        /// <returns>a list of text detected by the Vision API for the image.
        /// </returns>
        public IList<AnnotateImageResponse> DetectText(
            VisionService vision, string imagePath)
        {
            Console.WriteLine("Detecting Text...");
            string path = Directory.GetCurrentDirectory();
            //Console.WriteLine(path);
            string root = Path.GetPathRoot(path);
            Console.WriteLine(root);
            // Convert image to Base64 encoded for JSON ASCII text based request
            byte[] imageArray = System.IO.File.ReadAllBytes(imagePath);
            string imageContent = Convert.ToBase64String(imageArray);
            // Post text detection request to the Vision API
            var responses = vision.Images.Annotate(
                new BatchAnnotateImagesRequest()
                {
                    Requests = new[] {
                    new AnnotateImageRequest() {
                        Features = new [] { new Feature() { Type =
                          "TEXT_DETECTION"}},
                        Image = new Image() { Content = imageContent }
                    }
               }
                }).Execute();
            return responses.Responses;
        }
    }
}
