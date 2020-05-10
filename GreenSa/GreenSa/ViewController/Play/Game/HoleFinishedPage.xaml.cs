using GreenSa.Models.GolfModel;
using GreenSa.Models.Profiles;
using GreenSa.Models.Tools;
using GreenSa.Models.ViewElements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GreenSa.ViewController.Play.Game
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HoleFinishedPage : ContentPage
    {
        private Partie partie;
        ObservableCollection<Tuple<Shot, IEnumerable<Club>>> item;

        public HoleFinishedPage(Partie partie)
        {
            InitializeComponent();
            this.partie = partie;
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

        /**
         * This method is executed when the page is loaded
         * */
        protected override void OnAppearing()
        {
            base.OnAppearing();
            item = new ObservableCollection<Tuple<Shot, IEnumerable<Club>>>(partie.Shots.Select(s => new Tuple<Shot, IEnumerable<Club>>(s, partie.Clubs)));
            ListShotPartie.ItemsSource = item;
            numero.Text = "Trou n°" + partie.getCurrentHoleNumero() + " :";
            hole_finished.Text = "TROU N°" + partie.getCurrentHoleNumero() + " TERMINE !";
            par.Text = partie.getNextHole().Par.ToString();
            updateScoreText();
            if (!partie.hasNextHole())
            {
                next.Text = "Fin de partie";
            }
        }

        /**
         * Updates the score label text
         */
        private void updateScoreText()
        {
            if (partie.getCurrentScore() >= 0)
            {
                score.Text = "+"+partie.getCurrentScore().ToString();
            }
            else
            {
                score.Text = partie.getCurrentScore().ToString();
            }
        }

        /**
         * This method is called when clicking on the button to valid the current hole and go through the the next one
         */
        private async void validButtonClicked(object sender, EventArgs e)
        {
            if (partie.Shots.Count == 0)//checks if at least one shot was performed
            {
                await DisplayAlert("0 coups rentrés", "Impossible de valider avec aucun shot", "OK");
                return;
            }
            //the user has to confirm his click
            var confirm = await this.DisplayAlert("Trou sivant", "Passer au trou suivant ?", "Oui", "Non");
            if (confirm)
            {
                MessagingCenter.Send<HoleFinishedPage, int>(this, "ReallyFinit", 1);//sends a message : the hole is finished
                next.IsEnabled = false;
                Profil profil = StatistiquesGolf.getProfil();
                partie.holeFinished(profil.SaveStats);
                await Navigation.PopModalAsync();
                next.IsEnabled = true;
            }
        }

        /**
         * This method is called when clicking the button to end the game
         * The current holes is saved before ending the game
         */
        private async void stopPartieClicked(object sender, EventArgs e)
        {
            if (partie.Shots.Count == 0)//checks if the current hole was played
            {
                await this.DisplayAlert("Erreur", "Vous devez avoir joué ce trou pour arrêter la partie ici", "Ok");
            } else
            {
                //the user has to confirm his click
                var confirm = await this.DisplayAlert("Arrêter la partie", "Voulez vous vraiment arrêter la partie après ce trou ?", "Oui", "Non");
                if (confirm)
                {
                    MessagingCenter.Send<HoleFinishedPage, int>(this, "ReallyFinit", 2);//sends a message : the game is finished
                    Profil profil = StatistiquesGolf.getProfil();
                    partie.holeFinished(profil.SaveStats);
                    await Navigation.PopModalAsync();
                }
            }
            
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
                penalityCount = (int) picker.SelectedItem;
            }
            //updates penality count of thhe shot and the corresponding label text
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
            if(!shot.Club.IsPutter())//if not a putter shot then ask a delete confirmation
            {
                confirm = await this.DisplayAlert("Suppression", "Voulez vous vraiment supprimer ce coup ?", "Oui", "Non");
            }
            if (confirm)//then remove the shot from list view source and from the game shots list
            {
                item.Remove(item.ToList().Find(tuple => tuple.Item1.Equals(shot)));

                partie.Shots.Remove(shot);
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
            partie.Shots.Add(s);
            item.Add(new Tuple<Shot, IEnumerable<Club>>(s, partie.Clubs));
            updateScoreText();
        }


        protected override bool OnBackButtonPressed()
        {
            MessagingCenter.Send<HoleFinishedPage, int>(this, "ReallyFinit", 0);
            return base.OnBackButtonPressed();
        }

    }
}