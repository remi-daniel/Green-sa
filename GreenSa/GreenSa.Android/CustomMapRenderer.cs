using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Maps;
using Android.Gms.Maps.Model;
using Android.Gms.Maps;
using static Android.Gms.Maps.GoogleMap;
using Xamarin.Forms;
using GreenSa.Models.Tools.GPS_Maps;
using Greensa.Droid;
using GreenSa.Droid;
using System.Collections.ObjectModel;
using GreenSa.Models.GolfModel;
using GreenSa.ViewController.Play.Game;
using Geodesy;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace Greensa.Droid
{
    public class CustomMapRenderer : MapRenderer
    {

        GoogleMap map;
        private Polyline targetLine;//the current polyline
        private List<Polyline> coneLines;
        private Circle circle;
        private Circle circleMin;
        private Circle circleMax;



        public CustomMapRenderer(Context context) : base(context){
            coneLines = new List<Polyline>();
            MessagingCenter.Subscribe<CustomMap>(this, "updateTheMap", (sender) => {
                try
                {
                    UpdatePolyLinePos(false);
                    //UpdateShotCone(Math.PI / 4);
                }
                catch(Exception e) { }
            });
            MessagingCenter.Subscribe<Partie>(this, "updateTheCircle", (sender) => {
                try
                {
                    updateCircle();
                }
                catch (Exception e) { }
            });

            MessagingCenter.Subscribe<MainGamePage, bool>(this, "updateTheCircleVisbility", (sender, visible) => {
                try
                {
                    setCircleVisible(visible);

                }
                catch (Exception e) { }
            });
        }
        

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null) { }

            if (e.NewElement != null)
            {
                ((MapView)Control).GetMapAsync(this);
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            base.OnElementPropertyChanged(sender, e);
            if (this.Element == null || this.Control == null)
                return;
        }


        protected override void OnMapReady(Android.Gms.Maps.GoogleMap map)
        {
            base.OnMapReady(map);
            this.map = map;
            map.SetOnMarkerDragListener(new markerListenerDrag(this));
            map.UiSettings.ZoomControlsEnabled = false;
            map.UiSettings.MyLocationButtonEnabled = false;
        }


        protected override MarkerOptions CreateMarker(Pin pin)
        {
            var marker = base.CreateMarker(pin);

            if (!( pin is CustomPin ))
            {
                return marker;
            }
            if (((CustomPin)(pin)).type == CustomPin.MOVABLE)
            {
                marker.Draggable(true);
                marker.SetRotation(30.5f);
                BitmapDescriptor ic = BitmapDescriptorFactory.FromResource(Resource.Drawable.Target);

                marker.SetIcon(ic);
                marker.Anchor(0.5f, 0.5f);
            }
            else if (((CustomPin)(pin)).type == CustomPin.HOLE)
            {
                marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.flag));
            }
            else if (((CustomPin)(pin)).type == CustomPin.USER)
            {

            }
            else if (((CustomPin)(pin)).type == CustomPin.LOCKED)
            {
                marker.SetRotation(30.5f);
                marker.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueCyan));
            }


            return marker;
        }

        public void UpdateShotCone(double angle)
        {
            if (coneLines.Count != 0)
            {
                foreach (Polyline line in coneLines)
                {
                    line.Remove();
                }
            }

            CustomMap customMap = (CustomMap)this.Element;
            GeodeticCalculator geoCalculator = new GeodeticCalculator();
            double distTarget = 0.0;
            LatLng userPos = new LatLng(customMap.UserPin.Position.Latitude, customMap.UserPin.Position.Longitude);

            if (customMap != null)
            {
                distTarget = customMap.getDistanceUserTarget();
                addConePolyline(angle, geoCalculator, customMap, userPos, distTarget);
                addConePolyline(-angle, geoCalculator, customMap, userPos, distTarget);
            }
        }

        private void addConePolyline(double angle, GeodeticCalculator geoCalculator, CustomMap customMap, LatLng userPos, double distTarget)
        {
            var polylineOptions = new PolylineOptions();
            polylineOptions.Clickable(true);
            polylineOptions.InvokeJointType(JointType.Round);
            polylineOptions.InvokeWidth(10f);
            polylineOptions.InvokeColor(0x664444FF);

            polylineOptions.Add(userPos);
            LatLng conePoint = movePoint(angle, customMap.UserPin.Position, customMap.TargetPin.Position);
            Console.WriteLine("conePoint dist = " + CustomMap.DistanceTo(customMap.UserPin.Position.Latitude, customMap.UserPin.Position.Longitude, conePoint.Latitude, conePoint.Longitude, "M"));
            polylineOptions.Add(conePoint);
            coneLines.Add(map.AddPolyline(polylineOptions));
        }

        private LatLng movePoint(double angle, Position rotationCenter, Position p)
        {
            double xU = p.Latitude - rotationCenter.Latitude;
            double yU = p.Longitude - rotationCenter.Longitude;
            double xV = Math.Cos(angle) * xU - Math.Sin(angle) * yU;
            double yV = Math.Sin(angle) * xU + Math.Cos(angle) * yU;
            LatLng res = new LatLng(rotationCenter.Latitude + xV, rotationCenter.Longitude + yV);
            return res;    
        }

        public void UpdatePolyLinePos(bool init,LatLng pos=null)
        {
            if (targetLine != null)
            {
                targetLine.Remove();
                targetLine.Dispose();           
            }
            var polylineOptions = new PolylineOptions();
            polylineOptions.Clickable(true);
            polylineOptions.InvokeJointType(JointType.Round);//don't see the difference
            polylineOptions.InvokeWidth(10f);
            polylineOptions.InvokeColor(0x664444FF);

            int i = 0;
            CustomMap customMap = (CustomMap)this.Element;
            if (customMap != null)
            {
                foreach (var position in customMap.RouteCoordinates)
                {
                    if (i == 1 && !init && pos != null)
                        polylineOptions.Add(pos);
                    else
                        polylineOptions.Add(new LatLng(position.Latitude, position.Longitude));

                    i++;
                }
                targetLine = map.AddPolyline(polylineOptions);
            }
        }

        public void updateCircle()
        {
            CustomMap customMap = (CustomMap)this.Element;

            if (customMap != null)
            {
                if (customMap.UserPin != null)
                {
                    if (circle != null)
                    {
                        circle.Remove();
                        circle.Dispose();
                    }
                    //moy
                    CircleOptions circleOptions = new CircleOptions();
                    circleOptions.InvokeCenter(new LatLng(customMap.UserPin.Position.Latitude, customMap.UserPin.Position.Longitude));
                    circleOptions.InvokeRadius(customMap.getDistanceUserTarget());
                    circleOptions.InvokeFillColor(Android.Graphics.Color.Argb(0, 0, 0, 0));
                    circleOptions.InvokeStrokeColor(Android.Graphics.Color.Argb(240, 250, 250, 250));
                    circleOptions.InvokeStrokeWidth(5f);

                    circle = map.AddCircle(circleOptions);
                }
            }
        }

        public void setCircleVisible(bool visible)
        {
            circle.Visible = visible;
        }
      }
}
