using Android.App;
using Android.Widget;
using Android.OS;
using Android.Hardware;
using Android.Graphics;
using Android.Content;
using System;
using Android.Views;
using Android.Locations;
using System.Collections.Generic; 

namespace Kaart
{
    [Activity(Theme = "@android:style/Theme.NoTitleBar", Label = "vastLOPER")]
    public class Analyzeinterface : Activity
    {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Button knoppie = new Button(this);
            string trackstring = this.Intent.GetStringExtra("trackstring");
            string timestring = this.Intent.GetStringExtra("timestring");
            string name = this.Intent.GetStringExtra("name");

            if (trackstring == null)
            {
                knoppie.Text = "ERROR";
            }
            else {
                List<float[]> track = TrackAnalyzer.String_Trackify(trackstring);
                string knoptekst = "";

                knoptekst += "Total distance: "  + (TrackAnalyzer.Track_Total_Distance(track) / 1000f).ToString();

                knoppie.Text = knoptekst;

            }
            this.SetContentView(knoppie);

        }


    }


}

