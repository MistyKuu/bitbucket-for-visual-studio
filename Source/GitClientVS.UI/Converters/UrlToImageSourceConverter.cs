﻿using GitClientVS.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using BitBucket.REST.API.Interfaces;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.Wrappers;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Infrastructure;
using GitClientVS.UI.AttachedProperties;
using Markdown.Xaml;
using RestSharp;
using RestSharp.Authenticators;
using Svg;
using Image = System.Drawing.Image;

namespace GitClientVS.UI.Converters
{
    public class UrlToImageSourceConverter : BaseMarkupExtensionConverter, IImageManager
    {
        private static IUserInformationService _userInfoService;
        private static IProxyResolver _proxyResolver;
        private static HttpClient _httpClient = new HttpClient();

        static UrlToImageSourceConverter()
        {
            ExportProvider provider = (ExportProvider)Application.Current.Resources[Consts.IocResource];
            _userInfoService = _userInfoService ?? provider.GetExportedValue<IUserInformationService>();
            _proxyResolver = _proxyResolver ?? provider.GetExportedValue<IProxyResolver>();
        }

        public Task<BitmapImage> DownloadImage(string url)
        {
            return GetImage(url);
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var url = (string)value;
            var notifier = new TaskCompletionNotifier<BitmapImage>();
            notifier.StartAsync(GetImage(url));

            return notifier;
        }

        public async Task<BitmapImage> GetImage(string url)
        {
            var httpClientHandler = new HttpClientHandler()
            {
                Proxy = (WebRequest.DefaultWebProxy ?? WebRequest.GetSystemWebProxy()),
            };
            if (httpClientHandler.Proxy != null)
                httpClientHandler.Proxy.Credentials = _proxyResolver?.GetCredentials();

            var authHeader = $"Basic {System.Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_userInfoService.ConnectionData.UserName}:{_userInfoService.ConnectionData.Password}"))}";

            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", authHeader);
            var resp = await _httpClient.GetAsync(url);
            var buffer = await resp.Content.ReadAsByteArrayAsync();

            return resp.Content.Headers.ContentType.MediaType.Contains("svg", StringComparison.InvariantCultureIgnoreCase) ? GetSvgImage(buffer) : UrlToBitmap(buffer);
        }

        private BitmapImage GetSvgImage(byte[] buffer)
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

        BitmapImage UrlToBitmap(byte[] buffer)
        {
            using (var stream = new MemoryStream(buffer))
            {
                var bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = stream;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
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
