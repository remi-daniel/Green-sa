using GreenSa.Models.Tools;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenSa.Models.GolfModel
{
    /**
    * Data structure used to store in the DB information about the score of the player of a particular hole 
    */
    public class ScoreHole
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [ForeignKey(typeof(Hole))]
        public string IdHole { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead)]
        public Hole Hole { get; set; }
        public int Score { get; set; }
        public int Penality { get; set; }
        public bool Hit { get; set; }
        public DateTime Date { get; set; }
        public int NombrePutt{get;set ;}

        //List of the shots performed while playing this hole
        //[OneToMany(CascadeOperations = CascadeOperation.All)] A confirmer!
        //[OneToMany(CascadeOperations = CascadeOperation.CascadeRead)]
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Shot> Shots { get; set; }

        [ForeignKey(typeof(ScorePartie))]
        public int idScorePartie { get; set; }

        public ScoreHole()
        {

        }

        public ScoreHole(Hole hole, int penality, int score,bool hit,int nbPutt,DateTime date)
        {
            this.Hole = hole;
            this.Score = score;
            this.Hit = hit;
            this.Penality = penality;
            Date = date;
            NombrePutt = nbPutt;
        }

        /**
         * New constructor allowing for the addition of the shots in the data structure (see if old one still necessary)
         */
        public ScoreHole(Hole hole, int penality, int score, bool hit, int nbPutt, DateTime date, List<Shot> shots)
        {
            this.Hole = hole;
            this.Score = score;
            this.Hit = hit;
            this.Penality = penality;
            Date = date;
            NombrePutt = nbPutt;
            List<Shot> copy = new List<Shot>(shots);
            this.Shots = copy;
        }

        public override string ToString()
        {
            return "{ Hole "+Hole+" Score "+Score+" ("+(Hit?"Hit":"Miss")+") the "+Date;
        }


    }
}
