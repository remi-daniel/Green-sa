using GreenSa.Models.GolfModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xamarin.Forms;

namespace GreenSa.ViewController.Profile.MyGames
{
    public partial class HistoryPage : ContentPage
    {
        private ScorePartie Sp;
        private ScoreHole Sh;
        private int HoleNumber;//The position of this hole in the golf course
        private ObservableCollection<Tuple<Shot, IEnumerable<Club>>> item;
        bool LateralNavigation;
        List<Club> clubs;

        public HistoryPage(ScorePartie sp, int holeNumber, bool lateralNavigation)
        {
            InitializeComponent();
            HoleNumber = holeNumber;
            Sp = sp;
            Sh = sp.scoreHoles[holeNumber];
            LateralNavigation = lateralNavigation;

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
            previous.Margin = new Thickness(MainPage.responsiveDesign(5), MainPage.responsiveDesign(5), 0, MainPage.responsiveDesign(5));
            next.WidthRequest = previous.Width;
            add.Margin = new Thickness(MainPage.responsiveDesign(5), MainPage.responsiveDesign(15), MainPage.responsiveDesign(5), 0);

            
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if (HoleNumber <= 0)
            {
                previous.IsVisible = false;
            }
            else if (HoleNumber >= Sp.scoreHoles.Count - 1)
            {
                next.IsVisible = false;
            }

            //Ensures that previous HistoryPage is cleared from the navigation stack when going from one hole to the next
            //(so that the back button can take the user back directly to the DetailsPartiePage)
            if (LateralNavigation)
            {
                Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
            }

            clubs = await GestionGolfs.getListClubsAsync(null);
            item = new ObservableCollection<Tuple<Shot, IEnumerable<Club>>>(Sh.Shots.Select(s => new Tuple<Shot, IEnumerable<Club>>(s, clubs)));
            ListShotPartie.ItemsSource = item;
            numero.Text = "Trou n°" + (HoleNumber+1) + " :";
            hole_finished.Text = "TROU N°" + (HoleNumber+1);
            par.Text = Sh.Hole.Par.ToString();

            updateScoreText();
        }

        /**
         * Updates the score label text
         */
        private void updateScoreText()
        {
            int penalities = 0;
            int nbShots = 0;
            foreach (Shot shot in Sh.Shots)
            {
                penalities += shot.PenalityCount;
                nbShots++;
            }
            Sh.Penality = penalities;
            Sh.Score = nbShots;
            int totalScore = (Sh.Penality + Sh.Score - Sh.Hole.Par);//Update score in SHOT??
            if (totalScore >= 0)
            {
                score.Text = "+" + totalScore;
            }
            else
            {
                score.Text = totalScore.ToString();
            }

        }

        /**
         * This method is called when clicking on the button to see the results of the next hole
         */
        private async void nextHoleClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HistoryPage(Sp, (HoleNumber + 1), true));
        }

        /**
         * This method is called when clicking on the button to see the results of the previous hole
         */
        private async void previousHoleClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HistoryPage(Sp, (HoleNumber - 1), true));
        }

        /**
         * This method is called when a penality count is chosen
         */
        private void OnPenalityCompleted(object sender, EventArgs e)
        {
            //gets the shot associated to the picker
            var picker = sender as Picker;
            picker.TextColor = Color.White;
            var tgr = picker.GestureRecognizers[0] as TapGestureRecognizer;
            Shot shot = (Shot)tgr.CommandParameter;
            int penalityCount = 0;
            if (picker.SelectedItem != null)
            {
                penalityCount = (int)picker.SelectedItem;
            }
            //updates penality count of the shot and the corresponding label text
            if (shot != null)
            {
                shot.SetPenalityCount(penalityCount);
                this.updateScoreText();
            }
        }

        /**
         * This method is called when clicking on the cross to delete the associated shot
         */
        private async void OnShotDeletedClicked(object sender, EventArgs e)
        {
            //gets the shot associated to the image
            var image = sender as Image;
            var tgr = image.GestureRecognizers[0] as TapGestureRecognizer;
            Shot shot = (Shot)tgr.CommandParameter;
            var confirm = true;
            if (!shot.Club.IsPutter())//if not a putter shot then ask a delete confirmation
            {
                confirm = await this.DisplayAlert("Suppression", "Voulez vous vraiment supprimer ce coup ?", "Oui", "Non");
            }
            if (confirm)//then remove the shot from list view source and from the game shots list
            {
                item.Remove(item.ToList().Find(tuple => tuple.Item1.Equals(shot)));

                Sh.Shots.Remove(shot);
                updateScoreText();
            }
        }

        /**
         * This method is called when clicking on a new club for one shot
         */
        private void OnClubChanged(object sender, EventArgs e)
        {
            //gets the shot associated to the image
            var picker = sender as Picker;
            var tgr = picker.GestureRecognizers[0] as TapGestureRecognizer;
            Shot shot = null;
            try
            {
                shot = (Shot)tgr.CommandParameter;
                if (shot != null)
                {
                    shot.UpdateShotType();
                }
            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine("Error : " + ex.StackTrace);
            }
        }

        /**
         * This method is called when the button to add a putter shot is clicked
         */
        private void AddShotButtonClicked(object sender, EventArgs e)
        {
            Shot s = new Shot(Club.PUTTER, null, null, null, DateTime.Now);
            Sh.Shots.Add(s);
            
            item.Add(new Tuple<Shot, IEnumerable<Club>>(s, clubs));
            updateScoreText();

        }

        protected override bool OnBackButtonPressed()
        {
            return base.OnBackButtonPressed();
        }

        /**
         * Syncs the changes made to the database
         */
        private async void saveChanges()
        {
            
        }
    }
}