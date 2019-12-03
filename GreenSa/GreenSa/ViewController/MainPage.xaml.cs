using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GreenSa.ViewController.Play;
using GreenSa.ViewController.Option;
using GreenSa.ViewController.MesGolfs;
using GreenSa.ViewController.Profile;
using GreenSa.Models.GolfModel;
using GreenSa.Models.ViewElements;
using SQLite;
using System.Collections.ObjectModel;
using GreenSa.Persistence;
using GreenSa.Models.Profiles;
using GreenSa.ViewController.Test;

namespace GreenSa.ViewController
{
    public partial class MainPage : ContentPage
    {
        private SQLiteConnection DBconnection;

        public MainPage()
        {
            InitializeComponent();
            this.InitBDD();
        }

        protected override void OnAppearing()
        {
            this.titre.Margin = new Thickness(0, 0, 0, 42);
        }

        /**
         * Initialized the database and adds a new profile if no one exists
         */
        public void InitBDD()
        {
            DBconnection = DependencyService.Get<ISQLiteDb>().GetConnection();
            DBconnection.CreateTable<Profil>();
            if (!DBconnection.Table<Profil>().Any())
            {
                AddLocalUser();
            }
        }

        /**
         * Adds a new profile in the database
         */
        public void AddLocalUser()
        {
            DBconnection.Insert(new Profil());
        }

        /**
         * Transforms a given value in pixel so that it's responsive with the screen size
         * pix : the value in pixel
         * return the converted value as an integer
         */
        public static int responsiveDesign(int pix)
        {
            return (int)((pix * 4.1 / 1440.0) * Application.Current.MainPage.Width);
        }


        /**
         * These methods are called when the corresponding button is pressed and redirects to a new page  
         */
        async private void OnPlayClicked(object sender, EventArgs e)
        {
            Partie partie = new Partie();
            await Navigation.PushAsync(new Play.GolfSelectionPage(partie));
        }


        async private void OnProfilClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage());
        }


        async private void OnGolfClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GolfsManager());
        }


        async private void OnOptionsClicked(object sender, EventArgs e){ 
            await Navigation.PushAsync(new OptionPage());
        }
    }
}
