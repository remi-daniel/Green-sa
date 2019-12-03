using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenSa.Models.GolfModel
{
    public class Club
    {

        public static Club PUTTER = new Club("Putter", 0);


        [PrimaryKey]
        public string Name { get; set; }

        public bool selected { get; set; }//Does the user have this club on his bag

        public int DistanceMoyenne { get; set; }

        public Club()
        {
            selected = true;
        }

        public Club(string name,int distMoy)
        {
            Name = name;
            selected = true;

            DistanceMoyenne = distMoy;
        }


        public Boolean IsPutter()
        {
            return this.Equals(Club.PUTTER);
        }

        public override string ToString()
        {
            return Name+"  dMoy = "+DistanceMoyenne;
        }

        public override bool Equals(object obj)
        {
            return obj is Club && ((Club)obj).Name==Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
    
}
