using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace GreenSa.Models.Tools.Services
{
    public class WindService
    {
        private const string url = "http://api.openweathermap.org/data/2.5/weather?APPID=";
        private const string appid = "1845b50acc4f9dabca4feb5b3d55fc68";
        private ImageSource img;
        /**
         * Methode allant chercher des infos sur une API web pour retrouver des infos concernant le vent.
         */
        /*async*/
        public async Task<WindInfo> getCurrentWindInfo(MyPosition position)
        {
            if (!isAvaible()) throw new NotAvaibleException();
            position = await GpsService.getCurrentPosition();
            string fullUrl = url + appid + "&lat=" + position.X + "&lon=" + position.Y;

            img = ImageSource.FromResource("GreenSa.Ressources.Images.left-arrow.png");
                   
            return await FetchData(fullUrl);
        }

        private async Task<WindInfo> FetchData(string url)
        {
            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            // Send the request to the server and wait for the response:
            using (WebResponse response = await request.GetResponseAsync())
            {
                // Get a stream representation of the HTTP web response:
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader streamReader = new StreamReader(stream))
                    {
                        string json = streamReader.ReadToEnd();
                        double speed = 0, deg = 0;
                        JsonTextReader reader = new JsonTextReader(new StringReader(json));

                        while (reader.Read())
                        {
                            if(reader.Value != null && reader.TokenType.ToString() == "PropertyName")
                            {   
                                //if(reader.TokenType.ToString() == "String")
                                Debug.WriteLine("Token : "+reader.Value);
                                switch(((string) reader.Value)) {
                                    case "speed":
                                        reader.Read();
                                        Debug.WriteLine("Value : " + reader.Value);
                                        speed = (Convert.ToDouble(reader.Value));
                                        break;
                                    case "deg":
                                        reader.Read();
                                        Debug.WriteLine("Value : " + reader.Value);
                                        deg = (Convert.ToDouble(reader.Value));
                                        break;
                                }
                            }
                        }
                        return new WindInfo(speed, deg, img);
                        
                    }
                }
            }
            return null;
        }



        public static bool isAvaible()
        {
            return true;
            throw new NotImplementedException();
        }


    }
}
