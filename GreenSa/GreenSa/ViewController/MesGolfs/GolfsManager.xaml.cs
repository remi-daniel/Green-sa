using GreenSa.Models.GolfModel;
using GreenSa.Models.ViewElements;
using GreenSa.Persistence;
using GreenSa.ViewController.Option;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GreenSa.ViewController.MesGolfs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GolfsManager : ContentPage
    {

        public GolfCourseListViewModel gclvm;

        public GolfsManager()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            List<GolfCourse> res = await GestionGolfs.getListGolfsAsync(null);
            //Update the list of golf courses
            gclvm = new GolfCourseListViewModel(res);
            BindingContext = gclvm;
        }

        /**
         * This method is called when tho button to add a new golf course is clicked
         */
        async private void OnAddGolfClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new ImportGolfCourse());
            }
            catch (TargetInvocationException exception)
            {
                Debug.WriteLine(exception.StackTrace);
            }
        }

        /** 
         * Deletes a golf course from the ListView and from the database
         */
        private async void DeleteGolfCourse(object sender, EventArgs e)
        {
            var image = sender as Image;
            var tgr = image.GestureRecognizers[0] as TapGestureRecognizer;
            //for each line, the golf course name is stored in the cross image CommandParameter attribute to be able to identify an image to its golf course
            var name = tgr.CommandParameter.ToString(); 
            var confirmDelete = await this.DisplayAlert("Suppression d'un golf", "Voulez vous vraiment supprimer le golf : " + name + " ?", "Oui", "Non");
            if (confirmDelete)
            {
                //remove golf course cell from ListView
                var toDelete = image.BindingContext as GolfCourse;
                var vm = BindingContext as GolfCourseListViewModel;
                vm.RemoveGolfCourse.Execute(toDelete);

                SQLite.SQLiteConnection connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                try
                {
                    //remove golf course from database
                    connection.BeginTransaction();
                    connection.Delete<GolfCourse>(name);
                    connection.Commit();
                }
                catch (Exception bddException)
                {
                    await this.DisplayAlert("Erreur avec la base de donnée", bddException.StackTrace, "Ok");
                    connection.Rollback();
                }
            }
        }
    }
}