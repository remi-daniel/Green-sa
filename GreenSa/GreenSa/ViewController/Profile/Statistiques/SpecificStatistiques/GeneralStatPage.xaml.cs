using GreenSa.Models.GolfModel;
using GreenSa.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using Microcharts;
using Entry = Microcharts.Entry;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SkiaSharp;
using GreenSa.Persistence;

namespace GreenSa.ViewController.Profile.Statistiques.SpecificStatistiques
{
    public partial class GeneralStatPage : ContentPage
    {

        public static string NO_DATA_LIST = "-";
        public static string NO_DATA = "NoData";

        /* Theses lists are working as a cache to not select items from the database each time the user come into a page.
         * That way, lists are filled only once and the display of statistics is faster
         */
        private List<GolfCourse> allGolfCourses;
        private List<ScorePartie> allScoreParties;
        private List<Shot> allShots;
        private List<ScoreHole> allScoreHoles;

        public GeneralStatPage()
        {
            InitializeComponent();
            this.allGolfCourses = null;
            this.allScoreParties = null;
            this.allScoreHoles = null;
            this.allShots = null;
        }

        async protected override void OnAppearing()
        {
            if (this.allGolfCourses == null)
            {
                this.allGolfCourses = await StatistiquesGolf.getGolfCourses();
            }
            if (this.allScoreParties == null)
            {
                this.allScoreParties = await StatistiquesGolf.getScoreParties();
            }
            this.updateLast4Scores(allGolfCourses, allScoreParties);
            if (this.allShots == null)
            {
                this.allShots = await StatistiquesGolf.getShots();
            }
            this.updateMaxDistClubStat(allShots);
            if (this.allScoreHoles == null)
            {
                this.allScoreHoles = await StatistiquesGolf.getScoreHoles();
            }
            this.updateAveragePutts(allScoreHoles);
            this.updateAveragePar(allScoreParties);
        }


            private void updateLast4Scores(List<GolfCourse> allGolfCourses, List<ScorePartie> allScoreParties)
        {
            int index = (int)StatistiquesGolf.getPlayerIndex();
            int rowCount = last4ScoresGrid.Children.Count / 3;
            var scores = allScoreParties.OrderByDescending(d => d.DateDebut).ToList();
            int col = 0;
            int row = 0;
            foreach (View label in last4ScoresGrid.Children)
            {
                if (row > 0)
                {
                    switch (col)
                    {
                        case 0://column golf name
                            if (row <= scores.Count)
                            {
                                string courseName = "";
                                string id = scores[row - 1].scoreHoles[0].IdHole;
                                foreach (GolfCourse gc in allGolfCourses)
                                {
                                    foreach (Hole h in gc.Holes)
                                    {
                                        if (h.Id.Equals(id))
                                        {
                                            courseName = gc.Name;
                                            break;
                                        }
                                    }
                                }
                                ((Label)label).Text = courseName;
                            }
                            else
                            {
                                ((Label)label).Text = NO_DATA_LIST;
                            }
                            break;

                        case 1://column date
                            if (row <= scores.Count)
                            {
                                ((Label)label).Text = "" + scores[row - 1].DateString;
                            }
                            else
                            {
                                ((Label)label).Text = NO_DATA_LIST;
                            }
                            break;

                        case 2://column score
                            if (row <= scores.Count)
                            {
                                Tuple<int, int> score = scores[row - 1].GetScore();
                                int perf = (int)((double)score.Item1 / (double)score.Item2 * 18.0) - index;
                                ((Label)label).Text = ((score.Item1 >= 0) ? "+" : "") + score.Item1 + " / " + score.Item2 + " trous";
                                if (perf == 0)//you played your index
                                {
                                    ((Label)label).TextColor = Color.Gray;
                                }
                                else if (perf < 0)//you played better than your index
                                {
                                    ((Label)label).TextColor = Color.Green;
                                }
                                else if (perf < 10)//you played worse than your index
                                {
                                    ((Label)label).TextColor = Color.Orange;
                                }
                                else//you played than your index
                                {
                                    ((Label)label).TextColor = Color.Red;
                                }
                            }
                            else
                            {
                                ((Label)label).Text = NO_DATA_LIST;
                            }
                            break;

                        default:
                            break;
                    }
                    row++;
                    if (row == rowCount)
                    {
                        row = 0;
                        col++;
                    }
                }
                else
                {
                    row++;
                }
            }
        }


        private void updateAveragePar(List<ScorePartie> allScoreParties)
        {
            double avPars = StatistiquesGolf.getAveragePars(allScoreParties);
            if (avPars == -1.0)
            {
                this.averagePars.Text = NO_DATA;
                this.averagePars.TextColor = Color.Gray;
                this.averagePars.FontSize = 15;
            }
            else
            {
                this.averagePars.Text = "" + avPars.ToString("0.00");
                this.averagePars.TextColor = Color.FromHex("#39B54A");
                this.averagePars.FontSize = 30;
            }
        }

        private void updateAveragePutts(List<ScoreHole> allScoreHoles)
        {
            double avPutts = StatistiquesGolf.getAveragePutts(allScoreHoles);
            if (avPutts == -1.0)
            {
                this.averagePutts.Text = NO_DATA;
                this.averagePutts.TextColor = Color.Gray;
                this.averagePutts.FontSize = 15;
            }
            else
            {
                this.averagePutts.Text = "" + avPutts.ToString("0.00");
                this.averagePutts.TextColor = Color.FromHex("#39B54A");
                this.averagePutts.FontSize = 30;
            }
        }



        private void updateMaxDistClubStat(List<Shot> allShots)
        {
            Tuple<string, int> maxDist = StatistiquesGolf.getMaxDistClub(allShots);
            if (maxDist.Item2 == 0.0)
            {
                this.maxDistClubLabel.Text = "Coup le plus long";
                this.maxDistClub.Text = NO_DATA;
                this.maxDistClub.TextColor = Color.Gray;
                this.maxDistClub.FontSize = 15;
            }
            else
            {
                this.maxDistClubLabel.Text = maxDist.Item1 + " le plus long";
                this.maxDistClub.Text = maxDist.Item2 + " m";
                this.maxDistClub.TextColor = Color.FromHex("#39B54A");
                this.maxDistClub.FontSize = 30;
            }
        }



        /**
         * Méthode déclenchée au click sur le bouton "Distances moyennes de vos clubs"
         * Redirige vers la page "DistanceClubPage"
         * */
        async private void OnDistClubClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DistanceClubPage());
        }

    }
}