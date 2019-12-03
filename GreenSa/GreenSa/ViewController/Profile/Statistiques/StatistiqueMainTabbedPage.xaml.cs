using GreenSa.ViewController.Profile.Statistiques.SpecificStatistiques;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using GreenSa.ViewController.Play;
using GreenSa.ViewController.Profile.MyGames;

namespace GreenSa.ViewController.Profile.Statistiques
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatistiqueMainTabbedPage : TabbedPage
    {

        private GeneralStatPage generalStatPage;
        private GolfSelectionPage golfSelectionPage;
        private ViewPartieListPage partieSelectionPage;

        public StatistiqueMainTabbedPage()
        {
            InitializeComponent();
            this.BarBackgroundColor = Color.FromHex("0A7210");
            this.BarTextColor = Color.White;

            this.generalStatPage = new GeneralStatPage();
            this.generalStatPage.Title = "Général";
            this.Children.Add(this.generalStatPage);

            this.golfSelectionPage = new GolfSelectionPage();
            this.golfSelectionPage.Title = "Par parcours";
            this.Children.Add(this.golfSelectionPage);

            this.partieSelectionPage = new ViewPartieListPage(1);
            this.partieSelectionPage.Title = "Par partie";
            this.Children.Add(this.partieSelectionPage);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    
    }
}