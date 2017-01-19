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
    // Functie: Dit is de interface die uiteindelijk de kaartdetview op het scherm zet, samen met de knoppen eromheen.
    [Activity(Theme = "@android:style/Theme.NoTitleBar",  Label = "vastLOPER")]
    public class KaartInterface : Activity
    {
        KaartDetView info;
        Button startstopknop;
        EditText inputvakje;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Dit inputvakje hebben we nodig voor het naam-geven van een track.
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


            // Handlers voor klikken op bovenknoppen
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
        // Delen van een track.
        public void shareTrack(object o, EventArgs ea) {

            string bericht = info.TrackText(); 
            Intent i = new Intent(Intent.ActionSend);
            i.SetType("text/plain");
            i.PutExtra(Intent.ExtraText, bericht);
            this.StartActivity(i);

        }

        // Zorgt ervoor dat de startstopknop de juiste tekst weergeeft bij starten en stoppen
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

        // Analyse knop: Stuur de track en haar info door naar de analyzeinterface
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

        // Geef de gebruiker een mogelijk om zijn track een naam te geven. Gebeurt dit, verwijs dan door naar de functie SaveConfirmed
        public void SavePressed(object o, EventArgs ea) {
            // Bouw de alert en tovert m op het scherm!
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

        // De gebruiker wil door met opslaan. Laten we dat doen!
        public void SaveConfirmed(object o, EventArgs ea) {
            List<float[]> track = info.trackpoints;
            // Coderen..
            string trackstring = TrackAnalyzer.Track_Stringify(track);
            string trackname = inputvakje.Text;
            
            // Maak een saveload instantie...
            saveload saver = new saveload();
            float pauzetijd = info.pauseseconds; 
            // En sla de track op!
            saver.save_track(trackstring, DateTime.Now, trackname , pauzetijd);
            Toast.MakeText(this, trackname + " opgeslagen. ", ToastLength.Short).Show();


        }

    }

    
}

