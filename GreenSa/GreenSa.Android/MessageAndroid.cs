using Android.App;
using Android.Widget;
using GreenSa.Droid;
using System.Collections;
using System.Runtime.Remoting.Messaging;

[assembly: Xamarin.Forms.Dependency(typeof(MessageAndroid))]
namespace GreenSa.Droid
{
    public class MessageAndroid : IMessage
    {
        public IDictionary Properties => throw new System.NotImplementedException();

        public void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }
    }
}