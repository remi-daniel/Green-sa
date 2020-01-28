using GreenSa.Models;
using GreenSa.Models.GolfModel;
using GreenSa.Models.Tools;
using GreenSa.Models.Tools.GPS_Maps;
using GreenSa.Models.Tools.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using GreenSa.Models.Profiles;

namespace GreenSa.ViewController.Play.Game
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainGamePage : ContentPage
    {

        private int state; //0 when ready to shot and 1 when ready to localize
        private Partie partie;
        private int holFini;
        private double dUserTarget;
        private double dUserTargetTemp;//used to chose a club when the target moved more than 10 meters

        public MainGamePage(Partie partie)
        {
            InitializeComponent();

            forceVent.Margin = new Thickness(35, 10, 0, 0);
            windImg.Margin = new Thickness(35, 5, 0, 0);
            windImg.HeightRequest = MainPage.responsiveDesign(25);
            ball.HeightRequest = MainPage.responsiveDesign(91);
            ball.Margin = MainPage.responsiveDesign(10);
            radar.HeightRequest = MainPage.responsiveDesign(92);
            radar.Margin = MainPage.responsiveDesign(10);
            load.HeightRequest = MainPage.responsiveDesign(92);
            load.Margin = MainPage.responsiveDesign(10);
            backToRadar.HeightRequest = MainPage.responsiveDesign(30);
            backToRadar.Margin = new Thickness(MainPage.responsiveDesign(215), 0, 0, MainPage.responsiveDesign(10));
            backToBall.HeightRequest = MainPage.responsiveDesign(30);
            backToBall.Margin = new Thickness(MainPage.responsiveDesign(215), 0, 0, MainPage.responsiveDesign(10));
            clubs.HeightRequest = MainPage.responsiveDesign(45);
            clubs.Margin = new Thickness(MainPage.responsiveDesign(15), 0, 0, 0);
            ball_in.HeightRequest = MainPage.responsiveDesign(45);
            ball_in.Margin = new Thickness(0, 0, MainPage.responsiveDesign(14), MainPage.responsiveDesign(7));
            numclub.TextColor = Color.FromHex("009245");
            numclub.Margin = new Thickness(MainPage.responsiveDesign(34), 0, 0, MainPage.responsiveDesign(17));
            numclub.FontSize = MainPage.responsiveDesign(20);

            numcoup.FontSize = MainPage.responsiveDesign(40);
            numcoup.Margin = MainPage.responsiveDesign(-5);
            parTrou.FontSize = MainPage.responsiveDesign(15);
            parTrou.Margin = MainPage.responsiveDesign(38);
            distTrou.FontSize = MainPage.responsiveDesign(13);
            distTrou.Margin = new Thickness(MainPage.responsiveDesign(7), 0, 0, 0);
            distTarget.FontSize = MainPage.responsiveDesign(10);
            distTarget.Margin = new Thickness(0, MainPage.responsiveDesign(-4), 0, 0);
            distGrid.Margin = new Thickness(0, MainPage.responsiveDesign(3), MainPage.responsiveDesign(15), 0);
            forceVent.FontSize = MainPage.responsiveDesign(12);
            forceVent.Margin = new Thickness(MainPage.responsiveDesign(28), MainPage.responsiveDesign(23), 0, 0);
            windImg.Margin = new Thickness(MainPage.responsiveDesign(42), MainPage.responsiveDesign(8), 0, 0);
            windImg.HeightRequest = MainPage.responsiveDesign(15);
            numclub.IsEnabled = false;

            clubselection.BackgroundColor = Color.FromRgba(0, 0, 0, 0.6);
            clubselection.HeightRequest = MainPage.responsiveDesign(300);
            clubselection.WidthRequest = MainPage.responsiveDesign(300);
            clubselection.CornerRadius = MainPage.responsiveDesign(25);
            clubselection.Margin = new Thickness(MainPage.responsiveDesign(10), 0, 0, MainPage.responsiveDesign(135));
            ListClubsPartie.BackgroundColor = Color.Transparent;
            ListClubsPartie.HeightRequest = MainPage.responsiveDesign(165);
            ListClubsPartie.WidthRequest = MainPage.responsiveDesign(300);
            ListClubsPartie.Margin = new Thickness(MainPage.responsiveDesign(10), MainPage.responsiveDesign(167), MainPage.responsiveDesign(42), MainPage.responsiveDesign(135));

            ListHole.BackgroundColor = Color.Transparent;
            ListHole.HeightRequest = MainPage.responsiveDesign(300);
            ListHole.WidthRequest = MainPage.responsiveDesign(100);
            ListHole.Margin = new Thickness(MainPage.responsiveDesign(225), MainPage.responsiveDesign(74), MainPage.responsiveDesign(10), MainPage.responsiveDesign(120));
            ListHole.RowHeight = 40;

            cardBackground.HeightRequest = MainPage.responsiveDesign(50);
            cardBackground.WidthRequest = MainPage.responsiveDesign(50);
            cardBackground.CornerRadius = 100;
            cardBackground.BackgroundColor = Color.FromRgba(0, 0, 0, 0.5);
            score.HeightRequest = MainPage.responsiveDesign(100);
            score.WidthRequest = MainPage.responsiveDesign(100);

            GestionGolfs.calculAverageAsync(partie.Clubs);//Load average club distances

            dUserTargetTemp = -1;
            hideCard();
            hideClubs();
            showBall();
            backToRadar.IsVisible = false;
            backToBall.IsVisible = false;

            holFini = 1;
            this.partie = partie;
            map.MoveToRegion(
            MapSpan.FromCenterAndRadius(
                    new Position(48.1116654, -1.6843768), Distance.FromMiles(30)));

            BindingContext = partie;
            partie.CurrentClub = partie.Clubs.First();
            LoadClubIcon(partie.CurrentClub);
            //message which come from the markerListenerDrag,
            //when the target pin is moved => update the display of the distance
            MessagingCenter.Subscribe<CustomPin>(this, CustomPin.UPDATEDMESSAGE, (sender) => {
                updateDistance();
            });
            MessagingCenter.Subscribe<System.Object>(this, CustomPin.UPDATEDMESSAGE_CIRCLE, (sender) => {
                updateDistance();
            });
            //this message details the state of the game 0 if hole isn't finished, 1 otherwise and 2 if the game is finished
            MessagingCenter.Subscribe<HoleFinishedPage, int>(this, "ReallyFinit", (sender, val) => {
                holFini = val;
            });
            updateScore();
            loadCard();
        }

        /**
         * This method is executed when the page is loaded
         * */
        async protected override void OnAppearing()
        {
            base.OnAppearing();
            if (holFini == 0)
                return;

            //if there is still a hole to play
            if (partie.nextHole() && holFini != 2)//holFini == 2 means the user wants to stop the game before the end
            {
                updateScore();
                loadCard();
                Hole nextHole = partie.getNextHole();
                map.setHolePosition(nextHole);
                numcoup.Text = partie.getCurrentHoleNumero() + "";                              
                parTrou.Text = "PAR " + partie.getNextHole().Par.ToString();
                MyPosition position = new MyPosition(0, 0);
                //make sure that the GPS is avaible
                try
                {
                    position = await localize();
                    map.setUserPosition(position, partie.Shots.Count);
                    partie.CurrentClub = partie.CurrentClub;//just to update the circle
                }
                catch (Exception e)
                {
                    await DisplayAlert("Gps non disponible", "La localisation GPS n'est pas disponible, assurez-vous de l'avoir activé.", "OK");
                    await Navigation.PopToRootAsync();//come back to root to avoid any problems in the game flow
                }

                updateDistance(true);

                //manage the wind icon
                try
                {
                    WindService service = new WindService();
                    await Task.Run(async () =>
                    {
                        WindInfo windInfo = await service.getCurrentWindInfo(map.getUserPosition());
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await UpdateWindServiceUI(windInfo);
                        });

                    });
                }
                catch (Exception e)
                {
                    await DisplayAlert("Vent non disponible", "L'information concernant le vent n'est pas disponible", "OK");
                }


            } else//user wants to stop the game
            {
                await Navigation.PushAsync(new GameFinishedPage(partie));
                return;
            }
            holFini = 0;
        }


        /**
         * Localizes the user with his GPS
         * return a MyPosition class wrapping the longitude and latitude of the user current position
         */
        public async Task<MyPosition> localize()
        {
            backToRadar.IsVisible = false;
            showLoad();
            MyPosition position = await GpsService.getCurrentPosition();
            showBall();
            backToRadar.IsVisible = true;
            return position;
        }

        /**
         * Updates the wind icon using the given information
         * windInfo : information about the wind
         */
        public async Task UpdateWindServiceUI(WindInfo windInfo)
        {
            try
            {
                forceVent.Text = windInfo.strength + " km/h";
                await windImg.RotateTo(90 + windInfo.direction);
            }
            catch (Exception e)
            {

            }
        }

        /**
         * Updates the distances on the top right hand corner of the screen
         * OnAppearing : true if the method is called in the OnAppearing method, false otherwise
         */
        private void updateDistance(bool OnAppearing = false)
        {
            partie.updateUICircle();

            dUserTarget = map.getDistanceUserTarget();
            distTrou.Text = string.Format("{0:0.0}", map.getDistanceUserHole()) + "m";
            var distUsertarget = map.getDistanceUserTarget();
            var distTargetHole = map.getDistanceTargetHole();
            distTarget.Text = string.Format("{0:0.0}", distUsertarget) + " + " + string.Format("{0:0.0}", distTargetHole) + "m";
            if (dUserTargetTemp == -1)
            {
                dUserTargetTemp = dUserTarget;
            }

            //if the target has moved more than 5 meters or if the page was just refreshed
            if (Math.Abs(dUserTarget - dUserTargetTemp) > 5 || OnAppearing)
            {
                dUserTargetTemp = dUserTarget;
                Club c = GestionGolfs.giveMeTheBestClubForThatDistance(partie.Clubs, dUserTarget);
                setCurrentClub(c);
            }
        }

        /**
         * Shows the list of the available clubs
         */
        private void showClubs()
        {
            clubselection.IsVisible = true;
            ListClubsPartie.ItemsSource = partie.Clubs;
            ListClubsPartie.IsVisible = true;
            ListClubsPartie.SelectedItem = partie.CurrentClub;
        }

        /**
         * Hides the list of the available clubs
         */
        private void hideClubs()
        {
            clubselection.IsVisible = false;
            ListClubsPartie.IsVisible = false;
        }

        /**
         * Computes the current score of the user and update the corresponding label
         */
        private void updateScore()
        {
            int coups = 0;
            foreach(ScoreHole sc in partie.ScoreOfThisPartie.scoreHoles) {
                coups += sc.Score;
            }
            if (coups > 0) {
                score.Text = "+" + coups.ToString();
            } else {
                score.Text = coups.ToString();
            }
            if (coups >= 10) {
                score.FontSize = 26;
            }
            else
            {
                score.FontSize = 28;
            }
        }

        /**
         * Transforms each done holes score in a DisplayScoreCard class used to bind the data in the game card (ListView)
         */
        private void loadCard()
        {
            List<ScoreHole> scoreHoles = partie.ScoreOfThisPartie.scoreHoles;
            List<DisplayScoreCard> ScoreCard = new List<DisplayScoreCard>();
            //manage done holes scores
            for (int i = 0; i < scoreHoles.Count; i++)
            {
                DisplayScoreCard ds = new DisplayScoreCard(i+1, scoreHoles.ElementAt<ScoreHole>(i));
                ScoreCard.Add(ds);

            }
            //manage not done holes scores
            for(int i=scoreHoles.Count; i < partie.GolfCourse.Holes.Count; i++)
            {
                DisplayScoreCard ds = new DisplayScoreCard(i+1, partie.GolfCourse.Holes.ElementAt<Hole>(i).Par);
                ScoreCard.Add(ds);
            }
            ListHole.ItemsSource = ScoreCard;
        }

        /**
         * Shows the game card
         */
        private void showCard()
        {
            cardBackground.Margin = new Thickness(0, 0, MainPage.responsiveDesign(135), MainPage.responsiveDesign(305));
            score.Margin = new Thickness(0, 0, MainPage.responsiveDesign(110), MainPage.responsiveDesign(280));
            ListHole.IsVisible = true;
        }

        /**
         * Hides the game card to only display the current score
         */
        private void hideCard()
        {
            ListHole.IsVisible = false;
            cardBackground.Margin = new Thickness(0, 0, MainPage.responsiveDesign(10), MainPage.responsiveDesign(305));
            score.Margin = new Thickness(0, 0, MainPage.responsiveDesign(-15), MainPage.responsiveDesign(280));
        }

        /**
         * Sets the current club with the given one
         * club : the new club
         */
        private void setCurrentClub(Club club)
        {
            partie.setCurrentClub(club);
            LoadClubIcon(club);
        }

        /**
         * Load the icon corresponding to the given club
         * club : the club
         */
        private void LoadClubIcon(Club club)
        {
            //clubs is an image describing the club type and numClub a label to describe its numero
            switch (club.Name)
            {
                case "Bois 3":
                    clubs.Source = "bois.png";
                    numclub.Text = "3";
                    break;
                case "Bois 5":
                    clubs.Source = "bois.png";
                    numclub.Text = "5";
                    break;
                case "Driver":
                    clubs.Source = "bois.png";
                    numclub.Text = "D";
                    break;
                case "Putter":
                    clubs.Source = "put.png";
                    numclub.Text = "";
                    break;
                case "Fer 3":
                    clubs.Source = "fer.png";
                    numclub.Text = "3";
                    break;
                case "Fer 4":
                    clubs.Source = "fer.png";
                    numclub.Text = "4";
                    break;
                case "Fer 5":
                    clubs.Source = "fer.png";
                    numclub.Text = "5";
                    break;
                case "Fer 6":
                    clubs.Source = "fer.png";
                    numclub.Text = "6";
                    break;
                case "Fer 7":
                    clubs.Source = "fer.png";
                    numclub.Text = "7";
                    break;
                case "Fer 8":
                    clubs.Source = "fer.png";
                    numclub.Text = "8";
                    break;
                case "Fer 9":
                    clubs.Source = "fer.png";
                    numclub.Text = "9";
                    break;
                case "Hybride":
                    clubs.Source = "bois.png";
                    numclub.Text = "H";
                    break;
                case "Sandwedge":
                    clubs.Source = "fer.png";
                    numclub.Text = "S";
                    break;
                case "Pitching":
                    clubs.Source = "fer.png";
                    numclub.Text = "P";
                    break;
                default:
                    clubs.Source = "fer.png";
                    numclub.Text = "3";
                    break;
            }
        }

        /**
         * This method is used when the position of the user is being loading
         * Shows the load icon and hides the other ones (no little button when localizing)
         */
        private void showLoad()
        {
            load.IsEnabled = true;
            load.IsVisible = true;
            radar.IsEnabled = false;
            radar.IsVisible = false;
            ball.IsEnabled = false;
            ball.IsVisible = false;
            backToBall.IsVisible = false;
            backToRadar.IsVisible = false;
        }

        /**
         * This method is used when the user is ready to chose a target and to shot
         * Shows the ball icon and hides the other ones (relocalize little button is shown)
         */
        private void showBall()
        {
            state = 0;
            load.IsEnabled = false;
            load.IsVisible = false;
            radar.IsEnabled = false;
            radar.IsVisible = false;
            ball.IsEnabled = true;
            ball.IsVisible = true;
            backToBall.IsVisible = false;
            backToRadar.IsVisible = true;
        }

        /**
         * This method is used when the user has just shot and is ready to localize it
         * Shows the radar icon and hides the other ones (cancel lock little button is shown)
         */
        private void showRadar()
        {
            state = 1;
            load.IsEnabled = false;
            load.IsVisible = false;
            radar.IsEnabled = true;
            radar.IsVisible = true;
            ball.IsEnabled = false;
            ball.IsVisible = false;
            backToBall.IsVisible = true;
            backToRadar.IsVisible = false;
        }

        /**
         * Method executed when clicking on the back arrow when the target is locked to cancel the lock
         */
        private void onBackToBallClicked(object sender, EventArgs e)
        {
            map.setTargetMovable();
            showBall();
            state = 0;
        }

        /**
         * This method is called when a club is clicked on the list of available ones
         */
        private void OnClubClicked(object sender, EventArgs e)
        {
            var club = ListClubsPartie.SelectedItem as Club;
            setCurrentClub(club);
            clubselection.IsVisible = false;
            ListClubsPartie.IsVisible = false;
        }

        /**
         * This method is managing the click on the main button of the page
         */
        private async void onMainButtonClicked(object sender, EventArgs e)
        {
            switch (state)
            {
                case 0://ready to shot
                    showRadar();
                    map.lockTarget();
                    break;

                case 1://ready to localize
                    MyPosition newUserPosition = await localize();
                    MyPosition start = map.getUserPosition();
                    partie.addPositionForCurrentHole(start, new MyPosition(map.TargetPin.Position.Latitude, map.TargetPin.Position.Longitude), newUserPosition);
                    updateScore();
                    map.setUserPosition(newUserPosition, partie.Shots.Count);
                    map.setTargetMovable();
                    updateDistance();
                    showBall();
                    break;

                default://the user is ready to shot by default
                    showBall();
                    break;
            }
        }

        /**
         * This method is called when clicking on the current club
         * Shows or hides the list of available clubs
         */
        private void onClubSelectionClicked(object sender, EventArgs e)
        {
            if(clubselection.IsVisible == false) {
                showClubs();
            } else {
                hideClubs();
            }
        }

        /**
         * This method is called when clicking on the current score
         * Shows or hides the game card
         */
        private void OnScoreClicked(object sender, EventArgs e)
        {
            if (ListHole.IsVisible == false) {
                showCard();
            } else {
                hideCard();
            }
        }

        /**
         * This method is called when clicking on the bottom right hand corner in order to end the current hole
         */
        private async void onHoleFinishedButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new HoleFinishedPage(partie));
        }

        /**
         * This method is called when clicking on the little relocalize button
         */
        private async void onRelocalizeAction(object sender, EventArgs e)
        {
            MyPosition newUserPosition = await localize();
            map.setUserPosition(newUserPosition, partie.Shots.Count);
            updateDistance();
        }

        /**
         * This method is called when clicking on the back button of his phone
         */
        protected override bool OnBackButtonPressed()
        {
            Profil profil = StatistiquesGolf.getProfil();
            System.Diagnostics.Debug.WriteLine(this.partie.holeFinishedCount);
            Device.BeginInvokeOnMainThread(async () =>
            {
                //the user has to confirm his click
                if (await DisplayAlert("Quitter", "Voulez vous arreter cette partie maintenant ?", "Oui", "Non"))
                {
                    if (profil.SaveStats)//check wether the user wants to save his stats or not (a switch in option page manages this choice)
                    {
                        if (this.partie.holeFinishedCount <= 0)
                        {
                            base.OnBackButtonPressed();
                            await Navigation.PopToRootAsync();
                        } else
                        {
                            if (await DisplayAlert("Sauvegarder", "Voulez vous sauvegarder cette partie ?", "Oui", "Non"))
                            {
                                await Navigation.PushAsync(new GameFinishedPage(partie));
                            }
                            else
                            {
                                base.OnBackButtonPressed();
                                await Navigation.PopToRootAsync();
                            }
                        }
                    } else
                    {
                        base.OnBackButtonPressed();
                        await Navigation.PopToRootAsync();
                    }
                }
            });
            return true;
        }
    }
}