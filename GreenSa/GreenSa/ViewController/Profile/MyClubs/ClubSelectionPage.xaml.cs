using GreenSa.Models.GolfModel;
using GreenSa.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using GreenSa.Persistence;

namespace GreenSa.ViewController.Profile.MyClubs
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClubSelectionPage : ContentPage
    {
        public ClubSelectionPage()
        {
            InitializeComponent();
        }

        /**
         * This method is executed when the page is loaded
         * */
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Func<Club, bool> f = (c => true);
            listviewclub.ItemsSource = await GestionGolfs.getListClubsAsync(f);
        }

        /**
         * This method is called when the button to valid the club list is clicked
         */
        private async void onValidClubSelection(object sender, EventArgs e)
        {
            Btn.IsEnabled = false;
            Btn.Text = "En cours...";
            Btn.TextColor = Color.White;
            SQLite.SQLiteAsyncConnection connection = DependencyService.Get<ISQLiteDb>().GetConnectionAsync();
            await connection.CreateTableAsync<Club>();
            List<Club> clubselected = new List<Club>();
            bool atLeastOneSelected = false;
            foreach (Club c in listviewclub.ItemsSource) {//updates each club 'selected' attribute in the database
                if(c.selected){
                    atLeastOneSelected = true;
                }
                await SQLiteNetExtensionsAsync.Extensions.WriteOperations.UpdateWithChildrenAsync(connection, c);
            }
            if (!atLeastOneSelected)//checks if at least one club is selected
            {
                await DisplayAlert("Aucun club selectionné", "Vous devez selectionner au moins un club", "ok");
            } else
            {
                await Navigation.PopAsync();
            }
            Btn.IsEnabled = true;
            Btn.Text = "Valider";
        }
    }
}