using GreenSa.Models.GolfModel;
using GreenSa.Models.Profiles;
using GreenSa.Persistence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;

namespace GreenSa.Models.Tools
{
    class GolfXMLReader
    {
        public static List<GolfCourse> getListGolfCourseFromXMLFiles()
        {
            List<GolfCourse> gfcs = new List<GolfCourse>();

            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(App)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("GreenSa.Ressources.GolfCourses.GolfCourses_Descriptor.xml");
            string text = "";
            using (var reader = new System.IO.StreamReader(stream))//lit fichier contenant la liste des golf
            {
                text = reader.ReadToEnd();
            }

            //XDocument xDocumentForListOfGolfCoursesFiles = XDocument.Load("GreenSa/Ressources/GolfCourses/GolfCourses_Descriptor.xml");
            XDocument xDocumentForListOfGolfCoursesFiles = XDocument.Load(GenerateStreamFromString(text));//parsing


            foreach (var node in xDocumentForListOfGolfCoursesFiles.Element("GolfCourses").Elements("GolfCourse"))//for each file golfCourse
            {
                stream = assembly.GetManifestResourceStream("GreenSa.Ressources.GolfCourses." + node.Value + ".xml");
                text = "";
                using (var reader = new System.IO.StreamReader(stream))//read the file
                {
                    text = reader.ReadToEnd();
                }
                
                gfcs.Add(getSingleGolfCourseFromText(text));
            }

            System.Diagnostics.Debug.WriteLine(gfcs.ToString());

            return gfcs;
        }

        public static GolfCourse getSingleGolfCourseFromText(String text)
        {
            XDocument golfC = XDocument.Load(GenerateStreamFromString(text));//xmlparser

            List<Hole> trous = new List<Hole>();
            var nodeGolfC = golfC.Element("GolfCourse");
            foreach (var trou in nodeGolfC.Element("Coordinates").Elements("Trou"))//get the list of all holes
            {
                MyPosition pos = new MyPosition(Double.Parse(trou.Element("lat").Value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture), Double.Parse(trou.Element("lng").Value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture));
                trous.Add(new Hole(pos, int.Parse(trou.Element("par").Value)));
            }
            GolfCourse gc = new GolfCourse(nodeGolfC.Element("Name").Value, nodeGolfC.Element("NomGolf").Value, trous);
           /* foreach (Hole h in trous)
            {
                h.GolfCourse = gc;
            }*/
            return gc;
        }

        public static List<Club> getListClubFromXMLFiles()
        {
            List<Club> clubs = new List<Club>();

            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(App)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("GreenSa.Ressources.Clubs.Clubs_Descriptor.xml");
            string text = "";
            using (var reader = new System.IO.StreamReader(stream))//lit fichier contenant la liste des clubs
            {
                text = reader.ReadToEnd();
            }

            //XDocument xDocumentForListOfGolfCoursesFiles = XDocument.Load("GreenSa/Ressources/GolfCourses/GolfCourses_Descriptor.xml");
            XDocument xDocumentForListOfGolfCoursesFiles = XDocument.Load(GenerateStreamFromString(text));//parsing

            SQLite.SQLiteConnection connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            List<Profil> profils = SQLiteNetExtensions.Extensions.ReadOperations.GetAllWithChildren<Profil>(connection);
            string userIndexScale = "0";
            if (profils.Count != 0) {
                int userIndex = (int)profils[0].Index;
                System.Diagnostics.Debug.WriteLine("userIndex : " + userIndex);
                if (userIndex >= 10 && userIndex < 20) {
                    userIndexScale = "10";
                } else if (userIndex >= 20 && userIndex < 30) {
                    userIndexScale = "20";
                } else if (userIndex >= 30 && userIndex < 40) {
                    userIndexScale = "30";
                } else if (userIndex >= 40) {
                    userIndexScale = "40";
                }
            } else {
                userIndexScale = "40";
            }
            

            foreach (var node in xDocumentForListOfGolfCoursesFiles.Element("Clubs").Elements("Club"))//for each file club
            {
                stream = assembly.GetManifestResourceStream("GreenSa.Ressources.Clubs." + node.Value + ".xml");
                text = "";
                using (var reader = new System.IO.StreamReader(stream))//read the file
                {
                    text = reader.ReadToEnd();
                }

                XDocument golfC = XDocument.Load(GenerateStreamFromString(text));//xmlparser

                var nodeGolfC = golfC.Element("Club");
                var userMoyDistance = 0;
                if(node.Value.Equals("Putter"))
                {
                   userMoyDistance = int.Parse(nodeGolfC.Elements("DistanceMoyenne").First().Value);
                } else
                {
                    userMoyDistance = int.Parse(nodeGolfC.Elements("DistanceMoyenne").First(dist => dist.FirstAttribute.Value.Equals(userIndexScale)).Value);
                }
                Club gc = new Club(nodeGolfC.Element("Name").Value, userMoyDistance);
                clubs.Add(gc);
            }

            System.Diagnostics.Debug.WriteLine(clubs.ToString());

            return clubs;
        }

        public static Club getClubFromName(string name)
        {
            Club res;

            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(App)).Assembly;

            SQLite.SQLiteConnection connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            List<Profil> profils = SQLiteNetExtensions.Extensions.ReadOperations.GetAllWithChildren<Profil>(connection);
            string userIndexScale = "0";
            if (profils.Count == 0)
            {
                int userIndex = (int)profils[0].Index;
                if (userIndex >= 10 && userIndex < 20)
                {
                    userIndexScale = "10";
                }
                else if (userIndex >= 20 && userIndex < 30)
                {
                    userIndexScale = "20";
                }
                else if (userIndex >= 30 && userIndex < 40)
                {
                    userIndexScale = "30";
                }
                else
                {
                    userIndexScale = "40";
                }
            }
            else
            {
                userIndexScale = "40";
            }


            var stream = assembly.GetManifestResourceStream("GreenSa.Ressources.Clubs." + name + ".xml");
            string text = "";
            using (var reader = new System.IO.StreamReader(stream))//read the file
            {
                text = reader.ReadToEnd();
            }

            XDocument golfC = XDocument.Load(GenerateStreamFromString(text));//xmlparser

            var nodeGolfC = golfC.Element("Club");

            var userMoyDistance = int.Parse(nodeGolfC.Elements("DistanceMoyenne").First(dist => dist.FirstAttribute.Value.Equals(userIndexScale)).Value);
            res = new Club(nodeGolfC.Element("Name").Value, userMoyDistance);

            return res;
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

    }
}
