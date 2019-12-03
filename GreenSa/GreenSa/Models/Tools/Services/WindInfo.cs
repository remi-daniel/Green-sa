using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GreenSa.Models.Tools.Services
{
    public class WindInfo 
    {
        //force du vent
        public double strength;

        public double direction;

        //le type sera peut-être à changer
        //il s'agit de l'image associé au sens du vent
        public ImageSource icon;

        public WindInfo(double strength, double direction, ImageSource icon)
        {
            this.strength = strength;
            this.direction = direction;
            this.icon = icon;
        }

     
    }
}
