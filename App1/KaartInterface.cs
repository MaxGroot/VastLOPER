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
    [Activity(Theme = "@android:style/Theme.NoTitleBar",  Label = "vastLOPER")]
    public class KaartInterface : Activity
    {
        KaartDetView info;
        Button startstopknop;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);



            // Bovenknoppen
            Button centreerknop = new Button(this);
            centreerknop.Text = "Centreer";

            startstopknop = new Button(this);
            startstopknop.Text = "Start";
            Button leegknop = new Button(this);
            leegknop.Text = "Leegmaken";


            LinearLayout bovenknoppen = new LinearLayout(this);
            bovenknoppen.Orientation = Orientation.Horizontal;
            bovenknoppen.AddView(centreerknop);
            bovenknoppen.AddView(startstopknop);
            bovenknoppen.AddView(leegknop);


            // Onderknoppen 

            Button saveknop = new Button(this);
            saveknop.Text = "Opslaan";
            Button loadknop = new Button(this);
            loadknop.Text = "Laden";
            Button shareknop = new Button(this);
            shareknop.Text = "Delen";
            shareknop.Click += shareTrack;


            LinearLayout onderknoppen = new LinearLayout(this);
            onderknoppen.Orientation = Orientation.Horizontal;
            onderknoppen.AddView(saveknop);
            onderknoppen.AddView(loadknop);
            onderknoppen.AddView(shareknop);




            // Kaart
             info = new Kaart.KaartDetView(this);


            // Handlers voor klikken op knoppen
            centreerknop.Click += info.Centreer;
            leegknop.Click += info.Schoon;
            startstopknop.Click += info.Start;
            startstopknop.Click += this.SetStartButton;

            // Stapel bovenstaande views op elkaar en zet ze op het scherm. 
            LinearLayout viewstapel = new LinearLayout(this);
            viewstapel.Orientation = Orientation.Vertical;
            


            viewstapel.AddView(bovenknoppen);
            viewstapel.AddView(onderknoppen);
            viewstapel.AddView(info);

            this.SetContentView(viewstapel);

        }

        public void shareTrack(object o, EventArgs ea) {

            string bericht = info.TrackText(); 
            Intent i = new Intent(Intent.ActionSend);
            i.SetType("text/plain");
            i.PutExtra(Intent.ExtraText, bericht);
            this.StartActivity(i);

        }

        public void SetStartButton(object o, EventArgs ea) {
            bool running = info.log;
            if (running)
            {
                startstopknop.Text = "Stop";
            }
            else {
                startstopknop.Text = "Start";
            }
        }
        
    }

    
}

