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
    // Functie: Het weergeven van het analysescherm.
    public class Analyzeinterface : Activity
    {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Button knoppie = new Button(this);

            // Laad de doorgegeven informatie (track, naam en datum) in.
            string trackstring = this.Intent.GetStringExtra("trackstring");
            string timestring = this.Intent.GetStringExtra("timestring");
            string name = this.Intent.GetStringExtra("name");
            
            if (trackstring == null)
            {
                // Huh: geen informatie.
                knoppie.Text = "ERROR";
                this.SetContentView(knoppie);
            }
            else {
                // Decodeer de track
                List<knooppunt> track = TrackAnalyzer.String_Trackify(trackstring);
                string knoptekst = "";

                // Geef informatie erover weer. Joost, hier mag jij los! Ik zou het natuurlijk niet meer op een knoppie weergeven straks :P
                //knoptekst += "Total distance: "  + (TrackAnalyzer.Track_Total_Distance(track) / 1000f).ToString();
                //knoptekst = TrackAnalyzer.Track_Debugstring(track);
               // knoptekst += "\r\n " + TrackAnalyzer.Track_Share_String(track);
               
                graphview grafiek = new Kaart.graphview(this);
                grafiek.Set_Axis_List_One(TrackAnalyzer.Track_List_Speed_OverTime(track, true));
                grafiek.Set_Axis_List_Two(TrackAnalyzer.Track_List_Speed_OverTime(track, false));
                this.SetContentView(grafiek);
            }


        }


    }


}

