using GreenSa.Models.Tools.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GreenSa.ViewController.Test
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WindServicePage : ContentPage
    {
        public WindServicePage()
        {
            InitializeComponent();
        }

        private async void updateWind(object sender, EventArgs e)
        {
            WindService service = new WindService();
            WindInfo x = await service.getCurrentWindInfo();
            strength.Text = x.strength.ToString();
            windImg.Source = x.icon;
            await windImg.RotateTo(90+x.direction);
        }
    }
}