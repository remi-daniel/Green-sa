using GreenSa.Models.Profiles;
using GreenSa.Models.Tools;
using GreenSa.Models.Tools.GPS_Maps;
using GreenSa.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static GreenSa.Models.GolfModel.Hole;

namespace GreenSa.Models.GolfModel
{
    /*
     * Classe donnant les stats de golf
     * 
     * */
    public class StatistiquesGolf
    {

        /**
         * Gets the average distance for all clubs
         * if clubs  parameter is null then it deals with all clubs
         * */
        public async static Task<IEnumerable<Tuple<Club, double>>> getAverageDistanceForClubsAsync(Func<Club, bool> filtre, List<Club> clubs)
        {
            IEnumerable<Tuple<Club, double>> res=new List<Tuple<Club, double>>();
            SQLite.SQLiteConnection connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            Dictionary<Club, double> sommesEachClubs = new Dictionary<Club, double>();
            Dictionary<Club, int> nombreDeShotParClub = new Dictionary<Club, int>();

            try
            {
                if (clubs == null)
                {
                    clubs = await GestionGolfs.getListClubsAsync(null);
                }
                foreach (Club c in clubs)
                {
                    if (!c.Equals(Club.PUTTER))
                    {
                        if (!sommesEachClubs.ContainsKey(c))
                        {
                            sommesEachClubs.Add(c, 0);
                            nombreDeShotParClub.Add(c, 0);
                        }
                        sommesEachClubs[c] += (double)c.DistanceMoyenne;
                        nombreDeShotParClub[c] += 1;
                    }
                }

                IEnumerable<Shot> shots = SQLiteNetExtensions.Extensions.ReadOperations.GetAllWithChildren<Shot>(connection);
                shots = shots.Where(s => filtre(s.Club) && !s.Club.Equals(Club.PUTTER) && !s.ShotType.Equals(Shot.ShotCategory.ChipShot) && !s.ShotType.Equals(Shot.ShotCategory.FailedShot));
                foreach (Shot s in shots)
                {
                    if (!sommesEachClubs.ContainsKey(s.Club))
                    {
                        sommesEachClubs.Add(s.Club, 0);
                        nombreDeShotParClub.Add(s.Club, 0);
                    }
                    sommesEachClubs[s.Club] += s.RealShotDist();
                    nombreDeShotParClub[s.Club] += 1;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error : " + ex.StackTrace);

            }
            res = sommesEachClubs.Select(k => new Tuple<Club, double>(k.Key, k.Value / nombreDeShotParClub[k.Key]));//petit map pour calculer la moyenne
            return res;
        }

        /**
         * Gets the average amount of par of the games given in parameters
         * allScoreParties : a list of ScorePartie
         * return -1.0 if there isn't any game saved in the database
         */
        public static double getAveragePars(List<ScorePartie> allScoreParties)
        {
            double avPars = -1;
            double sum = 0;
            double nineHolesCount = 0;
            foreach (ScorePartie sp in allScoreParties)
            {
                foreach (ScoreHole sh in sp.scoreHoles)
                {
                    sum += (sh.Score == 0) ? 1 : 0;
                }
                nineHolesCount += (sp.scoreHoles.Count == 9) ? 1 : 2;
            }
            if (allScoreParties.Count > 0)
            {
                avPars = sum / nineHolesCount;
            }
            return avPars;
        }

        /**
         * Gets the games that wasn't finished yet on a specific golf course
         * golfCourse : the specific golf course
         */
        public static async Task<List<ScorePartie>> getNotFinishedGames(GolfCourse golfCourse)
        {
            List<ScorePartie> allNeededScoreParties = (await StatistiquesGolf.getScoreParties()).Where(sp => sp.scoreHoles[0].Hole.IdGolfC.Equals(golfCourse.Name) && sp.scoreHoles.Count != 9 && sp.scoreHoles.Count != 18).ToList();
            return allNeededScoreParties;
        }

        /**
         * Gets the average amount of putts in scores of holes
         * scoresHoles : the list of scores of holes
         * return -1.0 if there isn't any putt in the database
         */
        public static double getAveragePutts(List<ScoreHole> scoresHoles)
        {
            double avPutts = -1.0;
            int sum = 0;
            foreach (ScoreHole sh in scoresHoles)
            {
                sum += sh.NombrePutt;
            }
            if (scoresHoles.Count > 0)
            {
                avPutts = (double)((double)sum / (double)scoresHoles.Count);
            }
            return avPutts;
        }

        /**
         * Gets all the shots in the database
         */
        public async static Task<List<Shot>> getShots()
        {
            SQLite.SQLiteAsyncConnection connection = DependencyService.Get<ISQLiteDb>().GetConnectionAsync();
            await connection.CreateTableAsync<Shot>();
            List<Shot> allShots = await SQLiteNetExtensionsAsync.Extensions.ReadOperations.GetAllWithChildrenAsync<Shot>(connection);
            return allShots;
        }

        /**
         * Gets all the ScorePartie from the database
         */
        public async static Task<List<ScorePartie>> getScoreParties()
        {
            SQLite.SQLiteAsyncConnection connection = DependencyService.Get<ISQLiteDb>().GetConnectionAsync();
            await connection.CreateTableAsync<ScorePartie>();
            List<ScorePartie> allScoreParties = await SQLiteNetExtensionsAsync.Extensions.ReadOperations.GetAllWithChildrenAsync<ScorePartie>(connection, recursive:true);
            return allScoreParties;
        }

        /**
         * Gets all the ScoreHole from the database
         */
        public async static Task<List<ScoreHole>> getScoreHoles()
        {
            SQLite.SQLiteAsyncConnection connection = DependencyService.Get<ISQLiteDb>().GetConnectionAsync();
            await connection.CreateTableAsync<ScoreHole>();
            List<ScoreHole> allScoreHoles = await SQLiteNetExtensionsAsync.Extensions.ReadOperations.GetAllWithChildrenAsync<ScoreHole>(connection, recursive: true);
            return allScoreHoles;
        }

        /**
         * Gets all the golf courses from the database
         */
        public async static Task<List<GolfCourse>> getGolfCourses()
        {
            SQLite.SQLiteAsyncConnection connection = DependencyService.Get<ISQLiteDb>().GetConnectionAsync();
            await connection.CreateTableAsync<GolfCourse>();
            List<GolfCourse> golfCourses = await SQLiteNetExtensionsAsync.Extensions.ReadOperations.GetAllWithChildrenAsync<GolfCourse>(connection);
            return golfCourses;
        }

        /**
         * Filters the ScorePartie in the given list that took place in the given golf course 
         * allScoreParties : the initial list of ScorePartie
         * golfCourse : the golf course filter
         */
        public static List<ScorePartie> getScoreParties(List<ScorePartie> allScoreParties, GolfCourse golfCourse)
        {
            List<ScorePartie> allNeededScoreParties = allScoreParties.Where(sh => sh.scoreHoles[0].Hole.IdGolfC.Equals(golfCourse.Name)).ToList();
            return allNeededScoreParties;
        }

        /**
         * Filters the ScoreHole in the given list that took place in the given golf course 
         * allScoreHoles : the initial list of ScoreHole
         * golfCourse : the golf course filter
         */
        public static List<ScoreHole> getScoreHoles(List<ScoreHole> allScoreHoles, GolfCourse golfCourse)
        {
            List<ScoreHole> allNeededScoreHoles = allScoreHoles.Where(sh => sh.Hole.IdGolfC.Equals(golfCourse.Name)).ToList();
            return allNeededScoreHoles;
        }
        
        /**
         * Filters the given shot list keeping only the ones that was done during a game using the starting and ending date of the game
         * scorePartie : the game score
         * allShots : the list of shot to filter
         */
        public static List<Shot> getShotsFromPartie(ScorePartie scorePartie, List<Shot> allShots)
        {
            return allShots.Where(sh => sh.Date >= scorePartie.DateDebut && sh.Date <= scorePartie.DateFin && !sh.isPutt()).ToList();
        }

        /** 
         * Gets a tuple containing the name of the club with which the player did the higher distance and this distance
         */
        public static Tuple<string, int> getMaxDistClub(List<Shot> allShots)
        {
            double maxDist = 0.0;
            string clubName = "";
            foreach (Shot shot in allShots)
            {
                double dist = shot.RealShotDist();
                if (dist > maxDist)
                {
                    maxDist = dist;
                    clubName = shot.Club.Name;
                }
            }
            return new Tuple<string, int>(clubName, (int)maxDist);
        }

        /**
         * Gets the hole where the player plays the worst in a specific golf course
         * allScoreHoles : a list of ScoreHole
         * golfCourse : the concerned golf course
         * return the numero of the worst hole
         */
        public static int getWorstHole(List<ScoreHole> allScoreHoles, GolfCourse golfCourse)
        {
            int holeNumber = 0;
            List<ScoreHole> scoreHoles = getScoreHoles(allScoreHoles, golfCourse);
            if (scoreHoles.Count > 0)
            {
                //Dictionnary<Hole, Tuple<sum, count>> to compute mean
                Dictionary<Hole, double> sumScorePerHole = new Dictionary<Hole, double>();
                Dictionary<Hole, double> countScorePerHole = new Dictionary<Hole, double>();
                foreach (ScoreHole sh in scoreHoles)
                {
                    if (!sumScorePerHole.ContainsKey(sh.Hole))
                    {
                        sumScorePerHole.Add(sh.Hole, 0.0);
                        countScorePerHole.Add(sh.Hole, 0.0);
                    }
                    sumScorePerHole[sh.Hole] += sh.Score;
                    countScorePerHole[sh.Hole] += 1.0;
                }
                double maxScore = 0.0;
                foreach (Hole hole in sumScorePerHole.Keys)
                {
                    double currentMean = sumScorePerHole[hole] / countScorePerHole[hole];
                    if (currentMean > maxScore)
                    {
                        maxScore = currentMean;
                        holeNumber = golfCourse.Holes.IndexOf(hole) + 1;
                    }
                }
            }
            return holeNumber;
        }


        /**
         * Gets the proportion of each score done in a golf course
         * allScoreHoles : a list a ScoreHole
         * golfCourse : the concerned golf course
         * return a dictionnary where each possible score entry gives its proportion (it's not a percentage but it's over the number of holes ofgolf course)
         */
        public static Dictionary<ScorePossible,float> getProportionScore(List<ScoreHole> allScoreHoles, GolfCourse golfCourse)
        {
            Dictionary<ScorePossible, float> res = new Dictionary<ScorePossible, float>();
            List<ScoreHole> allNeededScoreHoles = getScoreHoles(allScoreHoles, golfCourse);
            int nbTotal = allNeededScoreHoles.Count;

            res.Add(ScorePossible.ALBATROS, allNeededScoreHoles.Where<ScoreHole>(sh => (sh.Score <= (int)ScorePossible.ALBATROS)).Count() / (float)nbTotal * golfCourse.Holes.Count);
            res.Add(ScorePossible.EAGLE, allNeededScoreHoles.Where<ScoreHole>(sh => (sh.Score==(int)ScorePossible.EAGLE)).Count()/ (float)nbTotal * golfCourse.Holes.Count);
            res.Add(ScorePossible.BIRDIE, allNeededScoreHoles.Where<ScoreHole>(sh => (sh.Score == (int)ScorePossible.BIRDIE)).Count() / (float)nbTotal * golfCourse.Holes.Count);
            res.Add(ScorePossible.PAR, allNeededScoreHoles.Where<ScoreHole>(sh => (sh.Score == (int)ScorePossible.PAR)).Count() / (float)nbTotal * golfCourse.Holes.Count);
            res.Add(ScorePossible.BOGEY, allNeededScoreHoles.Where<ScoreHole>(sh => (sh.Score == (int)ScorePossible.BOGEY)).Count() / (float)nbTotal * golfCourse.Holes.Count);
            res.Add(ScorePossible.DOUBLE_BOUGEY, allNeededScoreHoles.Where<ScoreHole>(sh => (sh.Score == (int)ScorePossible.DOUBLE_BOUGEY)).Count() / (float)nbTotal * golfCourse.Holes.Count);
            res.Add(ScorePossible.MORE, allNeededScoreHoles.Where<ScoreHole>(sh => (sh.Score >= (int)ScorePossible.MORE)).Count() / (float)nbTotal * golfCourse.Holes.Count);

            return res;
        }

        /**
         * Gets the amount of each shot category but chipShot in the given list of shot
         * shots : a list a shots
         * return a dictionnary where each shot category entry gives its amount in the given list of shot
         */
        public static Dictionary<Shot.ShotCategory, int> getProportionShot(List<Shot> shots)
        {
            Shot.ShotCategory[] shotCategories = {Shot.ShotCategory.PerfectShot, Shot.ShotCategory.GoodShot, Shot.ShotCategory.TolerableShot, Shot.ShotCategory.UnexpectedLongShot, Shot.ShotCategory.NotStraightShot, Shot.ShotCategory.FailedShot, Shot.ShotCategory.PenalityShot};
            Dictionary<Shot.ShotCategory, int> dico = new Dictionary<Shot.ShotCategory, int>();

            foreach (Shot.ShotCategory sc in shotCategories)
            {
                dico[sc] = 0;
            }
            foreach (Shot shot in shots)
            {
                if (!shot.isPutt() && shotCategories.Contains(shot.ShotType))
                {
                    dico[shot.ShotType] += 1;
                }
                if (shot.PenalityCount > 0)
                {
                    dico[Shot.ShotCategory.PenalityShot] += shot.PenalityCount;
                }
            }
            return dico;
        }

        /**
         * Saves the data of a hole
         * game : the game
         * hole : the hole to save
         * save : true to save the hole stats, false otherwise. false returns the ScoreHole anyway
         * return the created ScoreHole
         */
        public static ScoreHole saveForStats(Partie game, Hole hole, bool save)
        {
            ScoreHole h = new ScoreHole(hole, game.getPenalityCount(), game.getCurrentScore(), isHit(game.Shots, hole.Par), nbCoupPutt(game.Shots),DateTime.Now);
            if (save)
            {
                if (game.Shots.Count == 0)
                    throw new Exception("0 shots dans la liste des shots.");
                SQLite.SQLiteConnection connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                connection.CreateTable<Club>();
                connection.CreateTable<MyPosition>();
                connection.CreateTable<Shot>();
                //first let's insert in the database all the shots currently stored in the game
                SQLiteNetExtensions.Extensions.WriteOperations.InsertOrReplaceAllWithChildren(connection, game.Shots, true);
                connection.CreateTable<ScoreHole>();

                //then creates a ScoreHole object that stores the hole statistics and insert it in the database
                SQLiteNetExtensions.Extensions.WriteOperations.InsertWithChildren(connection, h, false);
                string sql = @"select last_insert_rowid()";
                h.Id = connection.ExecuteScalar<int>(sql);
            }
            return h;
        }

        /**
         * Saves the given game
         * scoreOfThisPartie : the game statistics
         */
        public async static Task saveGameForStats(ScorePartie scoreOfThisPartie)
        {
            SQLite.SQLiteAsyncConnection connection = DependencyService.Get<ISQLiteDb>().GetConnectionAsync();
            await connection.CreateTableAsync<ScoreHole>();
            await connection.CreateTableAsync<ScorePartie>();

            await SQLiteNetExtensionsAsync.Extensions.WriteOperations.InsertOrReplaceWithChildrenAsync(connection, scoreOfThisPartie,false);
        }

        /**
         * Gets the profil of the user 
         * the user profile is supposed to be describe as one line in the Profil table in the database, so the first line of the table is returned
         */
        public static Profil getProfil()
        {
            SQLite.SQLiteConnection connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            connection.CreateTable<Profil>();
            List<Profil> profils = SQLiteNetExtensions.Extensions.ReadOperations.GetAllWithChildren<Profil>(connection);
            if (profils.Count > 0)
            {
                return profils[0];
            } else
            {
                return null;
            }
        }

        /**
         * Gets the player index
         */
        public static double getPlayerIndex()
        {
            return getProfil().Index;
        }


        /** 
         * Computes the number of shots before the ball gets onto the green
         * shots : the list of the shots of a single hole
         * return : the number of shots before the ball gets onto the green
         */
        private static int numberShotsBeforeGreen(List<Shot> shots)
        {
            int puttCount = StatistiquesGolf.nbCoupPutt(shots);
            return shots.Count - puttCount;
        }

        /** 
         * Computes the number of putts
         * shots : the list of the shots of a single hole
         * return : the number of putts
         */
        private static int nbCoupPutt(List<Shot> shots)
        {
            int puttCount = 0;
            foreach (Shot shot in shots)
            {
                if (shot.Club.IsPutter())
                {
                    puttCount++;
                }
            }
            return puttCount;
        }

        /** 
         * Checks if the green was reached in regulation
         * shots : the list of the shots of a single hole
         * return : true if the number of shots before the ball gets onto the green is lower or equals to 2 under the par, false otherewise
         */
        private static bool isHit(List<Shot> shots, int par)
        {
            int shotsBeforeGreen = numberShotsBeforeGreen(shots);
            //int nbCoutNecessairePourHit = (int) (3.0 / 5 * par);
            int nbCoutNecessairePourHit = par - 2;

            return shotsBeforeGreen <= nbCoutNecessairePourHit;
        }


    }
}
