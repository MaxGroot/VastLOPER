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
    [Activity(Theme = "@android:style/Theme.NoTitleBar", Label = "LoadInterface")]
    // Functie: Het weergeven van de te laden tracks. 
    public class LoadInterface : Activity
    {
        List<TrackInfo> paden;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            ListView padendisplay = new ListView(this);

            // Laad tracks in
            paden = new List<TrackInfo>();
            saveload saver = new saveload();
            paden = saver.load_track();

            // Zorgt ervoor dat we ipv oud naar nieuw van nieuw naar oud gaan.
            paden.Reverse();

            List<String> padennamen = new List<String>();

            // Zet alle trackinfo instanties in een lijst van strings.
            foreach (TrackInfo pad in paden) {

                String tekst = "\r\n";
                tekst += pad.name + " - " + pad.timedate.ToString("dd-MM-yyyy");

                padennamen.Add(tekst);


            }
            // Zet die lijst van strings in de listview

            ArrayAdapter<string> adaptertje = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItemActivated1, padennamen);
            padendisplay.Adapter = adaptertje;
            padendisplay.ChoiceMode = ChoiceMode.Single;
            padendisplay.ItemClick += GaLaden;

        
            this.SetContentView(padendisplay);

        }

        private void GaLaden(object o, AdapterView.ItemClickEventArgs e) {
            // Er is een track aangeklikt!

            // Zoek de trackinfo instantie die erbij hoort.
            int positie = e.Position;
            TrackInfo gekozenpad = paden[positie];
           

            // Info over de track in de intent stoppen..
            Intent i;
            i = new Intent(this, typeof(Analyzeinterface));

            i.PutExtra("trackstring", gekozenpad.trackstring);
            i.PutExtra("timestring", gekozenpad.timedate.ToString("dd-MM-yyyy"));
            i.PutExtra("name", gekozenpad.name);
            
            // En analyseren kan beginnen!
            this.StartActivity(i);


        }

    }


}

