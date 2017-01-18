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

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            ListView padendisplay = new ListView(this);

            List<TrackInfo> paden = new List<TrackInfo>();
            saveload saver = new saveload();
            paden = saver.load_track();


            Button knoppie = new Button(this);
            string tekst = "";

            List<String> padennamen = new List<String>();
            foreach (TrackInfo pad in paden) {

                tekst = "\r\n";
                tekst += pad.name + " - " + pad.timedate.ToString("dd-MM-yyyy");

                padennamen.Add(tekst);


            }

            ArrayAdapter<string> adaptertje = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItemChecked, padennamen);
            padendisplay.Adapter = adaptertje; 


            knoppie.Text = "KLIK MIJ";
            this.SetContentView(padendisplay);

        }
        

    }


}

