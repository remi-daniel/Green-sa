using System;
using System.Collections.Generic;
using GreenSa.Models.GolfModel;
using GreenSa.Models.Tools;

using Xamarin.Forms;

namespace GreenSa.ViewController.Profile.MyGames
{
    public partial class DetailsPartiePage : ContentPage
    {
        ScorePartie sp;
        public DetailsPartiePage(ScorePartie sp)
        {
            InitializeComponent();
            this.sp = sp;
        }

        /**
         * This method is executed when the page is loaded
         * */
        protected override void OnAppearing()
        {
            base.OnAppearing();

            List<DisplayScoreCard> list = new List<DisplayScoreCard>();
            int i = 1;
            int totPar = 0;
            int totPutt = 0;
            int totPen = 0;
            var totScore = 0;
            //computes the total number of pars, putts, penality shots and shots of this game and creates a list of DisplayScoreCard (one by hole)
            foreach (ScoreHole sh in sp.scoreHoles)
            {
                list.Add(new DisplayScoreCard(i, sh));
                i++;
                totPar += sh.Hole.Par;
                totPutt += sh.NombrePutt;
                totPen += sh.Penality;
                totScore += (sh.Score + sh.Hole.Par);

            }
            listScore.ItemsSource = list;

            //manages the number of shots that the user did over the par of the golf course (top right hand corner of the view)
            var scoreDelta = totScore - totPar;
            var sdAbs = Math.Abs(scoreDelta);
            if (scoreDelta < 0)
            {
                score.Text = "-" + sdAbs;
            }
            else
            {
                score.Text = "+" + scoreDelta;
            }
            if (sdAbs >= 100)
            {
                score.FontSize = 25;
                score.Margin = new Thickness(MainPage.responsiveDesign(32), MainPage.responsiveDesign(33), 0, 0);
            }
            else if (sdAbs >= 10)
            {
                score.FontSize = 30;
                score.Margin = new Thickness(MainPage.responsiveDesign(35), MainPage.responsiveDesign(29), 0, 0);
            }
            else
            {
                score.Margin = new Thickness(MainPage.responsiveDesign(39), MainPage.responsiveDesign(27), 0, 0);
            }
            title.Margin = new Thickness(MainPage.responsiveDesign(65), MainPage.responsiveDesign(20), 0, 0);
            totalPar.Text = totPar + "";
            totalPutt.Text = totPutt + "";
            totalPen.Text = totPen + "";
            totalScore.Text = totScore + "";

            partie.Text = sp.GolfName + " :";
            date.Text = sp.DateString;
        }

        /**
        * This method is called when a hole is selected in the list and displays its game history
        */
        private async void onHistoryClick(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            DisplayScoreCard displayCard = (DisplayScoreCard)b.CommandParameter;
            int scoreHolePosition = int.Parse(displayCard.number) - 1;//The holes have numbers from 1 to n, and are sorted in the list from 0 to n-1
            await Navigation.PushAsync(new HistoryPage(sp,scoreHolePosition,false));
        }

    }
}
