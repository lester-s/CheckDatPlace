using Android.App;
using Android.OS;

namespace CheckDatPlace
{
    [Activity(Label = "Check Dat Place", MainLauncher = false, Icon = "@drawable/CDPIcon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
        }
    }
}