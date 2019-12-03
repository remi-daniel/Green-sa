using System;
using System.Collections.Generic;
using GreenSa.Models.GolfModel;
using GreenSa.Models.Tools;
using Entry = Microcharts.Entry;
using Xamarin.Forms;
using SkiaSharp;
using Microcharts;
using GreenSa.ViewController.Profile.Statistiques.SpecificStatistiques;
using System.Threading.Tasks;
using System.Linq;
using static GreenSa.Models.GolfModel.Hole;

namespace GreenSa.ViewController.Profile.Statistiques.StatistiquesGolfCourse
{
    public partial class PartieStatPage : ContentPage
    {

        private ScorePartie scorePartie;
        private List<Shot> allShots;

        public PartieStatPage(ScorePartie sp, string golfCourseName)
        {
            InitializeComponent();
            changePartie(sp, golfCourseName);
            this.legendButton.BorderColor = Color.FromHex("0C5E11");
            this.legendButton.BorderWidth = 2;

            this.allShots = null;
            
        }

        /**
         * Changes the game and some concerned labels
         */
        public void changePartie(ScorePartie sp, string golfCourseName)
        {
            this.scorePartie = sp;
            this.partieDate.Text = "Partie du " + sp.DateString + " sur le";
            this.golfCourseName.Text = golfCourseName;
        }

        async protected override void OnAppearing()
        {
            if (this.allShots == null)
            {
                this.allShots = await StatistiquesGolf.getShots();
            }
            this.updateChart();
            this.updateAveragePutts();
            this.updateNotableScores();
        }

        /**
         * Initializes the value of the given key in the dictionnary or increments it if already exists
         */
        private void incrementDicoKey(Dictionary<string, int> dico, string key)
        {
            if (!dico.ContainsKey(key))
            {
                dico[key] = 1;
            }
            else
            {
                dico[key] += 1;
            }
        }

        /**
         * Updates notable scores
         */
        private void updateNotableScores()
        {
            Dictionary<string, int> notableScores = new Dictionary<string, int>();
            List<Shot> ulgss = StatistiquesGolf.getShotsFromPartie(this.scorePartie, this.allShots).Where(sh => sh.ShotType.Equals(Shot.ShotCategory.UnexpectedLongShot)).ToList();
            string key = "UnbelievableUnexpectedShot";
            double max = 0.0;
            double currentDist = 0.0;
            int i = 0;
            int girCount = 0;
            double index = StatistiquesGolf.getPlayerIndex();
            int averageScorePerHole = (int)Math.Round(index / 18.0);
            //fills the dictionnary of relevent scores
            foreach (Shot shot in ulgss)//manages unbelievable unexpected shot index
            {
                currentDist = shot.RealShotDist();
                if (currentDist > max)
                {
                    max = currentDist;
                    notableScores[key] = i;
                }
                i++;
            }
            foreach (ScoreHole sh in this.scorePartie.scoreHoles)
            {
                if (sh.Score >= averageScorePerHole+2)//manages bad scores count
                {
                    key = "More";
                    incrementDicoKey(notableScores, key);
                } else if (sh.Score <= 0)//manages great scores count
                {
                    key = "" + sh.Score;
                    incrementDicoKey(notableScores, key);
                }
                if (sh.NombrePutt == 0)//manages approaches in count
                {
                    key = "approachIn";
                    incrementDicoKey(notableScores, key);
                }
                if (sh.NombrePutt == 1)//manages one putt count
                {
                    key = "onePutt";
                    incrementDicoKey(notableScores, key);
                }
                if (sh.Hit)//manages green in regulation count
                {
                    girCount++;
                }
            }

            int minScoreCount = 2;//for 9 holes or less
            if (this.scorePartie.scoreHoles.Count > 9)
            {
                minScoreCount = 4;
            }
            //checks if there are enought bad scores so that is relevent
            if (notableScores.ContainsKey("More"))
            {
                if (notableScores["More"] < minScoreCount)
                {
                    notableScores.Remove("More");
                }
            }
            //checks if there are enought pars so that is relevent
            if (notableScores.ContainsKey("0"))
            {
                int courseSizeMult = this.scorePartie.scoreHoles.Count / 9;//constants variate if it's a 9 or 18 holes
                int parMinCount = this.scorePartie.scoreHoles.Count - averageScorePerHole * 3 * courseSizeMult;
                if (averageScorePerHole == 0)
                {
                    parMinCount = this.scorePartie.scoreHoles.Count - 2 * courseSizeMult;
                }
                if (parMinCount >= 1)
                {
                    if (notableScores["0"] < parMinCount)
                    {
                        notableScores.Remove("0");
                    }
                }
                
            }
            //checks if there are enought one putt so that is relevent
            if (notableScores.ContainsKey("onePutt"))
            {
                if (notableScores["onePutt"] < minScoreCount)
                {
                    notableScores.Remove("onePutt");
                }
            }

            //choses the more relevent scores
            int chosenCount = 0;
            i = 0;
            if (notableScores.Keys.Count > 1)
            {
                this.notableScore2Frame.IsVisible = true;
                this.notableScore2Label.IsVisible = true;

                for (int j = -3; j<0; ++j)//choses between albatros eagle and birdie
                {
                    if (notableScores.ContainsKey("" + j))//albatros
                    {
                        updateNotableLabel(averageScorePerHole, this.notableScore1, this.notableScore1Label, "" + j, ulgss, notableScores);
                        chosenCount++;
                        break;
                    }
                }
                Label notForScore = this.notableScore1;
                Label notForScoreLabel = this.notableScore1Label;
                if (chosenCount == 1)
                {
                    notForScore = this.notableScore2;
                    notForScoreLabel = this.notableScore2Label;
                }
                //choses one between the ones left
                if (notableScores.ContainsKey("approachIn"))
                {
                    updateNotableLabel(averageScorePerHole, notForScore, notForScoreLabel, "approachIn", ulgss, notableScores);
                    chosenCount++;
                } else if (notableScores.ContainsKey("onePutt"))
                {
                    updateNotableLabel(averageScorePerHole, notForScore, notForScoreLabel, "onePutt", ulgss, notableScores);
                    chosenCount++;
                }
                else if (notableScores.ContainsKey("UnbelievableUnexpectedShot"))
                {
                    updateNotableLabel(averageScorePerHole, notForScore, notForScoreLabel, "UnbelievableUnexpectedShot", ulgss, notableScores);
                    chosenCount++;
                }

                if (chosenCount < 2)
                {
                    if (chosenCount == 0)//if par and more are left -> display GIR on the first labels
                    {
                        this.updateGIR(girCount);
                        if (notableScores.ContainsKey("More"))
                        {
                            updateNotableLabel(averageScorePerHole, this.notableScore2, this.notableScore2Label, "More", ulgss, notableScores);
                        }
                    } else
                    {
                        if (notableScores.ContainsKey("More"))
                        {
                            updateNotableLabel(averageScorePerHole, this.notableScore2, this.notableScore2Label, "More", ulgss, notableScores);
                        } else if (notableScores.ContainsKey("0"))
                        {
                            updateNotableLabel(averageScorePerHole, this.notableScore2, this.notableScore2Label, "0", ulgss, notableScores);
                        } else
                        {
                            updateGIR(this.notableScore2, this.notableScore2Label, girCount);
                        }
                    }
                    
                }
            } else
            {
                this.updateGIR(girCount);
                if (notableScores.Keys.Count == 1)
                {
                    updateNotableLabel(averageScorePerHole, this.notableScore2, this.notableScore2Label, notableScores.Keys.First(), ulgss, notableScores);
                    this.notableScore2Frame.IsVisible = true;
                    this.notableScore2Label.IsVisible = true;
                } else //nothing notable
                {
                    this.notableScore2Frame.IsVisible = false;
                    this.notableScore2Label.IsVisible = false;
                }
            }
        }

        /**
         * Updates a statistic on the view
         */
        private void updateNotableLabel(int averageScorePerHole, Label notableScore, Label notableScoreLabel, string chosenKey, List<Shot> ulgss, Dictionary<string, int> notableScores)
        {
            switch (chosenKey)
            {
                case "UnbelievableUnexpectedShot":
                    Shot unbelievableShot = ulgss[notableScores[chosenKey]];
                    notableScore.Text = "" + (int)unbelievableShot.RealShotDist() + "m";
                    notableScoreLabel.Text = "Distance inattendu d'un de vos coups de " + unbelievableShot.Club.Name;
                    break;

                case "approachIn":
                    notableScore.Text = "" + notableScores[chosenKey];
                    notableScoreLabel.Text = "Nombre d'approches rentrées";
                    break;

                case "onePutt":
                    notableScore.Text = "" + notableScores[chosenKey];
                    notableScoreLabel.Text = "Nombre de 1 putt";
                    break;

                case "More":
                    notableScore.Text = "" + notableScores[chosenKey];
                    notableScoreLabel.Text = "Nombre de trous dont le score est supérieur à +" + (averageScorePerHole + 1);
                    break;

                default://scores under par
                    notableScore.Text = "" + notableScores[chosenKey];
                    notableScoreLabel.Text = "Nombre de " + (((ScorePossible)int.Parse(chosenKey)).ToString());
                    break;
            }
        }

        /**
         * Updates some labels to display green in regulation stats
         * data : the label that will contain the data
         * dataLabel : the label that will contain the description
         * girCount : the green in regulation count
         */
        private void updateGIR(Label data, Label dataLabel, int girCount)
        {
            if (this.scorePartie.scoreHoles.Count == 0)
            {
                data.Text = GeneralStatPage.NO_DATA;
                data.TextColor = Color.Gray;
                data.FontSize = 15;
            }
            else
            {
                data.Text = "" + girCount;
                dataLabel.Text = "Nombre de greens touchés en régulation";
                data.TextColor = Color.FromHex("#39B54A");
                data.FontSize = 30;
            }

        }

        /**
         * Updates the first notable label with green in regulation stat
         * girCount : the green in regulation count
         */
        private void updateGIR(int girCount)
        {
            this.updateGIR(this.notableScore1, this.notableScore1Label, girCount);
        }

        /**
         * Updates the labels to display average putts count statistic
         */
        private void updateAveragePutts()
        {
            double averagePutts = StatistiquesGolf.getAveragePutts(this.scorePartie.scoreHoles);
            if (averagePutts == -1.0)
            {
                this.averagePutt.Text = GeneralStatPage.NO_DATA;
                this.averagePutt.TextColor = Color.Gray;
                this.averagePutt.FontSize = 15;
            }
            else
            {
                this.averagePutt.Text = "" + averagePutts.ToString("0.00");
                this.averagePutt.TextColor = Color.FromHex("#39B54A");
                this.averagePutt.FontSize = 30;
            }
        }

        /**
         * Updates the chart
         */
        private void updateChart()
        {
            List<Shot> allNeededShots = this.allShots.Where(sh => sh.Date >= scorePartie.DateDebut && sh.Date <= scorePartie.DateFin).ToList();
            Dictionary<Shot.ShotCategory, int> dico = StatistiquesGolf.getProportionShot(allNeededShots);
            int shotCount = 0;
            foreach (Shot.ShotCategory sc in dico.Keys)
            {
                if (!sc.Equals(Shot.ShotCategory.ChipShot))
                {
                    shotCount += dico[sc];
                }
            }

            var entries = new[]
             {
                new Entry(dico[Shot.ShotCategory.PerfectShot])
                 {
                    Label = "CP",
                    ValueLabel = dico[Shot.ShotCategory.PerfectShot].ToString(),
                    Color = SKColor.Parse("#0BF5A3")
                 },
                new Entry(dico[Shot.ShotCategory.GoodShot])
                 {
                    Label = "BC",
                    ValueLabel = dico[Shot.ShotCategory.GoodShot].ToString(),
                    Color = SKColor.Parse("#44F50B")
                 },
                new Entry(dico[Shot.ShotCategory.TolerableShot])
                 {
                     Label = "CA",
                    ValueLabel = dico[Shot.ShotCategory.TolerableShot].ToString(),
                    Color = SKColor.Parse("#F5A00B")
                 },
                new Entry(dico[Shot.ShotCategory.UnexpectedLongShot])
                 {
                     Label = "LCI",
                    ValueLabel = dico[Shot.ShotCategory.UnexpectedLongShot].ToString(),
                    Color = SKColor.Parse("#F9E65E")
                 },
                new Entry(dico[Shot.ShotCategory.NotStraightShot])
                 {
                     Label = "CD",
                    ValueLabel = dico[Shot.ShotCategory.NotStraightShot].ToString(),
                    Color = SKColor.Parse("#F5340B")
                 },
                new Entry(dico[Shot.ShotCategory.FailedShot])
                 {
                     Label = "CR",
                    ValueLabel = dico[Shot.ShotCategory.FailedShot].ToString(),
                    Color = SKColor.Parse("#818181")
                 },
                new Entry(dico[Shot.ShotCategory.PenalityShot])
                 {
                     Label = "CdP",
                    ValueLabel = dico[Shot.ShotCategory.PenalityShot].ToString(),
                    Color = SKColor.Parse("#313030")
                 },
            };

            this.chartView.Chart = new BarChart() { Entries = entries, LabelTextSize = 36, MaxValue = shotCount, ValueLabelOrientation = Orientation.Horizontal, LabelOrientation = Orientation.Horizontal};
        }


        /**
         * This method is called when clicking on the legend button
         * Hides the legend and display the stats or vice versa
         */
        private void onLegendClick(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            if (b.Text.Equals("?"))
            {
                b.Text = "X";
                this.legend.IsVisible = true;
                this.bottomStats.IsVisible = false;
            }
            else
            {
                b.Text = "?";
                this.legend.IsVisible = false;
                this.bottomStats.IsVisible = true;
            }
        }

    }
}
