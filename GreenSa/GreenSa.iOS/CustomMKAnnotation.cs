using System;
using CoreLocation;
using Foundation;
using MapKit;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using GreenSa.Models.Tools.GPS_Maps;

namespace GreenSa.iOS
{
    public class CustomMKAnnotation : MKPointAnnotation
    {

        public Pin Pin{get;set;}
        public CustomMapRenderer CustomMapRenderer { get; set; }
        //[Export("setCoordinate:")]
        public override void SetCoordinate(CLLocationCoordinate2D value)
        {
            base.SetCoordinate(value);
            //send message to CustomMap AND  mainGamePage
            Pin.Position = new Position(value.Latitude, value.Longitude);
            MessagingCenter.Send<CustomPin>(Pin as CustomPin, CustomPin.UPDATEDMESSAGE);

            CustomMapRenderer.UpdatePolyLinePos();
        }
      
        /// <summary>
        /// HACK: There (still) seems to be a bug in Xamarin.iOS where <value>_original_setCoordinate:</value> is not exported. 
        /// </summary>
        /// <param name="value">Coordinate to set</param>
        [Export("_original_setCoordinate:")]
        public void SetCoordinateOriginal(CLLocationCoordinate2D value)
        {
            this.SetCoordinate(value);
        }


	}
}
