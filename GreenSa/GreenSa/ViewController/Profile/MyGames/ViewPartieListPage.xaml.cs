using GreenSa.Models.GolfModel;
using GreenSa.ViewController.Profile.Statistiques.StatistiquesGolfCourse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GreenSa.ViewController.Profile.MyGames
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewPartieListPage : ContentPage
    {
        private int state;
        private PartieStatPage partieStatPage;
        private List<ScorePartie> scoreParties;
        private Partie partie;

        public ViewPartieListPage()
        {
            InitializeComponent();
            this.state = 0;
            this.partieStatPage = null;
            this.scoreParties = null;
            this.partie = null;
        }

        public ViewPartieListPage(int state)
        {
            InitializeComponent();
            this.state = state;
            this.partieStatPage = null;
            this.scoreParties = null;
            this.partie = null;
        }

        public ViewPartieListPage(int state, List<ScorePartie> scoreParties, Partie partie)
        {
            InitializeComponent();
            this.state = state;
            this.partieStatPage = null;
            this.scoreParties = scoreParties;
            this.partie = partie;
        }

        /**
         * This method is executed when the page is loaded
         * */
        async protected override void OnAppearing()
        {
            try {
                if (scoreParties == null)
                {
                    //Sort in descending order of games date
                    scoreParties = (await StatistiquesGolf.getScoreParties()).OrderByDescending(d => d.DateDebut).ToList();
                }
                listPartie.ItemsSource = scoreParties;
            } catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error : " + e.StackTrace);
            }
            
        }

        /** 
         * This method is called when an item is tapped in the list view of games
         */
        private async void listPartie_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ScorePartie sp = (ScorePartie)listPartie.SelectedItem;
            if (state == 0)//if in games panel in profil page
            {
                await Navigation.PushModalAsync(new DetailsPartiePage((ScorePartie)listPartie.SelectedItem));
            } else if (state == 1)//if in stats panel in profil page
            {
                if (this.partieStatPage == null)
                {
                    this.partieStatPage = new PartieStatPage(sp, sp.GolfName);
                }
                else
                {
                    this.partieStatPage.changePartie(sp, sp.GolfName);
                }
                await Navigation.PushModalAsync(this.partieStatPage);
            } else if (state == 2)//when loading an existing and not finished game
            {
                partie.ScoreOfThisPartie = sp;
                foreach (ScoreHole sh in sp.scoreHoles)
                {
                    partie.nextHole();//skip holes that was already done
                }
                //partie.holeFinishedCount++;//needs to increment one last time because this attibute is initialized to -1 to start at 0 after first main game page appearing
                await Navigation.PushAsync(new Play.Game.MainGamePage(partie), false);
            }
        }
    }
}