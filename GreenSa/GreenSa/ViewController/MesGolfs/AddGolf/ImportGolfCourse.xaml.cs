using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using GreenSa.Models.Tools.GPS_Maps;
using Xamarin.Forms.Maps;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using GreenSa.Models.GolfModel;
using GreenSa.Models.Tools;
using GreenSa.Persistence;
using System.Reflection;
using SQLite;
using GreenSa.Models.Exceptions;
//using Xamarin.Forms.Maps;
//using Xamarin.Forms.GoogleMaps;

namespace GreenSa.ViewController.Option
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImportGolfCourse : ContentPage
    {

        //list of the current pins placed on the map
        List<Pin> pins;

        public ImportGolfCourse()
        {
            InitializeComponent();
            this.pins = new List<Pin>();

            //Init some content on the view
            this.validPar.BorderColor = Color.FromHex("0C5E11");
            this.validPar.BorderWidth = 2;
            this.deletePin.BorderColor = Color.FromHex("0C5E11");
            this.deletePin.BorderWidth = 2;
            this.deleteAllPins.BorderColor = Color.FromHex("0C5E11");
            this.deleteAllPins.BorderWidth = 2;
            this.localizeCity.BorderColor = Color.FromHex("0C5E11");
            this.localizeCity.BorderWidth = 2;
            this.SetParVisibility(false);
            this.SetCourseNameVisibility(false);
            map.update();

            //Suscribe to get a notification about the pin that was added on the map by the user
            MessagingCenter.Subscribe<Pin>(this, "getAddGolfMapPins", (pin) => {
                this.pins.Add(pin);
                this.SetParVisibility(true);
                this.SetCourseNameVisibility(false);
            });
        }

        /** 
         * This method is called when the button to localize on the map is pressed
         */
        private async void OnLocalizeClick(object sender, EventArgs e)
        {
            if ("".Equals(cityEntry.Text))
            {
                await this.DisplayAlert("Erreur", "Le champ spécifiant le nom du golf ne peut pas être vide", "ok");
            } else
            {
                var address = cityEntry.Text;
                var locations = await Geocoding.GetLocationsAsync(address);
                var location = locations?.FirstOrDefault();
                if (location != null)
                {
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(location.Latitude, location.Longitude), Distance.FromMiles(0.12)));
                }
            }
        }

        /**
         * This method is called when the button to create a new golf course is pressed
         */
        private async void OnCreateCourseClick(object sender, EventArgs e)
        {

            if (this.pins.Count == 9 || this.pins.Count == 18)
            {
                var confirmDelete = await this.DisplayAlert("Création du parcours", "Voulez vous créer ce parcours ?", "Oui", "Non");
                if (confirmDelete)
                {
                    try
                    {
                        String xmlGolfCourse = this.CreateXmlGolfCourse();
                        this.InsertGolfCourseBdd(xmlGolfCourse);
                    } catch (EmptyStringException emptyStrException)
                    {
                        await this.DisplayAlert("Erreur", "La communication avec la base de données a échoué", "Ok");
                    }
                    
                }
            }
            else
            {
                await this.DisplayAlert("Erreur", "Un parcours ne peut être que de 9 ou 18 trous", "Ok");
            }
        }

        /** 
         * This method is called when the button to delete the last placed hole pin is pressed
         */
        private async void OnDeletePinClick(object sender, EventArgs e)
        {
            if (this.pins.Any())
            {
                var confirmDelete = await this.DisplayAlert("Suppression du dernier trou", "Voulez vous supprimer le trou n°" + this.pins.Count + " ?", "Oui", "Non");
                if (confirmDelete)
                {
                    this.pins.RemoveAt(this.pins.Count - 1);//remove in the common list
                    MessagingCenter.Send<Object>(this, "deleteLastPin");
                    NinePinsCourseNameManagement();
                    this.SetParVisibility(false);
                }
            } else
            {
                await this.DisplayAlert("Erreur de suppression", "Aucun trou à supprimer", "Ok");
            }
        }

        /** 
         * This method is called when the button to delete all hole pins is pressed
         */
        private async void OnDeleteAllPinsClick(object sender, EventArgs e)
        {
            if (this.pins.Any())
            {
                var confirmDelete = await this.DisplayAlert("Suppression de tous les trous", "Voulez vous supprimer tous les trous ?", "Oui", "Non");
                if (confirmDelete)
                {
                    this.ManageAllPinsDelete();
                }
            }
            else
            {
                await this.DisplayAlert("Erreur de suppression", "Aucun trou à supprimer", "Ok");
            }
        }

        /** 
         * This method is called when the button to validate the hole is pressed
         */
        private void OnValidParClick(object sender, EventArgs e)
        {
            if ("".Equals(this.golfParEntry.Text))
            {
                this.DisplayAlert("Erreur", "Le champ spécifiant le par du trou ne peut pas être vide", "ok");
            }
            else
            {
                int par = Convert.ToInt32(this.golfParEntry.Text);
                this.pins[this.pins.Count - 1].Id = par;//Given that the pin ID isn't used, let's use it to store the par
                try
                {
                    MessagingCenter.Send<Object>(par, "validPar");
                }
                catch (TargetInvocationException exception)
                {
                    this.DisplayAlert("OnValidParClick", exception.InnerException.StackTrace, "ok");
                }
                this.SetParVisibility(false);
                if (this.pins.Count == 18)
                {
                    this.golfNameEntry.Text = "18 trous de ";
                    this.SetCourseNameVisibility(true);
                }
                else
                {
                    NinePinsCourseNameManagement();
                }
            }
        }

        /** 
         * Creates an xml string describing the golf course whose data are specified in this pane by the user
         * return this xml string
         */
        private String CreateXmlGolfCourse()
        {
            if ("".Equals(this.cityEntry.Text))
            {
                throw new EmptyStringException("Le champ spécifiant le nom du golf ne peut pas être vide");
            }
            else if ("".Equals(this.golfNameEntry.Text))
            {
                throw new EmptyStringException("Le champ spécifiant le nom de ce parcours ne peut pas être vide");
            }
            else
            {
                //see Ressources/GolfCourses for a more understandable patern
                StringBuilder xmlGolfCourse = new StringBuilder("<GolfCourse>");
                xmlGolfCourse.Append("<Name>" + this.golfNameEntry.Text + "</Name>");
                xmlGolfCourse.Append("<NbTrous>" + this.pins.Count + "</NbTrous>");
                xmlGolfCourse.Append("<NomGolf>" + this.cityEntry.Text + "</NomGolf>");
                xmlGolfCourse.Append("<Coordinates>");
                foreach (Pin hole in this.pins)
                {
                    xmlGolfCourse.Append("<Trou>");
                    xmlGolfCourse.Append("<par>" + hole.Id + "</par>");//hole.Id is used to store the hole's par
                    xmlGolfCourse.Append("<lat>" + hole.Position.Latitude + "</lat>");
                    xmlGolfCourse.Append("<lng>" + hole.Position.Longitude + "</lng>");
                    xmlGolfCourse.Append("</Trou>");
                }
                xmlGolfCourse.Append("</Coordinates>");
                xmlGolfCourse.Append("</GolfCourse>");
                return xmlGolfCourse.ToString();
            }

        }

        /** 
         * Inserts a golf course in the database from an xml string describing the golf course
         * xmlGolfCourse : the xml string describing the golf course
         */
        private void InsertGolfCourseBdd(String xmlGolfCourse)
        {
            GolfCourse gc;
            try
            {
                gc = GolfXMLReader.getSingleGolfCourseFromText(xmlGolfCourse);
                SQLite.SQLiteConnection connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                try
                {
                    connection.CreateTable<Hole>();
                    connection.CreateTable<MyPosition>();
                    connection.CreateTable<GolfCourse>();
                    connection.BeginTransaction();
                    SQLiteNetExtensions.Extensions.WriteOperations.InsertWithChildren(connection, gc, true);
                    connection.Commit();
                    this.DisplayAlert("Succès", "Le " + this.pins.Count + " trous : " + golfNameEntry.Text + " a été créé avec succès", "Continuer");
                    this.ManageAllPinsDelete();
                }
                catch (SQLiteException bddException)
                {
                    this.DisplayAlert("Erreur avec la base de donnée", bddException.Source + " : Ce nom de golf existe déjà ou une autre erreur inattendu s'est produite", "Ok");
                    connection.Rollback();
                }
            }
            catch (Exception xmlConversionException)
            {
                this.DisplayAlert("Erreur lors de la conversion XML -> GolfCourse", xmlConversionException.StackTrace, "Ok");
            }
        }

        /** 
         * Manages the delete of all the pins currently placed on the map
         */
        private void ManageAllPinsDelete()
        {
            this.pins.Clear();
            MessagingCenter.Send<Object>(this, "deleteAllPins");//send a message to delete all pins from the map
            this.SetCourseNameVisibility(false);
            this.SetParVisibility(false);
        }


        /** 
         * Sets the visibility to the golf course name input area
         * isVisible : true is it has to be visible, false otherwise
         */
        private void SetCourseNameVisibility(Boolean isVisible)
        {
            golfNameLabel.IsVisible = isVisible;
            golfNameEntry.IsVisible = isVisible;
        }


        /** 
         * Sets the visibility to the par validation input area
         * isVisible : true is it has to be visible, false otherwise
         */
        private void SetParVisibility(Boolean isVisible)
        {
            golfParLabel.Text = "Par du trou n°" + this.pins.Count + " :";
            golfParEntry.IsVisible = isVisible;
            golfParLabel.IsVisible = isVisible;
            validPar.IsVisible = isVisible;
        }

        /** 
         * Sets visible the golf course name input area if nine pins are placed on the map, not visible otherwise
         */
        private void NinePinsCourseNameManagement()
        {
            if (this.pins.Count == 9)
            {
                this.SetCourseNameVisibility(true);
                this.golfNameEntry.Text = "9 trous de ";
            }
            else
            {
                this.SetCourseNameVisibility(false);
            }
        }
    }
}