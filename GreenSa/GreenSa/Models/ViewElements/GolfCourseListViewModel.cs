using GreenSa.Models.GolfModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GreenSa.Models.ViewElements
{

    public class GolfCourseListViewModel : BaseViewModel
    {

        public ObservableCollection<GolfCourse> GolfCourses { get; set; }

        public GolfCourseListViewModel(List<GolfCourse> res)
        {
            this.GolfCourses = new ObservableCollection<GolfCourse>(res);
        }

        public Command<GolfCourse> RemoveGolfCourse
        {
            get
            {
                return new Command<GolfCourse>((GolfCourse) => {
                    GolfCourses.Remove(GolfCourse);
                });
            }
        }
    }
}