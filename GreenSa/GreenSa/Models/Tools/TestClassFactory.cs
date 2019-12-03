using GreenSa.Models.Tools;
using GreenSa.Persistence;
using SQLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GreenSa.Models.GolfModel
{
    class TestClassFactory
    {
        private static int createdScorePartieCount = 0;

        public static T RandomEnumValue<T>()
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(new Random().Next(v.Length));
        }

        /**
         * Creates a filled ScorePartie and insert it recursivly in the database. 
         * Values used to create the statistics have not necessarily any sense
         */
        public static ScorePartie CreateScorePartie()
        {
            SQLite.SQLiteConnection connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            connection.CreateTable<GolfCourse>();
            connection.CreateTable<ScorePartie>();
            connection.CreateTable<ScoreHole>();
            connection.CreateTable<Shot>();
            connection.CreateTable<MyPosition>();
            List<GolfCourse> golfCourses = SQLiteNetExtensions.Extensions.ReadOperations.GetAllWithChildren<GolfCourse>(connection);
            Random r = new Random();
            var holes = golfCourses[r.Next()%(golfCourses.Count)].Holes;
            //var holes = golfCourses[0].Holes;
            //DateTime date = new DateTime(2019, DateTime.Now.Month, (TestClassFactory.createdScorePartieCount % 28) + 1);
            DateTime date = DateTime.Now;
            ScorePartie sp = new ScorePartie(date);
            List<Shot> shots = new List<Shot>();
            List<Club> clubs = SQLiteNetExtensions.Extensions.ReadOperations.GetAllWithChildren<Club>(connection);
            int i = 0;
            if (holes != null)
            {
                foreach (Hole hole in holes)
                {
                    int randPutt = r.Next() % 3 + 1;
                    int randScore = r.Next() % 9 - 3;
                    if (TestClassFactory.createdScorePartieCount == 0)
                    {
                        randScore = 10;
                    }
                    //int randScore = 1;
                    System.Diagnostics.Debug.WriteLine("randPutt = " + randPutt+ " randScore = "+ randScore+"\n");
                    for (int j = 0; j<randScore; ++j)
                    {
                        shots.Add(new Shot(clubs[2], RandomEnumValue<Shot.ShotCategory>(), DateTime.Now));
                    }
                    if (i == 7 && r.Next()%3 == 1)
                    {
                        randPutt = 0;
                    }
                    ScoreHole sh = new ScoreHole(hole, 0, randScore, randPutt == 2, randPutt, DateTime.Now);
                    sp.add(sh);
                    i++;
                }
            }
            System.Diagnostics.Debug.WriteLine("\n");
            sp.DateFin = DateTime.Now;
            try
            {
                SQLiteNetExtensions.Extensions.WriteOperations.InsertAllWithChildren(connection, shots, true);
                SQLiteNetExtensions.Extensions.WriteOperations.InsertAllWithChildren(connection, sp.scoreHoles, true);
                SQLiteNetExtensions.Extensions.WriteOperations.InsertWithChildren(connection, sp, true);
            } catch(SQLiteException sqlex)
            {
                System.Diagnostics.Debug.WriteLine(sqlex.StackTrace);
            }

            List<Shot> shotss = SQLiteNetExtensions.Extensions.ReadOperations.GetAllWithChildren<Shot>(connection);
            System.Diagnostics.Debug.WriteLine(shotss.Count);

            TestClassFactory.createdScorePartieCount++;
            return sp;
        }

    }
}