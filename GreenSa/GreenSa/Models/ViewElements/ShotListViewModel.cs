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

    public class ShotListViewModel : BaseViewModel
    {

        public ObservableCollection<Tuple<Shot, IEnumerable<Club>>> Shots { get; set; }

        public ShotListViewModel(ObservableCollection<Tuple<Shot, IEnumerable<Club>>> res)
        {
            this.Shots = new ObservableCollection<Tuple<Shot, IEnumerable<Club>>>(res);
        }

        public Command<Tuple<Shot, IEnumerable<Club>>> RemoveShot
        {
            get
            {
                return new Command<Tuple<Shot, IEnumerable<Club>>>((shot) => {
                    Shots.Remove(Shots.ToList().Find(s => s.Item1.Equals(shot)));
                });
            }
        }
    }
}
