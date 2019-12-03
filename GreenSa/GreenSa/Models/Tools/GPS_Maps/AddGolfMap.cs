using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
//using Xamarin.Forms.GoogleMaps;

namespace GreenSa.Models.Tools.GPS_Maps
{
    public class AddGolfMap : Map
    {

        public AddGolfMap() {
            
        }

        public void update()
        {
            this.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(47.364765, -1.915990), Distance.FromMiles(0.12)));
        }

    }
}
