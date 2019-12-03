using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace GreenSa.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            Xamarin.FormsMaps.Init("JNiuoOLXpoPjeXSUVb2P~nLg-m53DPj5cHy6ahqCMjQ~AmDtO279pBQlyX104yC8s0dzUecCaPXMmeRk02ejtYi7BnJKdLuK5_CTT4-8RtGG");
            LoadApplication(new GreenSa.App());
        }
    }
}
