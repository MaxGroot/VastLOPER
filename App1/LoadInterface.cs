using Android.App;
using Android.Widget;
using Android.OS;
using Android.Hardware;
using Android.Graphics;
using Android.Content;
using System;
using Android.Views;
using Android.Locations;

namespace Kaart
{
    [Activity(Label = "LoadInterface")]
    public class LoadInterface : Activity
    {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Button knoppie = new Button(this);
            knoppie.Text = "WORK IN PROGRESS";
            this.SetContentView(knoppie);

        }
        

    }


}

