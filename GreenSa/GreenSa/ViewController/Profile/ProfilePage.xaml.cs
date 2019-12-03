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
using GreenSa.Models.Profiles;
using GreenSa.Models.ViewElements;
using SQLite;
using System.Collections.ObjectModel;
using GreenSa.Persistence;
using GreenSa.ViewController.Profile.Statistiques;

namespace GreenSa.ViewController.Profile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        private SQLiteConnection DBconnection;
        private Profil LocalUser;
        private StatistiqueMainTabbedPage statPage;

        public ProfilePage()
        {
            InitializeComponent();
            this.statPage = null;

            photo.Margin = new Thickness(0, MainPage.responsiveDesign(25), 0, MainPage.responsiveDesign(32));
            photo.HeightRequest = MainPage.responsiveDesign(140);
            user.FontSize = 25;
            engrenage.Margin = MainPage.responsiveDesign(10);
            engrenage.HeightRequest = MainPage.responsiveDesign(30);
            arrow.Margin = MainPage.responsiveDesign(10);
            arrow.HeightRequest = MainPage.responsiveDesign(25);
            golfref.Margin = new Thickness(0, MainPage.responsiveDesign(26), 0, 0);
            index.Margin = new Thickness(0, MainPage.responsiveDesign(26), 0, 0);
            niv.Margin = new Thickness(0, MainPage.responsiveDesign(26), 0, 0);
            clubs.Margin = new Thickness(0, 0, 0, MainPage.responsiveDesign(15));
            parties.Margin = new Thickness(0, 0, 0, MainPage.responsiveDesign(15));
            stats.Margin = new Thickness(0, 0, 0, MainPage.responsiveDesign(15));
            golfref.FontSize = 17;
            index.FontSize = 17;
            niv.FontSize = 17;
            golfreftitle.FontSize = 20;
            indextitle.FontSize = 20;
            nivtitle.FontSize = 20;
            clubstitle.FontSize = 19;
            partiestitle.FontSize = 19;
            statstitle.FontSize = 19;
            boutons.Margin = new Thickness(10, 0, 10, 15);

            updateLabels();
        }

        protected override void OnAppearing()
        {
            updateLabels();
        }

        /**
         * Updates the labels describing the information of the user
         */
        public void updateLabels()
        {
            LocalUser = StatistiquesGolf.getProfil();
            user.Text = LocalUser.Username;
            index.Text = LocalUser.Index.ToString();
            golfref.Text = LocalUser.GolfRef;
            photo.Source = "user" + LocalUser.Photo.ToString() + ".png";

            if (LocalUser.Index > 30) { niv.Text = "Debutant"; }
            else if (LocalUser.Index > 18) { niv.Text = "Moyen"; }
            else if (LocalUser.Index > 11) { niv.Text = "Confirmé"; }
            else if (LocalUser.Index > 5) { niv.Text = "Très bon joueur"; }
            else { niv.Text = "Compétitif"; }
        }


        /**
         * These methods are called when the corresponding button is pressed and redirects to a new page  
         */
        async private void OnClubsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyClubs.ClubSelectionPage());
        }

        async private void OnPartiesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyGames.ViewPartieListPage());
        }

        async private void OnStatsClicked(object sender, EventArgs e)
        {
            if (this.statPage == null)
            {
                this.statPage = new StatistiqueMainTabbedPage();
            }
            await Navigation.PushAsync(this.statPage);
        }

        async private void OnOptionsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Options.ProfileOptions());
        }

        async private void OnArrowClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}