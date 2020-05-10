using GreenSa.Models;
using GreenSa.Models.GolfModel;
using GreenSa.Models.Tools;
using GreenSa.ViewController.Profile.MyGames;
using GreenSa.ViewController.Profile.Statistiques.StatistiquesGolfCourse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GreenSa.ViewController.Play
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GolfSelectionPage : ContentPage
    {
        Partie p;
        private GolfCourseStatPage golfCourseStatPage;

        public GolfSelectionPage(Partie partie)
        {
            InitializeComponent();
            p = partie;
            golfCourseStatPage = null;
        }

        public GolfSelectionPage()
        {
            InitializeComponent();
            p = null;
            golfCourseStatPage = null;
        }

        /**
         * This method is executed when the page is loaded
         */
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Func<GolfCourse, bool> f = (c => true);
            List<GolfCourse> res = await GestionGolfs.getListGolfsAsync(f);
            ListGolfCourse.ItemsSource = res;
        }


        /*
         * Called when a golf course is picked by the user
         * This method ask the user whether he wants to load a not ended game or start a new one. Then the game is start
         */
         private async void onGolfSelection(object sender, EventArgs e)
         {
            var g = ListGolfCourse.SelectedItem as GolfCourse;
            if (p == null)//if not in game part (if in stat part)
            {
                if (this.golfCourseStatPage == null)
                {
                    this.golfCourseStatPage = new GolfCourseStatPage(g);
                } else
                {
                    this.golfCourseStatPage.changeGolfCourse(g);
                }
                await Navigation.PushModalAsync(this.golfCourseStatPage, true);
            } else
            {
                p.GolfCourse = g;
                Func<Club, bool> f = (c => true);
                List<Club> clubselected = await GestionGolfs.getListClubsAsync(f);
                clubselected.RemoveAll(c => c.selected == false);
                //Checks if the user has at least one club on his bag
                if (clubselected.Count == 0)
                {
                    await this.DisplayAlert("Erreur", "Vous n'avez aucun club dans votre sac. Veuillez en choisir au moins un dans le page 'Profil'", "ok");
                } else
                {
                    p.Clubs = clubselected;
                    List<ScorePartie> scoreParties = await StatistiquesGolf.getNotFinishedGames(g);
                    if (scoreParties.Count > 0)
                    {
                        var newGame = await this.DisplayAlert("Lancement d'une partie", "Voulez vous lancer une nouvelle partie ou charger une existante ?", "Nouvelle partie", "Charger une existante");
                        if (newGame)
                        {
                            await Navigation.PushAsync(new Game.MainGamePage(p), false);
                        }
                        else//if load a not ended game then show the list of not ended games
                        {
                            await Navigation.PushAsync(new ViewPartieListPage(2, scoreParties, p), false);
                        }
                    } else//if no not ended game then start a new one directly
                    {
                        await Navigation.PushAsync(new Game.MainGamePage(p), false);
                    }
                } 
            }
         }

    }
}