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
    public class Hole
    {
        public enum ScorePossible
        {
            ALBATROS = -3,EAGLE =-2,BIRDIE=-1,PAR=0,BOGEY=1,DOUBLE_BOUGEY=2,MORE=3
        }

        [PrimaryKey]
        public string Id
        {
            get
            {
                return Position.X + ":"+Position.Y+":"+Par;
            }
            set {
                string[] tab = value.Split(':');
                Position = new MyPosition(Double.Parse(tab[0]), Double.Parse(tab[1]));
                Par=int.Parse(tab[2]);
            }
        }

        [ForeignKey(typeof(GolfCourse))]
        public string IdGolfC { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public GolfCourse GolfCourse { get; set; }

        [ForeignKey(typeof(MyPosition))]
        public String IdPos { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public MyPosition Position { get; set; }

        public int Par { get; set; }

        public Hole()
        {

        }

        public Hole(MyPosition p, int par)
        {
            this.Position = p;
            this.Par = par;
        }

        public Hole(MyPosition p,int par,GolfCourse golfCourse)
        {
            this.Position = p;
            this.Par = par;
            GolfCourse = golfCourse;
        }

        public override string ToString()
        {
            return Id+"" ;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Hole))
                return false;
            Hole h = (Hole)obj;
            return h.Position.Equals(Position) && h.Par==Par;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
