using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace CheckDatPlace.CustomView
{
    public class ActionBarSpinner: Spinner
    {
        public ActionBarSpinner(Context context, IAttributeSet attrs): base(context, attrs)
        {
            
        }
    }
}