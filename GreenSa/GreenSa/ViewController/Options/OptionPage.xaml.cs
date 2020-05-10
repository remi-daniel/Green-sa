using GreenSa.Models.GolfModel;
using GreenSa.Models.Profiles;
using GreenSa.Models.Tools;
using GreenSa.Persistence;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GreenSa.ViewController.Option
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OptionPage : ContentPage
    {

        private SQLiteConnection DBConnection;
        private Profil profil;

        public OptionPage()
        {
            InitializeComponent();
            DBConnection = DependencyService.Get<ISQLiteDb>().GetConnection();
            profil = StatistiquesGolf.getProfil();
            if (profil != null)
            {
                this.OnOff.IsToggled = profil.SaveStats;
            } else
            {
                this.OnOff.IsToggled = false;
            }
            UpdateSwitchLabel();
        }

        /** 
         * This method is called when the button to delete all stats is clicked
         */
        private async void dropStats(object sender, EventArgs e)
        {
           bool delete = await DisplayAlert("Avertissement", "Attention, supprimer les statistiques est une manoeuvre irréversible !", "Continuer", "Annuler");
            if (!delete)
                return;
            try
            {
                SQLite.SQLiteConnection connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                connection.DropTable<MyPosition>();
                connection.DropTable<Shot>();
                connection.DropTable<ScoreHole>();
                connection.DropTable<ScorePartie>();

                connection.CreateTable<MyPosition>();
                connection.CreateTable<Shot>();
                connection.CreateTable<ScoreHole>();
                connection.CreateTable<ScorePartie>();
            }
            catch (Exception)
            {
            }
        }

        /**
         * This method is called when the switch is toggled
         * Updates the On/Off label and update the profile in the database 
         */
        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            this.UpdateSwitchLabel();
            profil.SaveStats = this.OnOff.IsToggled;
            if (profil != null)
            {
                if (DBConnection == null)
                {
                    DBConnection = DependencyService.Get<ISQLiteDb>().GetConnection();
                }
                DBConnection.Update(profil);
            }
        }

        private void UpdateSwitchLabel()
        {
            this.OnOffLabel.Text = this.OnOff.IsToggled ? "Oui" : "Non";
        }
    }
}