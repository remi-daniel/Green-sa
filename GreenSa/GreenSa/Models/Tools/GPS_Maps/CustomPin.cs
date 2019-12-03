using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using static GreenSa.Models.Tools.GPS_Maps.CustomMap;

namespace GreenSa.Models.Tools.GPS_Maps
{
    public class CustomPin : Xamarin.Forms.Maps.Pin
    {
        public static string UPDATEDMESSAGE = "updatePosition";
        public static string UPDATEDMESSAGE_CIRCLE= "updatePositionWithCircle";


        public static string LOCKED = "NO_MOVABLE";
        public static string MOVABLE = "MOVABLE";
        public static string HOLE = "HOLE";
        public static string USER = "USER";

        public string type { get; set; }

        private CustomMap map;

        public new Position Position {
            get {return base.Position;}
            set {base.Position=value;
            }
        }

        public CustomPin(string type) : base()
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

    }
}
