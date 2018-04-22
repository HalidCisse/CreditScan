using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace CreditScan
{
    public partial class MainPage 
    {
        private const string Key = "AIzaSyBFoeyIVIZUJAWmb5FTJkjrPbDd4T92LVM";
            
        public MainPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            CameraButton.Clicked += CameraButton_Clicked;
        }

        private async void CameraButton_Clicked(object sender, EventArgs e)
        {
            var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });
            if (photo == null) return;

            var number = await ExtractPhoneNumber(photo.GetStream());
            Device.OpenUri(new Uri($"tel:{number}"));
        }
  
        private async Task<string> ExtractPhoneNumber(Stream stream)
        {
            stream = new FileStream("recharge.jpg", FileMode.Open, FileAccess.Read);

            try
            {
                using (var ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms);

                    using (var client = new HttpClient())
                    {
                        var payload = new
                        {
                            requests = new object[] {
                                new {
                                    image = new {
                                        content = Convert.ToBase64String(ms.ToArray())

                                    },
                                    features = new object[]{
                                        new  {
                                            type = "TEXT_DETECTION"
                                        }
                                    }
                                }
                            }
                        };

                        var res = await client.PostAsync(new Uri($"https://vision.googleapis.com/v1/images:annotate?key={Key}"), new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));
                        if (!res.IsSuccessStatusCode) return string.Empty;
                        var annotation = JsonConvert.DeserializeObject<AnotationResponse>(await res.Content.ReadAsStringAsync());
                        var phoneNumber = annotation.Responses.FirstOrDefault()?.TextAnnotations?.FirstOrDefault()?.Description;
                        return phoneNumber;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return string.Empty;
            }
        }
    }
        
        
    public class AnotationResponse
    {
        public Respons[] Responses { get; set; }
    }

    public class Respons
    {
        public Textannotation[] TextAnnotations { get; set; }
        public Fulltextannotation FullTextAnnotation { get; set; }
    }

    public class Fulltextannotation
    {
        public Page[] Pages { get; set; }
        public string Text { get; set; }
    }

    public class Page
    {
        public Property1 Property { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Block[] Blocks { get; set; }
    }

    public class Property1
    {
        public Detectedlanguage[] DetectedLanguages { get; set; }
    }

    public class Detectedlanguage
    {
        public string LanguageCode { get; set; }
    }

    public class Block
    {
        public Property2 Property { get; set; }
        public Boundingbox BoundingBox { get; set; }
        public Paragraph[] Paragraphs { get; set; }
        public string BlockType { get; set; }
    }

    public class Property2
    {
        public Detectedlanguage1[] DetectedLanguages { get; set; }
    }

    public class Detectedlanguage1
    {
        public string LanguageCode { get; set; }
    }

    public class Boundingbox
    {
        public Vertex[] Vertices { get; set; }
    }

    public class Vertex
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class Paragraph
    {
        public Property3 Property { get; set; }
        public Boundingbox1 BoundingBox { get; set; }
        public Word[] Words { get; set; }
    }

    public class Property3
    {
        public Detectedlanguage2[] DetectedLanguages { get; set; }
    }

    public class Detectedlanguage2
    {
        public string LanguageCode { get; set; }
    }

    public class Boundingbox1
    {
        public Vertex1[] Vertices { get; set; }
    }

    public class Vertex1
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class Word
    {
        public Property4 Property { get; set; }
        public Boundingbox2 BoundingBox { get; set; }
        public Symbol[] Symbols { get; set; }
    }

    public class Property4
    {
        public Detectedlanguage3[] DetectedLanguages { get; set; }
    }

    public class Detectedlanguage3
    {
        public string LanguageCode { get; set; }
    }

    public class Boundingbox2
    {
        public Vertex2[] Vertices { get; set; }
    }

    public class Vertex2
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class Symbol
    {
        public Property5 Property { get; set; }
        public Boundingbox3 BoundingBox { get; set; }
        public string Text { get; set; }
    }

    public class Property5
    {
        public Detectedlanguage4[] DetectedLanguages { get; set; }
        public Detectedbreak DetectedBreak { get; set; }
    }

    public class Detectedbreak
    {
        public string Type { get; set; }
    }

    public class Detectedlanguage4
    {
        public string LanguageCode { get; set; }
    }

    public class Boundingbox3
    {
        public Vertex3[] Vertices { get; set; }
    }

    public class Vertex3
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class Textannotation
    {
        public string Locale { get; set; }
        public string Description { get; set; }
        public Boundingpoly BoundingPoly { get; set; }
    }

    public class Boundingpoly
    {
        public Vertex4[] Vertices { get; set; }
    }

    public class Vertex4
    {
        public int X { get; set; }
        public int Y { get; set; }
    }


}
