using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenSa.Models.Tools
{
    public class MyPosition
    {
        [PrimaryKey]
        public String IdPos
        {
            get
            { return X + ";" + Y; }
            set {
                string[] splited = value.Split(';');
                X = Double.Parse(splited[0]);
                Y = Double.Parse(splited[1]);
            }
        }
        public Double X { get; set; }
        public Double Y { get; set; }
        public MyPosition() { }

        public MyPosition(Double x, Double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return " ("+X+";"+Y+")";
        }

        public override bool Equals(object obj)
        {
            return obj is MyPosition && ((MyPosition)obj).X == X && ((MyPosition)obj).Y==Y;
        }
    }
}
