using GreenSa.Models.GolfModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Xamarin.Forms;

namespace GreenSa.ViewController.Profile.MyGames
{
    public partial class HistoryPage : ContentPage
    {
        ScoreHole sh;
        ObservableCollection<Tuple<Shot, IEnumerable<Club>>> item;

        public HistoryPage(ScoreHole sh)
        {
            InitializeComponent();
            this.sh = sh;

            hole_finished.Margin = new Thickness(-8, MainPage.responsiveDesign(19), 0, MainPage.responsiveDesign(20));
            ListShotPartie.Margin = new Thickness(MainPage.responsiveDesign(10), MainPage.responsiveDesign(34), MainPage.responsiveDesign(10), MainPage.responsiveDesign(58));
            club.Margin = new Thickness(MainPage.responsiveDesign(30), MainPage.responsiveDesign(5), 0, 0);
            distance.Margin = new Thickness(MainPage.responsiveDesign(140), MainPage.responsiveDesign(5), 0, 0);
            pen.Margin = new Thickness(MainPage.responsiveDesign(242), MainPage.responsiveDesign(5), 0, 0);
            numero.Margin = new Thickness(MainPage.responsiveDesign(25), MainPage.responsiveDesign(25), 0, 0);
            par.Margin = new Thickness(MainPage.responsiveDesign(205), MainPage.responsiveDesign(25), 0, 0);
            score.Margin = new Thickness(MainPage.responsiveDesign(265), MainPage.responsiveDesign(25), 0, 0);
            parlegende.Margin = new Thickness(MainPage.responsiveDesign(200), MainPage.responsiveDesign(5), 0, 0);
            scorelegende.Margin = new Thickness(MainPage.responsiveDesign(260), MainPage.responsiveDesign(5), 0, 0);
            next.BackgroundColor = Color.FromHex("39B54A");
            next.Margin = new Thickness(0, MainPage.responsiveDesign(5), MainPage.responsiveDesign(5), MainPage.responsiveDesign(5));
            stop.Margin = new Thickness(MainPage.responsiveDesign(5), MainPage.responsiveDesign(5), 0, MainPage.responsiveDesign(5));
            next.WidthRequest = stop.Width;
            add.Margin = new Thickness(MainPage.responsiveDesign(5), MainPage.responsiveDesign(15), MainPage.responsiveDesign(5), 0);
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            //List<Club> clubs = buildClubList();
            List <Club> clubs = await GestionGolfs.getListClubsAsync(null);
            item = new ObservableCollection<Tuple<Shot, IEnumerable<Club>>>(sh.Shots.Select(s => new Tuple<Shot, IEnumerable<Club>>(s, clubs)));
            ListShotPartie.ItemsSource = item;
            numero.Text = "LAALLALALALA";
            /*numero.Text = "Trou n°" + partie.getCurrentHoleNumero() + " :";
            hole_finished.Text = "TROU N°" + partie.getCurrentHoleNumero() + " TERMINE !";
            par.Text = partie.getNextHole().Par.ToString();
            updateScoreText();
            if (!partie.hasNextHole())
            {
                next.Text = "Fin de partie";
            }*/
        }

        /**
         * Generates a list of all clubs used during the hole.
         * Because the club database could be modified later and become incompatible, it is
         * safer to directly import them from the game data.
         */
        private List<Club> buildClubList()
        {
            //Move to game level/useless
            List<Club> clubs = new List<Club>();
            foreach(Shot s in sh.Shots){
                if(!clubs.Contains(s.Club)) clubs.Add(s.Club);
            }
            return clubs;
        }




        //---------------------------------------






        /**
         * Updates the score label text
         */
        private void updateScoreText()
        {
            
        }

        /**
         * This method is called when clicking on the button to valid the current hole and go through the the next one
         */
        private async void validButtonClicked(object sender, EventArgs e)
        {
            
        }

        /**
         * This method is called when clicking the button to end the game
         * The current holes is saved before ending the game
         */
        private async void stopPartieClicked(object sender, EventArgs e)
        {
            

        }

        /**
         * This method is called when a penality count is chosen
         */
        private void OnPenalityCompleted(object sender, EventArgs e)
        {
            
        }

        /**
         * This method is called when clicking on the cross to delete the associated shot
         */
        private async void OnShotDeletedClicked(object sender, EventArgs e)
        {
            
        }

        /**
         * This method is called when clicking on a new club for one shot
         */
        private void OnClubChanged(object sender, EventArgs e)
        {
            
        }

        /**
         * This method is called when the button to add a putter shot is clicked
         */
        private void AddShotButtonClicked(object sender, EventArgs e)
        {
           
        }


        protected override bool OnBackButtonPressed()
        {
            
            return base.OnBackButtonPressed();
        }
    }
}