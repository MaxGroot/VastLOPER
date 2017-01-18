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
    public class LoadInterface : Activity
    {
        List<TrackInfo> paden;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            ListView padendisplay = new ListView(this);

            paden = new List<TrackInfo>();
            saveload saver = new saveload();
            paden = saver.load_track();
            
            List<String> padennamen = new List<String>();
            foreach (TrackInfo pad in paden) {

                String tekst = "\r\n";
                tekst += pad.name + " - " + pad.timedate.ToString("dd-MM-yyyy");

                padennamen.Add(tekst);


            }

            ArrayAdapter<string> adaptertje = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItemActivated1, padennamen);
            padendisplay.Adapter = adaptertje;
            padendisplay.ChoiceMode = ChoiceMode.Single;
            padendisplay.ItemClick += GaLaden;

        
            this.SetContentView(padendisplay);

        }

        private void GaLaden(object o, AdapterView.ItemClickEventArgs e) {
            int positie = e.Position;
            TrackInfo gekozenpad = paden[positie];
           

            
            Intent i;
            i = new Intent(this, typeof(Analyzeinterface));

            i.PutExtra("trackstring", gekozenpad.trackstring);
            i.PutExtra("timestring", gekozenpad.timedate.ToString("dd-MM-yyyy"));
            i.PutExtra("name", gekozenpad.name);

            this.StartActivity(i);


        }

    }


}

