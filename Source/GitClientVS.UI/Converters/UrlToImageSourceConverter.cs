using GitClientVS.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using Svg;

namespace GitClientVS.UI.Converters
{
    public class UrlToImageSourceConverter : BaseMarkupExtensionConverter
    {
        private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var url = (string)value;
                if (_cache.ContainsKey(url))
                    return _cache[url];

                var client = new HttpClient();
                var response = client.GetAsync(url).Result;
                var filetype = response.Content.Headers.ContentType.MediaType;
                var buffer = response.Content.ReadAsByteArrayAsync().Result;

                object res;
                if (filetype.Contains("svg", StringComparison.InvariantCultureIgnoreCase))
                {
                    res = GetSvgImage(buffer);
                    _cache.Add(url, res);
                    return res;
                }

                res = GetImage(buffer);
                _cache.Add(url, res);
                return res;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private object GetSvgImage(byte[] buffer)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Encoding.Default.GetString(buffer));
            var svgDocument = SvgDocument.Open(doc);
            using (var smallBitmap = svgDocument.Draw())
            {
                var source = BitmapToImageSource(smallBitmap);
                return source;
            }
        }

        private object GetImage(byte[] buffer)
        {
            var bitmap = new BitmapImage();
            using (var stream = new MemoryStream(buffer))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();
            }

            return bitmap;
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                bitmapimage.Freeze();

                return bitmapimage;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
