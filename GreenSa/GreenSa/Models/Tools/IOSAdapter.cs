using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace GreenSa.Models.Tools
{
    class IOSAdapter
    {

        private static Color green = Color.FromHex("3ab54a");

        /*
         * This method will automatically create margins on the top and bottom of the screen
         * if the device is under iOS, preventing the app from running fullscreen and colliding with the iOS UI/the notch
         * Parameters :
         * p : the page calling this method
         * color : a string referencing the color to be used for the margin
         */
        public static void SafeArea(ContentPage p, String color)
        {
            p.On<iOS>().SetUseSafeArea(true);//Sets the margins
            p.Content.BackgroundColor = Color.White;

            if(color.Equals("green")) p.BackgroundColor = green;//Colors the margin in green if needed
        }
    }
}
