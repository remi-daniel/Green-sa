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
         * if the device is under iOS, else the app runs on fullscreen and conflicts with
         * iOS UI 
         * Parameters:
         * p : the page calling this method
         * color : a string referencing the color to be used for the margin
         */
        public static void SafeArea(ContentPage p, String color)
        {
            p.On<iOS>().SetUseSafeArea(true);
            var safeInsets = p.On<iOS>().SafeAreaInsets();
            p.Content.BackgroundColor = Color.White;

            if(color.Equals("green")) p.BackgroundColor = green;

            //var layout = new StackLayout();
            //layout.BackgroundColor = Color.Green;
            //layout.Padding = safeInsets;
            //p.Content.Parent = layout;
            //p.Content = layout;
        }
    }
}
