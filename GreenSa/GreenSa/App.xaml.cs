using GreenSa.Models.GolfModel;
using GreenSa.ViewController;
using GreenSa.ViewController.Play.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace GreenSa
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());
        }


        protected override void OnStart()
        {
            //Create some data to test some features
            int number = 0;
            for (int i = 0; i < number; i++)
            {
                TestClassFactory.CreateScorePartie();
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
