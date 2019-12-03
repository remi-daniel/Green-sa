using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GreenSa.Models.ViewElements
{
    public class MySwitch : Switch
    {
        public MySwitch() : base()
        {
            this.OnColor = Color.FromHex("39B54A");
        }
    }
}
