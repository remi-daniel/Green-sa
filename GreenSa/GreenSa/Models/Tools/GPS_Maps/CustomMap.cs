using GreenSa.Models.GolfModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace GreenSa.Models.Tools.GPS_Maps
{
    public class CustomMap : Map , INotifyPropertyChanged
    {
        private List<Position>   routeCoordinates = new List<Position>();

        //List of the significative points of the map (use to draw the blue polyline)
        public List<Position> RouteCoordinates
        {
            get {
                routeCoordinates = new List<Position>();
                routeCoordinates.Add(UserPin.Position);
                routeCoordinates.Add(TargetPin.Position);
                routeCoordinates.Add(HolePin.Position);
                return routeCoordinates; 
            }
            set
            {
                routeCoordinates = value;
                MessagingCenter.Send<CustomMap>(this, "updateTheMap");

            }
        }
        public List<CustomPin> CustomPins{
            get{
                List<CustomPin> l = new List<CustomPin>();
                l.Add(UserPin);
                l.Add(TargetPin);
                l.Add(HolePin);
                return l;
            }
            set{
                
            }
        }
        public CustomPin UserPin { get => userPin; set => userPin = value; }
        public CustomPin TargetPin { get => targetPin; set => targetPin = value; }
        public CustomPin HolePin { get => holePin; set => holePin = value; }

        private CustomPin userPin;
        private CustomPin targetPin;
        private CustomPin holePin;

        public CustomMap()
        {
            //message which come from the markerListenerDrag (android only, ios set direclty)
            //when the target pin is moved =>update the model position of the target
            MessagingCenter.Subscribe<CustomPin>(this, CustomPin.UPDATEDMESSAGE, (sender) => {
                TargetPin.Position = sender.Position;
            });

            RouteCoordinates = new List<Position>();
            Position pos = new Position(0, 0);
            UserPin = new CustomPin(CustomPin.USER)
            {
                Type = PinType.Place,
                Position = pos,
                Label = "Vous"
            };
            TargetPin = new CustomPin(CustomPin.MOVABLE)
            {
                Type = PinType.Place,
                Position = pos,
                Label = "Cible"
            };
            HolePin = new CustomPin(CustomPin.HOLE)
            {
                Type = PinType.Place,
                Position = pos,
                Label = "Trou"
            };
        }
        
        public CustomMap(MapSpan region) : base(region)
        {
            RouteCoordinates = new List<Position>();
            RouteCoordinates.Add(region.Center);
            RouteCoordinates.Add(region.Center);
            RouteCoordinates.Add(region.Center);
        }

        /**
         * Sets the user pin to the given position and update its label with the bumber of shots currently performed
         * pos : the new position
         * nbShot : the number of shots
         */
        public void setUserPosition(MyPosition pos, int nbShot)
        {
            UserPin.Position = new Position(pos.X, pos.Y);
            UserPin.Label = "Vous : Coup " + nbShot;
            TargetPin.Position = getDefaultTargetPosition();
            update();

        }

        /**
         * Set the hole pin to a new position and update its label with the par of the given hole
         * hole : the hole
         */
        public void setHolePosition(Hole hole)
        {
            HolePin.Position = new Position(hole.Position.X, hole.Position.Y);
            HolePin.Label = "Par "+hole.Par;
        }

        /**
         * Gets the default target position which is the middle of the user and the hole
         */
        public Position getDefaultTargetPosition()
        {
            return new Position((UserPin.Position.Latitude + HolePin.Position.Latitude )/2, (UserPin.Position.Longitude + HolePin.Position.Longitude)/2);
        }

        /**
         * Updates the map
         */
        public void update()
        {
            //updates pin position
            Pins.Clear();
            this.Pins.Add(UserPin);
            this.Pins.Add(TargetPin);
            this.Pins.Add(HolePin);

            //updates update routeCoordinates list
            var list = new List<Position>();
            list.Add(UserPin.Position);
            list.Add(TargetPin.Position);
            list.Add(HolePin.Position);
            this.RouteCoordinates = list;

            //moves the map so that the target pin is in the middle
            var dist = getDistanceUserHole();
            /* the radius is computed by linear regression so that the zoom is varying with shot distance: 
               x =  Dist  |  25   |  50   |  100  |  150  |  200  |  250  |  300  |  350  |  400  |  450  |  500  |	
               y = Radius | 0.020 | 0.030 | 0.055 | 0.073 | 0.095 | 0.105 | 0.123 | 0.128 | 0.140 | 0.145 | 0.150 |
             */
            var radius = dist * 0.000278 + 0.0265;
            this.MoveToRegion(MapSpan.FromCenterAndRadius(TargetPin.Position, Distance.FromMiles(radius)));
        }


        public double getDistanceUserHole()
        {
            return DistanceTo(UserPin.Position.Latitude, UserPin.Position.Longitude,
                                HolePin.Position.Latitude, HolePin.Position.Longitude,"M");
        }

        public double getDistanceTargetHole()
        {
            return DistanceTo(TargetPin.Position.Latitude, TargetPin.Position.Longitude,
                                HolePin.Position.Latitude, HolePin.Position.Longitude, "M");
        }

        public double getDistanceUserTarget()
        {
            return DistanceTo(UserPin.Position.Latitude, UserPin.Position.Longitude,
                                TargetPin.Position.Latitude, TargetPin.Position.Longitude, "M");
        }

        /**
         * Computes the distance between two positions in a specific unit
         * lat1 : the latitude of the first position
         * lon1 : the longitude of the first position
         * lat2 : the latitude of the second position
         * lon2 : the longitude of the second position
         * unit : K to gets kilometers
         *        M to gets meters
         *        N to gets nautic miles
         */
        public static double DistanceTo(double lat1, double lon1, double lat2, double lon2, string unit)
        {
            var rlat1 = Math.PI * lat1 / 180;
            var rlat2 = Math.PI * lat2 / 180;
            var rlon1 = Math.PI * lon1 / 180;
            var rlon2 = Math.PI * lon2 / 180;

            var theta = lon1 - lon2;
            var rtheta = Math.PI * theta / 180;

            var dist = Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) * Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            if (unit == "K") { dist = dist * 1.609344; }
            if (unit == "M") { dist = dist * 1.609344 * 1000; }
            if (unit == "N") { dist = dist * 0.8684; }
            return dist;
        }

        /**
         * Sets the target pin movable
         */
        internal void setTargetMovable()
        {

            TargetPin.type = CustomPin.MOVABLE;
            update();
        }

        /**
         * Locks the target pin
         */
        public void lockTarget()
        {
            TargetPin.type = CustomPin.LOCKED;
            update();
        }

        /**
         * Wraps the user pin postion in a MyPosition object
         */
        internal MyPosition getUserPosition()
        {
            return new MyPosition(userPin.Position.Latitude, userPin.Position.Longitude);
        }

        public new event PropertyChangedEventHandler PropertyChanged;

        protected override void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
