using GreenSa.Models.Tools;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Windows.UI.Xaml.Media.Imaging;
using Xamarin.Forms;

namespace GreenSa.Models.Profiles
{
    public class Profil
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string Username { get; set; }
        //public Image Im { get; set; }
        public double Index { get; set; }
        [MaxLength(50)]
        public string GolfRef { get; set; }
        public int Photo { get; set; }
        public Boolean SaveStats { get; set; }

        public Profil()
        {
            Id = "localUser";
            Username = "Pseudo";
            //Im = new Image { Source = "basicProfile.png" };
            Index = 53.5;
            GolfRef = "Votre golf favori";
            Photo = 1;
            SaveStats = true;
        }
    }
}