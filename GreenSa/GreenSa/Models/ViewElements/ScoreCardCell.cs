using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GreenSa.Models.ViewElements
{
    class ScoreCardCell : ViewCell
    {
        public ScoreCardCell()
        {
            Label left = new Label();
            Label mid = new Label();
            Label right = new Label();

            BoxView l = new BoxView();
            BoxView lb = new BoxView();
            BoxView m = new BoxView();
            BoxView r = new BoxView();

            var grid= new Grid();

            grid.ColumnSpacing = 0;
            grid.HeightRequest = 40;

            l.BackgroundColor = Color.FromHex("39B54A");
            l.CornerRadius = 100;
            l.HeightRequest = l.Width;
            lb.BackgroundColor = Color.FromRgba(0, 0, 0, 0.6);
            lb.Margin = new Thickness(20, 0, 0, 0);
            m.BackgroundColor = Color.FromRgba(0, 0, 0, 0.6);
            r.BackgroundColor = Color.FromRgba(0, 0, 0, 0.6);

            left.TextColor = Color.White;
            left.FontSize = 25;
            left.FontAttributes = FontAttributes.Bold;
            left.HorizontalOptions = LayoutOptions.Center;
            left.VerticalOptions = LayoutOptions.Center;

            mid.TextColor = Color.FromRgba(255, 255, 255, 0.6);
            mid.FontSize = 20;
            mid.FontAttributes = FontAttributes.None;
            mid.HorizontalOptions = LayoutOptions.Center;
            mid.VerticalOptions = LayoutOptions.Center;

            right.TextColor = Color.White;
            right.FontSize = 25;
            right.FontAttributes = FontAttributes.Bold;
            right.HorizontalOptions = LayoutOptions.Center;
            right.VerticalOptions = LayoutOptions.Center;


            left.SetBinding(Label.TextProperty, "number");
            mid.SetBinding(Label.TextProperty, "par");
            right.SetBinding(Label.TextProperty, "score");

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            grid.Children.Add(lb, 0, 0);
            grid.Children.Add(l, 0, 0);
            grid.Children.Add(m, 1, 0);
            grid.Children.Add(r, 2, 0);
            grid.Children.Add(left, 0, 0);
            grid.Children.Add(mid, 1, 0);
            grid.Children.Add(right, 2, 0);
            View = grid;
        }
    }
}
