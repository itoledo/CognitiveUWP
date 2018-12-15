using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Media.Capture;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CognitiveUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        const string key = "";
        const string endpoint = "https://eastus.api.cognitive.microsoft.com/";

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;

            var file = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            var features = new List<VisualFeatureTypes>()
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags
            };

            var VisionServiceClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(key));
            VisionServiceClient.Endpoint = endpoint;
            using (Stream imageFileStream = await file.OpenStreamForReadAsync())
            {
                var analysisResult = await VisionServiceClient.AnalyzeImageInStreamAsync(imageFileStream, features);
                var captions = string.Join(Environment.NewLine, analysisResult?.Description?.Captions?.Select(c => $"{c.Text} ({c.Confidence})") ?? Enumerable.Empty<string>());
                await new MessageDialog(captions).ShowAsync();
            }
        }
    }
}
