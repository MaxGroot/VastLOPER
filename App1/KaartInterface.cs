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
    [Activity(Theme = "@android:style/Theme.NoTitleBar",  Label = "vastLOPER")]
    public class KaartInterface : Activity
    {
        KaartDetView info;
        Button startstopknop;
        EditText inputvakje;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
             inputvakje = new EditText(this);



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
            saveknop.Click += SavePressed;
            Button analyzeknop = new Button(this);
            analyzeknop.Text = "Analyseren";
            analyzeknop.Click += GotoAnalyze;
            Button shareknop = new Button(this);
            shareknop.Text = "Delen";
            shareknop.Click += shareTrack;

            


            LinearLayout onderknoppen = new LinearLayout(this);
            onderknoppen.Orientation = Orientation.Horizontal;
            onderknoppen.AddView(saveknop);
            onderknoppen.AddView(analyzeknop);
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

        public void GotoAnalyze(object o, EventArgs ea) {
            List<float[]> track = info.trackpoints;
            string trackstring = TrackAnalyzer.Track_Stringify(track);

            Intent i;
            i = new Intent(this, typeof(Analyzeinterface));
            i.PutExtra("trackstring", trackstring);
            i.PutExtra("name", "Naamloze tocht");
            i.PutExtra("timestring", DateTime.Now.ToString("dd-MM-yyyy"));

            this.StartActivity(i);

        }
        public void SavePressed(object o, EventArgs ea) {
            //set alert for executing the task
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle("Naam voor uw tocht");
            alert.SetView(inputvakje);

            alert.SetPositiveButton("Opslaan", SaveConfirmed);

            alert.SetNegativeButton("Annuleren", (senderAlert, args) => {
                Toast.MakeText(this, "Geannuleerd!", ToastLength.Short).Show();
            });


            Dialog dialog = alert.Create();
            dialog.Show();
        }
        public void SaveConfirmed(object o, EventArgs ea) {
            List<float[]> track = info.trackpoints;
            string trackstring = TrackAnalyzer.Track_Stringify(track);
            string trackname = inputvakje.Text;
            
            saveload saver = new saveload();

            saver.save_track(trackstring, DateTime.Now, trackname);
            Toast.MakeText(this, trackname + " opgeslagen. ", ToastLength.Short).Show();


        }

    }

    
}

