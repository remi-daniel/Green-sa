using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using CoreGraphics;
using CoreLocation;
using Foundation;
using GreenSa.iOS;
using GreenSa.Models.GolfModel;
using GreenSa.Models.Tools.GPS_Maps;
using GreenSa.ViewController.Play.Game;
using MapKit;
using ObjCRuntime;
using UIKit;
using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace GreenSa.iOS
{
    public class CustomMapRenderer :  ViewRenderer
    {
        CLLocationManager _locationManager;
        bool _shouldUpdateRegion;
        object _lastTouchedView;
        bool _disposed;

        MKPolylineRenderer polylineRenderer;
        private CustomMap formsMap;
        private MKCircleRenderer circleRenderer;
        private MKCircle circleOver;
        private MKCircle circleOverMin;
        private MKCircle circleOverMax;

        public CustomMapRenderer( ) : base()
        {
            //message from the CustomMap
            MessagingCenter.Subscribe<CustomMap>(this, "updateTheMap", (sender) => {
                try
                {
                    UpdatePolyLinePos();
                }
                catch (Exception e) { }
            });

            /*MessagingCenter.Subscribe<Partie>(this, "updateTheCircle", (sender) => {
                try
                {
                    updateCircle(sender.CurrentClub.DistanceMoyenneJoueur);
                }
                catch (Exception e) { }
            });

            MessagingCenter.Subscribe<MainGamePage, bool>(this, "updateTheCircleVisbility", (sender, visible) => {
                try
                {
                    setCircleVisible(visible);

                }
                catch (Exception e) { }
            });*/
        }
        

        private void setCircleVisible(bool visible)
        {
            var nativeMap = Control as MKMapView;
            if (!visible)
            {
                nativeMap.RemoveOverlay(circleOver);
                nativeMap.RemoveOverlay(circleOverMin);
                nativeMap.RemoveOverlay(circleOverMax);

            }
            else
            {
                nativeMap.OverlayRenderer = GetOverlayRendererCircle;
                nativeMap.AddOverlay(circleOver);
                nativeMap.OverlayRenderer = GetOverlayRendererCircleMax;
                nativeMap.AddOverlay(circleOverMax);
                nativeMap.OverlayRenderer = GetOverlayRendererCircleMin;
                nativeMap.AddOverlay(circleOverMin);
            }
        }

        private void updateCircle(Tuple<int, int, int> distanceMoyenneJoueur)
        {

            List<Position> r = ((CustomMap)this.Element).RouteCoordinates;
            if (r.Count != 0)
            {
                var nativeMap = Control as MKMapView;
                nativeMap.OverlayRenderer = GetOverlayRendererCircle;

                //MKCircle mkcircle = null;
                if (nativeMap.Overlays != null)
                foreach (IMKOverlay elem in nativeMap.Overlays)
                    if (elem is MKCircle)
                    {
                        nativeMap.RemoveOverlay(elem as MKCircle);
                    }

                //if (mkcircle != null)
                   
                circleOver = MKCircle.Circle(new CLLocationCoordinate2D(r[0].Latitude, r[0].Longitude), distanceMoyenneJoueur.Item1);
                nativeMap.AddOverlay(circleOver);

                nativeMap.OverlayRenderer = GetOverlayRendererCircleMax;

                circleOverMax = MKCircle.Circle(new CLLocationCoordinate2D(r[0].Latitude, r[0].Longitude), distanceMoyenneJoueur.Item3);
                nativeMap.AddOverlay(circleOverMax);

                nativeMap.OverlayRenderer = GetOverlayRendererCircleMin;

                circleOverMin = MKCircle.Circle(new CLLocationCoordinate2D(r[0].Latitude, r[0].Longitude), distanceMoyenneJoueur.Item2);
                nativeMap.AddOverlay(circleOverMin);

            }        
        }

        public void UpdatePolyLinePos()
        {
            var nativeMap = Control as MKMapView;
            nativeMap.OverlayRenderer = GetOverlayRenderer;

            if(nativeMap.Overlays!=null)
            foreach (IMKOverlay elem in nativeMap.Overlays)
                    if ( !(elem is MKCircle))
                        nativeMap.RemoveOverlay(elem);
                    
            CLLocationCoordinate2D[] coords = new CLLocationCoordinate2D[formsMap.RouteCoordinates.Count];
            int index = 0;
            foreach (var position in formsMap.RouteCoordinates)//48.0699815 ; -1.7472885
            {
                coords[index] = new CLLocationCoordinate2D(position.Latitude, position.Longitude);
                index++;
            }

            MKPolyline routeOverlay = MKPolyline.FromCoordinates(coords);
            nativeMap.AddOverlay(routeOverlay);
        }

        const string MoveMessageName = "MapMoveToRegion";

        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            return Control.GetSizeRequest(widthConstraint, heightConstraint);
        }

        // iOS 9/10 have some issues with releasing memory from map views; each one we create allocates
        // a bunch of memory we can never get back. Until that's fixed, we'll just reuse MKMapViews
        // as much as possible to prevent creating new ones and losing more memory

        // For the time being, we don't want ViewRenderer handling disposal of the MKMapView
        // if we're on iOS 9 or 10; during Dispose we'll be putting the MKMapView in a pool instead

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (disposing)
            {
                if (Element != null)
                {
                    var mapModel = (Map)Element;
                    MessagingCenter.Unsubscribe<Map, MapSpan>(this, MoveMessageName);
                    ((ObservableCollection<Pin>)mapModel.Pins).CollectionChanged -= OnCollectionChanged;
                }

                var mkMapView = (MKMapView)Control;
                mkMapView.RegionChanged -= MkMapViewOnRegionChanged;
                mkMapView.GetViewForAnnotation = null;
                if (mkMapView.Delegate != null)
                {
                    mkMapView.Delegate.Dispose();
                    mkMapView.Delegate = null;
                }
                mkMapView.RemoveFromSuperview();

                // For iOS versions < 9, the MKMapView will be disposed in ViewRenderer's Dispose method

                if (_locationManager != null)
                {
                    _locationManager.Dispose();
                    _locationManager = null;
                }

                _lastTouchedView = null;
            }

            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                var mapModel = (Map)e.OldElement;
                MessagingCenter.Unsubscribe<Map, MapSpan>(this, MoveMessageName);
                ((ObservableCollection<Pin>)mapModel.Pins).CollectionChanged -= OnCollectionChanged;
              
            }
            var nativeMap = Control as MKMapView;
          
            if (e.NewElement != null)
            {
                var mapModel = (Map)e.NewElement;

                if (Control == null)
                {
                    MKMapView mapView = null;

                    if (mapView == null)
                    {
                        // If this is iOS 8 or lower, or if there weren't any MKMapViews in the pool,
                        // create a new one
                        mapView = new MKMapView(RectangleF.Empty);
                    }

                    SetNativeControl(mapView);

                    mapView.GetViewForAnnotation = GetViewForAnnotation;
                    mapView.RegionChanged += MkMapViewOnRegionChanged;
                     
                  }

                MessagingCenter.Subscribe<Map, MapSpan>(this, MoveMessageName, (s, a) => MoveToRegion(a), mapModel);
                if (mapModel.LastMoveToRegion != null)
                    MoveToRegion(mapModel.LastMoveToRegion, false);

                UpdateMapType();
                UpdateIsShowingUser();
                UpdateHasScrollEnabled();
                UpdateHasZoomEnabled();

                ((ObservableCollection<Pin>)mapModel.Pins).CollectionChanged += OnCollectionChanged;

                OnCollectionChanged(((Map)Element).Pins, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            
            }
                //------------------------------

             formsMap = (CustomMap)e.NewElement;
            UpdatePolyLinePos();

            
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Map.MapTypeProperty.PropertyName)
                UpdateMapType();
            else if (e.PropertyName == Map.IsShowingUserProperty.PropertyName)
                UpdateIsShowingUser();
            else if (e.PropertyName == Map.HasScrollEnabledProperty.PropertyName)
                UpdateHasScrollEnabled();
            else if (e.PropertyName == Map.HasZoomEnabledProperty.PropertyName)
                UpdateHasZoomEnabled();
            else if (e.PropertyName == VisualElement.HeightProperty.PropertyName && ((Map)Element).LastMoveToRegion != null)
                _shouldUpdateRegion = true;
        }

#if __MOBILE__
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            UpdateRegion();
        }
#else
        public override void Layout()
        {
            base.Layout();
            UpdateRegion();
        }
#endif

        protected virtual IMKAnnotation CreateAnnotation(Pin pin)
        {
            var mk = new CustomMKAnnotation
            {
                Pin=pin,
                Title = pin.Label,
                Subtitle = pin.Address ?? "",
                CustomMapRenderer = this,
                Coordinate = new CLLocationCoordinate2D(pin.Position.Latitude, pin.Position.Longitude)
            };
            return mk;

        }

        protected virtual MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            MKPinAnnotationView mapPin = null;

            // https://bugzilla.xamarin.com/show_bug.cgi?id=26416
            var userLocationAnnotation = Runtime.GetNSObject(annotation.Handle) as MKUserLocation;
            if (userLocationAnnotation != null)
                return null;

            const string defaultPinId = "defaultPin";
            mapPin = mapView.DequeueReusableAnnotation(defaultPinId) as MKPinAnnotationView;
            if (mapPin == null)
            {
                mapPin = new MKPinAnnotationView(annotation, defaultPinId);
                mapPin.CanShowCallout = true;
            }

            mapPin.Annotation = annotation;
            AttachGestureToPin(mapPin, annotation);
            if (annotation is CustomMKAnnotation)
            {
                CustomMKAnnotation ann = (CustomMKAnnotation)annotation;

                if (!(ann.Pin is CustomPin))
                {
                    return mapPin;
                }

                CustomPin cpin = (CustomPin)ann.Pin;
                if (cpin.type == CustomPin.MOVABLE)
                {
                    mapPin.Draggable = true;
                    //mapPin.AccessibilityDragSourceDescriptors.
                mapPin.Image = UIImage.FromFile("shape_circle.png");
                    mapPin.CalloutOffset = new CGPoint(0, 0);
                    mapPin.PinColor = MKPinAnnotationColor.Green;

                }else if (cpin.type == CustomPin.HOLE)
                {
                    //marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.flag));
                    mapPin.Image = UIImage.FromFile("flag.png");
                    mapPin.CalloutOffset = new CGPoint(0, 0);

                }
                else if (cpin.type == CustomPin.HOLE)
                {
                }
                else if (cpin.type == CustomPin.USER)
                {
                    mapPin.PinColor = MKPinAnnotationColor.Purple;
                }
                else if (cpin.type == CustomPin.LOCKED)
                {
                    mapPin.PinColor = MKPinAnnotationColor.Purple;
                }

            }


            return mapPin;
        }

        protected void AttachGestureToPin(MKAnnotationView mapPin, IMKAnnotation annotation)
        {
            var recognizers = mapPin.GestureRecognizers;

            if (recognizers != null)
            {
                foreach (var r in recognizers)
                {
                    mapPin.RemoveGestureRecognizer(r);
                }
            }

#if __MOBILE__
            var recognizer = new UITapGestureRecognizer(g => OnClick(annotation, g))
            {
                ShouldReceiveTouch = (gestureRecognizer, touch) =>
                {
                    _lastTouchedView = touch.View;
                    return true;
                }
            };
#else
            var recognizer = new NSClickGestureRecognizer(g => OnClick(annotation, g));
#endif
            mapPin.AddGestureRecognizer(recognizer);
        }

#if __MOBILE__
        void OnClick(object annotationObject, UITapGestureRecognizer recognizer)
#else
        void OnClick(object annotationObject, NSClickGestureRecognizer recognizer)
#endif
        {
            // https://bugzilla.xamarin.com/show_bug.cgi?id=26416
            NSObject annotation = Runtime.GetNSObject(((IMKAnnotation)annotationObject).Handle);
            if (annotation == null)
                return;

            // lookup pin
            Pin targetPin = null;
            foreach (Pin pin in ((Map)Element).Pins)
            {
                object target = pin.Id;
                if (target != annotation)
                    continue;

                targetPin = pin;
                break;
            }

            // pin not found. Must have been activated outside of forms
            if (targetPin == null)
                return;

            // if the tap happened on the annotation view itself, skip because this is what happens when the callout is showing
            // when the callout is already visible the tap comes in on a different view
            if (_lastTouchedView is MKAnnotationView)
                return;

            targetPin.SendTap();
        }

        void UpdateRegion()
        {
            if (_shouldUpdateRegion)
            {
                MoveToRegion(((Map)Element).LastMoveToRegion, false);
                _shouldUpdateRegion = false;
            }
        }

        void AddPins(IList pins)
        {
            foreach (Pin pin in pins)
            {
                var annotation = CreateAnnotation(pin);
                pin.Id = annotation;
                ((MKMapView)Control).AddAnnotation(annotation);
            }
        }

        void MkMapViewOnRegionChanged(object sender, MKMapViewChangeEventArgs e)
        {
            if (Element == null)
                return;

            var mapModel = (Map)Element;
            var mkMapView = (MKMapView)Control;

            mapModel.SetVisibleRegion(new MapSpan(new Position(mkMapView.Region.Center.Latitude, mkMapView.Region.Center.Longitude), mkMapView.Region.Span.LatitudeDelta, mkMapView.Region.Span.LongitudeDelta));
        }

        void MoveToRegion(MapSpan mapSpan, bool animated = true)
        {
            Position center = mapSpan.Center;
            var mapRegion = new MKCoordinateRegion(new CLLocationCoordinate2D(center.Latitude, center.Longitude), new MKCoordinateSpan(mapSpan.LatitudeDegrees, mapSpan.LongitudeDegrees));
            ((MKMapView)Control).SetRegion(mapRegion, animated);
        }

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            switch (notifyCollectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddPins(notifyCollectionChangedEventArgs.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemovePins(notifyCollectionChangedEventArgs.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    RemovePins(notifyCollectionChangedEventArgs.OldItems);
                    AddPins(notifyCollectionChangedEventArgs.NewItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    var mapView = (MKMapView)Control;
                    if (mapView.Annotations?.Length > 0)
                        mapView.RemoveAnnotations(mapView.Annotations);
                    AddPins((IList)(Element as Map).Pins);
                    break;
                case NotifyCollectionChangedAction.Move:
                    //do nothing
                    break;
            }
        }

        void RemovePins(IList pins)
        {
            foreach (object pin in pins)
                ((MKMapView)Control).RemoveAnnotation((IMKAnnotation)((Pin)pin).Id);
        }

        void UpdateHasScrollEnabled()
        {
            ((MKMapView)Control).ScrollEnabled = ((Map)Element).HasScrollEnabled;
        }

        void UpdateHasZoomEnabled()
        {
            ((MKMapView)Control).ZoomEnabled = ((Map)Element).HasZoomEnabled;
        }

        void UpdateIsShowingUser()
        {

            ((MKMapView)Control).ShowsUserLocation = ((Map)Element).IsShowingUser;
        }

        void UpdateMapType()
        {
            switch (((Map)Element).MapType)
            {
                case MapType.Street:
                    ((MKMapView)Control).MapType = MKMapType.Standard;
                    break;
                case MapType.Satellite:
                    ((MKMapView)Control).MapType = MKMapType.Satellite;
                    break;
                case MapType.Hybrid:
                    ((MKMapView)Control).MapType = MKMapType.Hybrid;
                    break;
            }
        }



      
        MKOverlayRenderer GetOverlayRenderer(MKMapView mapView, IMKOverlay overlayWrapper)
        {
            //if (polylineRenderer == null && !Equals(overlayWrapper, null))
            //{
                var overlay = Runtime.GetNSObject(overlayWrapper.Handle) as IMKOverlay;
                polylineRenderer = new MKPolylineRenderer(overlay as MKPolyline)
                {
                    FillColor = UIColor.Blue,
                    StrokeColor = UIColor.Red,
                    LineWidth = 4f,
                    Alpha = 0.4f
                };
            //}

            return polylineRenderer;
        }

        MKOverlayRenderer GetOverlayRendererCircle(MKMapView mapView, IMKOverlay overlayWrapper)
        {//MOYENNE
            //if (polylineRenderer == null && !Equals(overlayWrapper, null))
            //{
            var overlay = Runtime.GetNSObject(overlayWrapper.Handle) as IMKOverlay;
            circleRenderer = new MKCircleRenderer(overlay as MKCircle)
            {
                FillColor = UIColor.FromRGBA(0,0,0,0),
                StrokeColor = UIColor.FromRGBA(250, 250, 250, 250),
                LineWidth=5f

            };
            //}

            return circleRenderer;
        }

        MKOverlayRenderer GetOverlayRendererCircleMin(MKMapView mapView, IMKOverlay overlayWrapper)
        {
            //if (polylineRenderer == null && !Equals(overlayWrapper, null))
            //{
            var overlay = Runtime.GetNSObject(overlayWrapper.Handle) as IMKOverlay;
            circleRenderer = new MKCircleRenderer(overlay as MKCircle)
            {
                FillColor = UIColor.Red,
                StrokeColor = UIColor.FromRGBA(200, 200, 30, 30),
                Alpha = 0.6f

            };
            //}

            return circleRenderer;
        }


        MKOverlayRenderer GetOverlayRendererCircleMax(MKMapView mapView, IMKOverlay overlayWrapper)
        {
            //if (polylineRenderer == null && !Equals(overlayWrapper, null))
            //{
            var overlay = Runtime.GetNSObject(overlayWrapper.Handle) as IMKOverlay;
            circleRenderer = new MKCircleRenderer(overlay as MKCircle)
            {
                FillColor = UIColor.Green,
                StrokeColor = UIColor.FromRGBA(230, 20, 170, 20),
                Alpha=0.5f

            };
            //}

            return circleRenderer;
        }

    }






}
