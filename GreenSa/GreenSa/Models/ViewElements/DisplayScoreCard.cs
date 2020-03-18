using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GreenSa.Models.GolfModel
{
    class DisplayScoreCard
    {
        public string number { get; set; }
        public string par { get; set; }
        public string putt { get; set; }
        public string penalities { get; set; }
        public string score { get; set; }

        public int firstFrameCornerRadius { get; set; }
        public Color firstFrameBackgroundColor { get; set; }
        public Color firstFrameBorderColor { get; set; }
        public int secondFrameCornerRadius { get; set; }
        public Color secondFrameBackgroundColor { get; set; }
        public Color secondFrameBorderColor { get; set; }
        public Color textColor { get; set; }
        public int firstFrameWidth { get; set; }
        public int secondFrameWidth { get; set; }

        public DisplayScoreCard(int i, ScoreHole sh)
        {
            number = i.ToString();
            par = sh.Hole.Par.ToString();
            putt = sh.NombrePutt.ToString();
            penalities = sh.Penality.ToString();
            score = (sh.Score + sh.Hole.Par).ToString();
            this.setScoreSymbol(sh.Score, sh.Hole.Par);
            secondFrameWidth = firstFrameWidth - 2;
        }

        private void setScoreSymbol(int score, int par)
        {
            var frameSize = 26;
            if (score+par >= 10)//if 2 digits
            {
                frameSize = 31;
            }
            switch (score)
            {
                case -2:
                    this.firstFrameCornerRadius = 45;
                    this.secondFrameCornerRadius = 45;
                    this.firstFrameBackgroundColor = Color.White;
                    this.secondFrameBackgroundColor = Color.White;
                    this.firstFrameBorderColor = Color.FromHex("#292727");
                    this.secondFrameBorderColor = Color.FromHex("#292727");
                    this.textColor = Color.Black;
                    this.firstFrameWidth = 28;
                    break;

                case -1:
                    this.firstFrameCornerRadius = 45;
                    this.secondFrameCornerRadius = 45;
                    this.firstFrameBackgroundColor = Color.White;
                    this.secondFrameBackgroundColor = Color.White;
                    this.firstFrameBorderColor = Color.FromHex("#292727");
                    this.secondFrameBorderColor = Color.White;
                    this.textColor = Color.Black;
                    this.firstFrameWidth = 28;
                    break;

                case 0:
                    this.firstFrameCornerRadius = 45;
                    this.secondFrameCornerRadius = 45;
                    this.firstFrameBackgroundColor = Color.White;
                    this.secondFrameBackgroundColor = Color.White;
                    this.firstFrameBorderColor = Color.White;
                    this.secondFrameBorderColor = Color.White;
                    this.textColor = Color.Black;
                    this.firstFrameWidth = 28;
                    break;

                case 1:
                    this.firstFrameCornerRadius = 0;
                    this.secondFrameCornerRadius = 0;
                    this.firstFrameBackgroundColor = Color.White;
                    this.secondFrameBackgroundColor = Color.White;
                    this.firstFrameBorderColor = Color.FromHex("#292727");
                    this.secondFrameBorderColor = Color.White;
                    this.textColor = Color.Black;
                    this.firstFrameWidth = 26;
                    break;

                case 2:
                    this.firstFrameCornerRadius = 0;
                    this.secondFrameCornerRadius = 0;
                    this.firstFrameBackgroundColor = Color.White;
                    this.secondFrameBackgroundColor = Color.White;
                    this.firstFrameBorderColor = Color.FromHex("#292727");
                    this.secondFrameBorderColor = Color.FromHex("#292727");
                    this.textColor = Color.Black;
                    this.firstFrameWidth = 26;
                    break;

                default:
                    this.firstFrameCornerRadius = 0;
                    this.secondFrameCornerRadius = 0;
                    this.firstFrameBackgroundColor = Color.FromHex("#292727");
                    this.secondFrameBackgroundColor = Color.FromHex("#292727");
                    this.firstFrameBorderColor = Color.FromHex("#292727");
                    this.secondFrameBorderColor = Color.FromHex("#292727");
                    this.textColor = Color.White;
                    this.firstFrameWidth = frameSize;
                    break;
            }
        }

        public DisplayScoreCard(int i, int p)
        {
            number = i.ToString();
            par = p.ToString();
            score = "-";
        }

        public override string ToString()
        {
            return number.ToString() + " : " + par.ToString() + " : " + score.ToString();
        }
    }
}
