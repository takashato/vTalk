using Imgur.API;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vTalkClient.tools
{
    class ImgurUtils
    {
        public const string ClientID = "d807c3d23ef3bd2";
        public const string ClientSecret = "6043627a8fda59b0e5576034253bf43274c4c6e2";

        public static async Task<string> UploadImage(string imgPath)
        {
            try
            {
                var client = new ImgurClient(ClientID, ClientSecret);
                var endpoint = new ImageEndpoint(client);
                IImage image;
                using (var fs = new FileStream(imgPath, FileMode.Open))
                {
                    image = await endpoint.UploadImageStreamAsync(fs);
                }

                return image.Link;
            }
            catch (ImgurException imgurEx)
            {
                return null;
                Console.WriteLine(imgurEx.Message);
            }
        }
    }
}
