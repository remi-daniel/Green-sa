using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GreenSa.Models.ViewElements
{
    public class MyButton : Button
    {
        public MyButton() : base()
        {
            this.BackgroundColor = Color.FromHex("39B54A");
            this.TextColor = Color.White;
            this.CornerRadius = 50;
        }
        public MyButton(Color bc, Double opacity) : base()
        {
            this.BackgroundColor = bc;
            this.Opacity = opacity;
            this.TextColor = Color.White;
            this.CornerRadius = 50;
        }
    }
}
