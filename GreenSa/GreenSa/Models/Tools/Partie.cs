using GreenSa.Models.Tools;
using GreenSa.Persistence;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GreenSa.Models.GolfModel
{
    public class Partie
    {

        private GolfCourse golfCourse;
        private Club currentClub;

        public Club CurrentClub
        {
            get
            {
                return currentClub;
            }
            set
            {
                currentClub = value;
            }
        }
        public List<Shot> Shots { get; set; }
        public List<Club> Clubs { get; set; }
        public List<Hole> Holes;
        private List<Hole>.Enumerator itHole;
        public ScorePartie ScoreOfThisPartie { get; set; }
        public int holeFinishedCount;

        public GolfCourse GolfCourse
        {
            get
            {
                return golfCourse;
            }
            set
            {
                golfCourse = value;
                Holes = value.Holes;
                itHole = value.GetHoleEnumerator();
            }
        }

        public Partie()
        {
            Shots = new List<Shot>();
            CurrentClub = GolfXMLReader.getClubFromName("Fer3");
            ScoreOfThisPartie = new ScorePartie();
            this.holeFinishedCount = -1;
        }

        public void setCurrentClub(Club club)
        {
            CurrentClub = club;
        }

        /**
         * Computes the number of penality shots done during the game
         */
        public int getPenalityCount()
        {
            int penalities = 0;
            foreach (Shot shot in this.Shots)
            {
                penalities += shot.PenalityCount;
            }
            return penalities;
        }

        /**
         * Gets the current number of shots the user played over the par of the current game
         */
        public int getCurrentScore()
        {
            int penalities = getPenalityCount();
            return penalities + this.Shots.Count - this.getNextHole().Par;
        }


        /**
         * Gets the next hole
         */
        public Hole getNextHole()
        {
            return itHole.Current;
        }

        /**
         * Gets the numero of the current hole
         */
        public int getCurrentHoleNumero()
        {
            return golfCourse.Holes.IndexOf(itHole.Current) + 1;
        }

        /**
         * Adds a shot in the shots list using positions in parameters to create it
         * start : the position where the shot was performed
         * target : the position of the target on the map
         * userPosition : the current position of the user (the position where the ball was shot)
         * return the created Shot object
         */
        public Shot addPositionForCurrentHole(MyPosition start, MyPosition target, MyPosition userPosition)
        {
            Shot s = new Shot(CurrentClub, start, target, userPosition, DateTime.Now);
            Shots.Add(s);
            return s;
        }

        /**
         * Manages the end of the current hole
         * saveForStatistics : true to save the stats of the current hole, false to not save them
         */
        public void holeFinished(bool saveForStatistics)
        {
            ScoreHole sh = StatistiquesGolf.saveForStats(this, itHole.Current, saveForStatistics);
            ScoreOfThisPartie.add(sh);
            Shots.Clear();
        }

        /**
         * Manages the end of this game
         * saveForStatistics : true to save the stats of this game, false to not save them
         */
        public async Task gameFinished(bool saveForStatistics)
        {
            if (saveForStatistics)
            {
                ScoreOfThisPartie.DateFin = DateTime.Now;
                await StatistiquesGolf.saveGameForStats(ScoreOfThisPartie);
            }
        }


        /**
         * Checks if there is any hole left to play
         * return true if there is any hole left to play, false otherwise
         */
        public bool hasNextHole()
        {
            return this.holeFinishedCount < this.Holes.Count - 1;
        }

        /**
         * Moves to the next hole if one exists
         * return true if there is any hole left to play, false otherwise
         */
        public bool nextHole()
        {
            this.holeFinishedCount++;
            return itHole.MoveNext();
        }

        /**
         * Updates the circle of the targeted shot on the map
         */
        internal void updateUICircle()
        {
            MessagingCenter.Send<Partie>(this, "updateTheCircle");
        }
    }


}
