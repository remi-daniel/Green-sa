using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using static GreenSa.Models.Tools.GPS_Maps.CustomMap;

namespace GreenSa.Models.Tools.GPS_Maps
{
    public class AddGolfPin : Pin
    {
        public static string UPDATEDMESSAGE = "updatePosition";
        public static string UPDATEDMESSAGE_CIRCLE= "updatePositionWithCircle";


        public static string LOCKED = "NO_MOVABLE";
        public static string MOVABLE = "MOVABLE";
        public static string HOLE = "HOLE";
        public static string USER = "USER";

        public string type { get; set; }

        private AddGolfMap map;


        public AddGolfPin(string type) : base()
        {
            this.type = type;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

      


        /*public event PropertyChangedEventHandler PropertyChanged;


        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }*/
    }
}
