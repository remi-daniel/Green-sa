using System;
using System.Collections.Generic;
using GreenSa.Models.GolfModel;
using GreenSa.Models.Tools;
using Entry = Microcharts.Entry;
using Xamarin.Forms;
using SkiaSharp;
using Microcharts;
using GreenSa.ViewController.Profile.Statistiques.SpecificStatistiques;

namespace GreenSa.ViewController.Profile.Statistiques.StatistiquesGolfCourse
{
    public partial class GolfCourseStatPage : ContentPage
    {

        private GolfCourse golfCourse;

        private List<ScoreHole> allScoreHoles;
        private List<ScorePartie> allScoreParties;

        public GolfCourseStatPage(GolfCourse g)
        {
            InitializeComponent();
            this.changeGolfCourse(g);
            this.allScoreParties = null;
            this.allScoreHoles = null;
        }

        public void changeGolfCourse(GolfCourse g)
        {
            this.golfCourse = g;
            this.golfCourseName.Text = this.golfCourse.Name;
        }

        async protected override void OnAppearing()
        {
            if (this.allScoreHoles == null)
            {
                this.allScoreHoles = await StatistiquesGolf.getScoreHoles();
            }
            this.updateChart(allScoreHoles);
            if (this.allScoreParties == null)
            {
                this.allScoreParties = await StatistiquesGolf.getScoreParties();
            }
            this.updateGIR(allScoreParties);
            this.updateAveragePutts(allScoreHoles);
            this.updateWorstHole(allScoreHoles);
        }

        /**
         * Updates the worst hole label
         */
        private void updateWorstHole(List<ScoreHole> allScoreHoles)
        {
            int worstHoleNumber = StatistiquesGolf.getWorstHole(allScoreHoles, this.golfCourse);
            if (worstHoleNumber == 0)
            {
                this.worstHole.Text = GeneralStatPage.NO_DATA;
                this.worstHole.TextColor = Color.Gray;
                this.worstHole.FontSize = 15;
            }
            else
            {
                this.worstHole.Text = "" + worstHoleNumber;
                this.worstHole.TextColor = Color.FromHex("#39B54A");
                this.worstHole.FontSize = 30;
            }
        }

        /**
         * Updates the average putts count labels
         */
        private void updateAveragePutts(List<ScoreHole> allScoreHoles)
        {
            double avPutts = StatistiquesGolf.getAveragePutts(StatistiquesGolf.getScoreHoles(allScoreHoles, this.golfCourse));
            if (avPutts == -1.0)
            {
                this.averagePutts.Text = GeneralStatPage.NO_DATA;
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

        /**
         * Updates the green in regulation stat labels
         */
        private void updateGIR(List<ScorePartie> allScoreParties)
        {
            List<ScorePartie> scoreParties = StatistiquesGolf.getScoreParties(allScoreParties, this.golfCourse);
            double sum = 0.0;
            foreach (ScorePartie sp in scoreParties)
            {
                foreach (ScoreHole sh in sp.scoreHoles)
                {
                    sum += (sh.Hit) ? 1.0 : 0.0;
                }
            }

            if (scoreParties.Count == 0)
            {
                this.averageGIR.Text = GeneralStatPage.NO_DATA;
                this.averageGIR.TextColor = Color.Gray;
                this.averageGIR.FontSize = 15;
            }
            else
            {
                this.averageGIR.Text = "" + (sum/scoreParties.Count).ToString("0.00");
                this.averageGIR.TextColor = Color.FromHex("#39B54A");
                this.averageGIR.FontSize = 30;
            }

        }

        /**
         * Updates the chart
         */
        private void updateChart(List<ScoreHole> allScoreHoles)
        {
            /*float albatros = 0f;*/
            float eagle = 0f;
            float birdie = 0f;
            float par = 0f;
            float bogey = 0f;
            float dbogey = 0f;
            float more = 0f;

            Dictionary<Hole.ScorePossible, float> d = StatistiquesGolf.getProportionScore(allScoreHoles, this.golfCourse);

            foreach (KeyValuePair<Hole.ScorePossible, float> k in d)
            {
                /*if (k.Key.Equals(Hole.ScorePossible.ALBATROS))
                {
                    albatros = k.Value;
                }*/
                if (k.Key.Equals(Hole.ScorePossible.BIRDIE))
                {
                    birdie = k.Value;
                }
                if (k.Key.Equals(Hole.ScorePossible.BOGEY))
                {
                    bogey = k.Value;
                }
                if (k.Key.Equals(Hole.ScorePossible.DOUBLE_BOUGEY))
                {
                    dbogey = k.Value;
                }
                if (k.Key.Equals(Hole.ScorePossible.EAGLE))
                {
                    eagle = k.Value;
                }
                if (k.Key.Equals(Hole.ScorePossible.MORE))
                {
                    more = k.Value;
                }
                if (k.Key.Equals(Hole.ScorePossible.PAR))
                {
                    par = k.Value;
                }
            }


            var entries = new[]
             {

                /*new Entry(albatros)
                 {
                     Label = "Albatros",
                     ValueLabel =float.IsNaN(albatros)?"N/A":(albatros.ToString("n2")   +"%"),
                    Color = SKColor.Parse("#0BF5A3")
                 },*/
                new Entry(eagle)
                 {
                     Label = "Eagle",
                    ValueLabel =float.IsNaN(eagle)?"N/A": (eagle.ToString("n2")),
                    Color = SKColor.Parse("#0BF5A3")
                 },
                new Entry(birdie)
                 {
                     Label = "Birdie",
                    ValueLabel = float.IsNaN(birdie)?"N/A":(birdie.ToString("n2")),
                    Color = SKColor.Parse("#0BF54E")
                 },
                new Entry(par)
                 {
                     Label = "Par",
                    ValueLabel = float.IsNaN(par)?"N/A":(par.ToString("n2")),
                    Color = SKColor.Parse("#44F50B")
                 },
                new Entry(bogey)
                 {
                     Label = "Bogey",
                    ValueLabel = float.IsNaN(bogey)?"N/A":(bogey.ToString("n2")),
                    Color = SKColor.Parse("#C0F50B")
                 },
                new Entry(dbogey)
                 {
                     Label = "Dbl-Bogey",
                    ValueLabel = float.IsNaN(dbogey)?"N/A":(dbogey.ToString("n2")),
                    Color = SKColor.Parse("#F5A00B")
                 },
                new Entry(more)
                 {
                     Label = "More",
                    ValueLabel = float.IsNaN(more)?"N/A":(more.ToString("n2")),
                    Color = SKColor.Parse("#F5340B")
                 },
            };

            this.chartView.Chart = new BarChart() { Entries = entries, LabelTextSize = 26, MaxValue = this.golfCourse.Holes.Count, ValueLabelOrientation = Orientation.Horizontal, LabelOrientation = Orientation.Horizontal};

        }

    }
}
