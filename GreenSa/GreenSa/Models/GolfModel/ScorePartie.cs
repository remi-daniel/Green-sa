using GreenSa.Models.Tools;
using GreenSa.Persistence;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GreenSa.Models.GolfModel
{
    public class ScorePartie
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead)]
        public List<ScoreHole> scoreHoles { get; set; }

        public DateTime DateDebut { get; set; }

        public DateTime DateFin { get; set; }

        [Ignore]
        public String DateString
        {
            get
            {
                return  ((DateDebut.Day < 10) ? "0" : "") + DateDebut.Day + "/" + ((DateDebut.Month < 10) ? "0" : "") + DateDebut.Month + "/" + (DateDebut.Year-2000);
            }
        }

        public string GolfName { get; set; }

        public Tuple<int, int> GetScore()
        {
            int score = 0;
            foreach (ScoreHole sh in this.scoreHoles)
            {
                score += sh.Score;
            }
            return new Tuple<int, int>(score, this.scoreHoles.Count);
        }

        public ScorePartie() : this(DateTime.Now)
        {
        }

        public ScorePartie(DateTime date)
        {
            scoreHoles = new List<ScoreHole>();
            DateDebut = date;
        }

        /**
         * Adds a ScoreHole in the scoreHole list
         */
        public void add(ScoreHole sh)
        {
            scoreHoles.Add(sh);
            if (scoreHoles.Count == 1)//if first element added then init GolfName
            {
                var id = this.scoreHoles[0].Hole.Id;
                System.Diagnostics.Debug.WriteLine("before");
                SQLite.SQLiteConnection connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                connection.CreateTable<GolfCourse>();
                List<GolfCourse> allGolfCourses = SQLiteNetExtensions.Extensions.ReadOperations.GetAllWithChildren<GolfCourse>(connection);
                foreach (GolfCourse gc in allGolfCourses)
                {
                    foreach (Hole h in gc.Holes)
                    {
                        if (h.Id.Equals(id))
                        {
                            this.GolfName = gc.Name;
                            break;
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine("after");
            }
        }

        /**
         * Converts an integer describing a mounth into its name as a string
         */
        private string getMonthStr(int month)
        {
            String mont = "";
            switch (month)
            {
                case 1:
                    mont = "janvier";
                    break;
                case 2:
                    mont = "février";
                    break;
                case 3:
                    mont = "mars";
                    break;
                case 4:
                    mont = "avril";
                    break;
                case 5:
                    mont = "mai";
                    break;
                case 6:
                    mont = "juin";
                    break;
                case 7:
                    mont = "juillet";
                    break;
                case 8:
                    mont = "août";
                    break;
                case 9:
                    mont = "septembre";
                    break;
                case 10:
                    mont = "octobre";
                    break;
                case 11:
                    mont = "novembre";
                    break;
                case 12:
                    mont = "décembre";
                    break;
            }
            return mont;
        }
    }
}
