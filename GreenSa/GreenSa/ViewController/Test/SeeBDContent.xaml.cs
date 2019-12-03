using GreenSa.Models.GolfModel;
using GreenSa.Models.Tools;
using GreenSa.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GreenSa.ViewController.Test
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SeeBDContent : ContentPage
    {
        public SeeBDContent()
        {
            InitializeComponent();
            //(SQLiteNetExtensions.Extensions.ReadOperations.GetAllWithChildren<GolfCourse>(connection));
            // connection.Query("SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY name; ");
            /*List<GolfCourse> gfcs = new List<GolfCourse>();
            connection.CreateTable<MyPosition>();
            Debug.WriteLine("Vreated table pos");

            connection.CreateTable<GolfCourse>();
            Debug.WriteLine("Vreated table GolfCourse");
            
            gfcs = (SQLiteNetExtensions.Extensions.ReadOperations.GetAllWithChildren<GolfCourse>(connection));*/
        }

        private void seeGolfCourse(object sender, EventArgs e)
        {
            try
            {
                SQLite.SQLiteConnection connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                List<GolfCourse> gfcs = (SQLiteNetExtensions.Extensions.ReadOperations.GetAllWithChildren<GolfCourse>(connection));
                label.Text = "";
                foreach (GolfCourse c in gfcs)
                {
                    label.Text += c.ToString() + "\n";
                }
            }
            catch (Exception ex)
            {
                label.Text = "" + ex.Message;

            }

        }

        private void seeShot(object sender, EventArgs e)
        {
            SQLite.SQLiteConnection connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            try
            {
                List<Shot> gfcs = SQLiteNetExtensions.Extensions.ReadOperations.GetAllWithChildren<Shot>(connection);
                label.Text = "";
                foreach (Shot c in gfcs)
                {
                    label.Text += c.ToString() + "\n";
                }
            }catch(Exception ex)
            {
                label.Text = "" + ex.Message;

            }

        }
        private void seePosition(object sender, EventArgs e)
        {
            try
            {
                SQLite.SQLiteConnection connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                List<MyPosition> gfcs = (SQLiteNetExtensions.Extensions.ReadOperations.GetAllWithChildren<MyPosition>(connection));
                label.Text = "";
                foreach (MyPosition c in gfcs)
                {
                    label.Text += c.ToString() + "\n";
                }
            }catch(Exception ex)
            {
                label.Text = "" + ex.Message;

            }
        }

       

        private void seeClub(object sender, EventArgs e)
        {
            try
            {

                SQLite.SQLiteConnection connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                List<Club> gfcs = (SQLiteNetExtensions.Extensions.ReadOperations.GetAllWithChildren<Club>(connection));
                label.Text = "";
                foreach (Club c in gfcs)
                {
                    label.Text += c.ToString() + "\n";
                }
            }
            catch (Exception ex)
            {
                label.Text = "" + ex.Message;

            }
        }

        private void dropAllTable(object sender, EventArgs e)
        {
            SQLite.SQLiteConnection connection = DependencyService.Get<ISQLiteDb>().GetConnection();

            connection.DropTable<MyPosition>();
            connection.DropTable<GolfCourse>();
            //connection.DropTable<Club>();
            connection.DropTable<Shot>();
            connection.DropTable<Hole>();
            connection.DropTable<ScorePartie>();
            connection.DropTable<ScoreHole>();
        }

        private void seeHoles(object sender, EventArgs e)
        {
            try
            {
                SQLite.SQLiteConnection connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                List<Hole> gfcs = (SQLiteNetExtensions.Extensions.ReadOperations.GetAllWithChildren<Hole>(connection));
                label.Text = "";
                foreach (Hole c in gfcs)
                {
                    label.Text += c.ToString() + "\n";
                }
            }
            catch (Exception ex)
            {
                label.Text = "" + ex.Message;

            }
        }

        private void seeScore(object sender, EventArgs e)
        {
            SQLite.SQLiteConnection connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            try
            {
                List<ScoreHole> gfcs = (SQLiteNetExtensions.Extensions.ReadOperations.GetAllWithChildren<ScoreHole>(connection));
                label.Text = "";
                foreach (ScoreHole c in gfcs)
                {
                    label.Text += c.ToString() + "\n";
                }
            }
            catch (Exception ex)
            {
                label.Text = ""+ex.Message;

            }

        }

        private void seeParties(object sender, EventArgs e)
        {
            SQLite.SQLiteConnection connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            try
            {
                List<ScorePartie> gfcs = (SQLiteNetExtensions.Extensions.ReadOperations.GetAllWithChildren<ScorePartie>(connection));
                label.Text = "";
                foreach (ScorePartie c in gfcs)
                {
                    label.Text += c.ToString() + "\n";
                }
            }
            catch (Exception ex)
            {
                label.Text = "" + ex.Message;

            }

        }

    }
}