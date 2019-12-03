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


namespace GreenSa.ViewController.Profile.Options
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChooseAvatar : ContentPage
    {
        private SQLiteConnection DBconnection;
        private Profil LocalUser;
        private int lastPhotoIndex;
        private Color col;
        private ImageButton[] photos;

        public ChooseAvatar()
        {
            InitializeComponent();
            col = Color.Gray;
            title.FontSize = 30;
            ok.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));
            ok.Margin = MainPage.responsiveDesign(-15);
            ok.HeightRequest = MainPage.responsiveDesign(80);
            ok.WidthRequest = MainPage.responsiveDesign(80);
            this.photos = new ImageButton[6];
            this.photos[0] = photo1;
            this.photos[1] = photo2;
            this.photos[2] = photo3;
            this.photos[3] = photo4;
            this.photos[4] = photo5;
            this.photos[5] = photo6;

            this.InitBDD();
            LocalUser = StatistiquesGolf.getProfil();

            title.Margin = new Thickness(80, 5, 80, 10);
            lastPhotoIndex = LocalUser.Photo - 1;
            this.photos[lastPhotoIndex].BorderWidth = 3;
            this.photos[lastPhotoIndex].BorderColor = col;
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
         * This method is called when an image is clicked
         * Unselects the last image to select the clicked one and update the profile in the database in consequence
         */
        private void OnPhotoSelected(object sender, EventArgs e)
        {
            ImageButton image = (ImageButton) sender;
            this.photos[lastPhotoIndex].BorderWidth = 0;
            this.photos[lastPhotoIndex].BorderColor = Color.Transparent;
            int index = this.photos.ToList().IndexOf(image);
            LocalUser.Photo = index + 1;
            DBconnection.Update(LocalUser);
            lastPhotoIndex = index;
            this.photos[lastPhotoIndex].BorderWidth = 3;
            this.photos[lastPhotoIndex].BorderColor = col;
        }

        async private void OnArrowClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}