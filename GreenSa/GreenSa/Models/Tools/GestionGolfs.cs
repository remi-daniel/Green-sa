using GreenSa.Models.Tools;
using GreenSa.Persistence;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Xamarin.Forms;

namespace GreenSa.Models.GolfModel
{
    public class GestionGolfs
    {
        private static IEnumerable<Tuple<Club, double>> listAverage;

        /**
         * Gets a list of golf courses using a filter
         * if the filter is null, then all golf courses are returned
         */
        public static async Task<List<GolfCourse>> getListGolfsAsync(Func<GolfCourse, bool> filtre)
        {
            if (filtre == null)
                filtre = x => true;

            SQLite.SQLiteAsyncConnection connection = DependencyService.Get<ISQLiteDb>().GetConnectionAsync();
            await connection.CreateTableAsync<Hole>();
            await connection.CreateTableAsync<MyPosition>();
            await connection.CreateTableAsync<GolfCourse>();
            List<GolfCourse> gfcs = (await SQLiteNetExtensionsAsync.Extensions.ReadOperations.GetAllWithChildrenAsync<GolfCourse>(connection, recursive: true));

            if (gfcs.Count == 0)//if no golf courses in the datatbase
            {
                //parse the default golf courses of Rennes from the XML files and add them (Ressources/GolfCourses)
                gfcs = GolfXMLReader.getListGolfCourseFromXMLFiles();
                await SQLiteNetExtensionsAsync.Extensions.WriteOperations.InsertOrReplaceAllWithChildrenAsync(connection, gfcs, true);
            }
            return gfcs;
        }

        /**
         * Gets a list of clubs using a filter
         * if the filter is null, then all clubs are returned
         * */
        public static async Task<List<Club>> getListClubsAsync(Func<Club, bool> filtre)
        {
            if (filtre == null)
                filtre = x => true;

            SQLite.SQLiteAsyncConnection connection = DependencyService.Get<ISQLiteDb>().GetConnectionAsync();
            List<Club> clubs = new List<Club>();
            await connection.CreateTableAsync<Club>();
            clubs = (await SQLiteNetExtensionsAsync.Extensions.ReadOperations.GetAllWithChildrenAsync<Club>(connection));

            if (clubs.Count == 0)//if no clubs in the datatbase
            {
                //parse the default clubs from the XML files and add them (Ressources/Clubs)
                clubs = GolfXMLReader.getListClubFromXMLFiles();
                await connection.InsertAllAsync(clubs);
            }
            return clubs;
        }

        /**
         * Initialized the list of average distances with the ones of the clubs in parameter
         * clubs : the list of clubs to compute the average distance
         */
        public async static void calculAverageAsync(List<Club> clubs)
        {
            listAverage = await StatistiquesGolf.getAverageDistanceForClubsAsync(c => clubs.Contains(c), clubs);
        }

        /**
         * Selects for the user the most appropriate club from the given list of clubs depending on the distance between him and the target
         * clubs : a list of clubs where to perform the selection
         * dUserTarget : the distance between the user and the target
         */
        public static Club giveMeTheBestClubForThatDistance(List<Club> clubs, double dUserTarget)
        {
            /* Easier version but slower one
                Club minDiffClub = clubs.First();
                double minDiff = Math.Abs(dUserTarget - listAverage.ToList().Find(c => c.Item1.Name == minDiffClub.Name).Item2);
                double dist = -1;
                foreach (Club c in clubs)
                {
                    if (!c.Equals(Club.PUTTER))
                    {
                        dist = Math.Abs(dUserTarget - listAverage.ToList().Find(c1 => c1.Item1.Name.Equals(c.Name)).Item2);
                        Debug.WriteLine(c.Name + " - dist : " + dist + " - minDiff : " + minDiff);
                        if (dist < minDiff)
                        {
                            minDiff = dist;
                            minDiffClub = c;
                        }
                    }
                }
                  return minDiffClub;
            */
            Club minDiffClub = null;
            double minDiff = -1;
            if (dUserTarget < 6.0)//if the target isn't further than 6 meters, let's advise the user to use his putter
            {
                minDiffClub = Club.PUTTER;
            }
            else
            {
                //Find the lowest absolute difference between all clubs average distances and the distance between the user and the target
                foreach (Tuple<Club, double> tuple in listAverage)
                {
                    Club c = tuple.Item1;
                    if (!c.Equals(Club.PUTTER))
                    {
                        if (minDiffClub == null)
                        {
                            minDiffClub = c;
                            minDiff = Math.Abs(dUserTarget - tuple.Item2);
                        }
                        else
                        {
                            double distanceC = tuple.Item2;
                            double distDiff = Math.Abs(dUserTarget - distanceC);
                            if (distDiff < minDiff)
                            {
                                minDiffClub = c;
                                minDiff = distDiff;
                            }
                        }
                    }
                }
            }

            return minDiffClub;

        }
    }


}
