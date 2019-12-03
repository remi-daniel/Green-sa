using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GreenSa.Persistence;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GreenSa.Models;
using GreenSa.ViewController.Play;
using GreenSa.ViewController.Option;
using GreenSa.ViewController.MesGolfs;
using GreenSa.Models.GolfModel;
using GreenSa.Models.ViewElements;
using GreenSa.Models.Profiles;

using SQLite;
using System.Collections.ObjectModel;
using GreenSa.Models.Tools;

namespace GreenSa.ViewController.Profile.Options
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfileOptions : ContentPage
    {
        private SQLiteConnection DBconnection;
        private Profil LocalUser;

        public ProfileOptions()
        {
            InitializeComponent();
            photo.Margin = new Thickness(0, MainPage.responsiveDesign(30), 0, MainPage.responsiveDesign(5));
            photo.HeightRequest = MainPage.responsiveDesign(140);
            modifier.Margin = MainPage.responsiveDesign(15);
            modifier.HeightRequest = MainPage.responsiveDesign(30);
            arrow.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));
            arrow.Margin = MainPage.responsiveDesign(-15);
            arrow.HeightRequest = MainPage.responsiveDesign(80);
            arrow.WidthRequest = MainPage.responsiveDesign(80);
            golfreftitle.FontSize = 25;
            indextitle.FontSize = 25;
            usernametitle.FontSize = 25;
            golfref.FontSize = 20;
            index.FontSize = 20;
            username.FontSize = 20;
            DBconnection = DependencyService.Get<ISQLiteDb>().GetConnection();
            updateLabels();
            boutons.Margin = new Thickness(30, 15, 30, 15);
        }

        protected override void OnAppearing()
        {
            updateLabels();
        }

        /**
         * Updates different labels of the view
         */
        private void updateLabels()
        {
            LocalUser = StatistiquesGolf.getProfil();
            username.Text = LocalUser.Username;
            index.Text = LocalUser.Index.ToString();
            golfref.Text = LocalUser.GolfRef;
            photo.Source = "user" + LocalUser.Photo + ".png";
        }

        /**
         * When the name is completed by the user, update the profile in the database
         */
        private void OnNameCompleted(object sender, EventArgs e)
        {
            Entry entry = (Entry) sender;
            if (entry.Text != "")
            {
                LocalUser.Username = entry.Text;
                DBconnection.Update(LocalUser);
            } else
            {
                entry.Text = LocalUser.Username;
            }
        }

        /**
         * When the reference golf is completed by the user, update the profile in the database
         */
        private void OnGolfRefCompleted(object sender, EventArgs e)
        {
            Entry entry = (Entry)sender;
            if (entry.Text != "")
            {
                LocalUser.GolfRef = entry.Text;
                DBconnection.Update(LocalUser);
            }
            else
            {
                entry.Text = LocalUser.GolfRef;
            }
        }

        /**
         * When the index is completed by the user, update the profile in the database and the average distance of each club
         */
        private async void OnIndexCompleted(object sender, EventArgs e)
        {
            LocalUser.Index = double.Parse(((Entry)sender).Text);
            DBconnection.Update(LocalUser);
            List<Club> clubs = await GestionGolfs.getListClubsAsync(null);
            List<Club> xmlClubs = GolfXMLReader.getListClubFromXMLFiles();
            foreach (Club club in clubs)
            {
                club.DistanceMoyenne = xmlClubs.Find(c => c.Name.Equals(club.Name)).DistanceMoyenne;
            }
            DBconnection.UpdateAll(clubs);
        }

        async private void OnArrowClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        async private void OnPhotoClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ChooseAvatar());
        }
    }
}