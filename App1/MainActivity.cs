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
    [Activity(Label = "Deze app LOOPT vast", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);



            // Knoppen
            Button centreerknop = new Button(this);
            centreerknop.Text = "Centreer";

            Button startstopknop = new Button(this);
            startstopknop.Text = "Start";
            Button leegknop = new Button(this);
            leegknop.Text = "Leegmaken";


            LinearLayout knoppen = new LinearLayout(this);
            knoppen.Orientation = Orientation.Horizontal;
            knoppen.AddView(centreerknop);
            knoppen.AddView(startstopknop);
            knoppen.AddView(leegknop);


            // Kaart
            KaartDetView info = new Kaart.KaartDetView(this);


            // Handlers voor klikken op knoppen
            centreerknop.Click += info.Centreer;
            leegknop.Click += info.Schoon;
            startstopknop.Click += info.Start;

            // Stapel bovenstaande views op elkaar en zet ze op het scherm. 
            LinearLayout viewstapel = new LinearLayout(this);
            viewstapel.Orientation = Orientation.Vertical;
            
            viewstapel.AddView(knoppen);
            viewstapel.AddView(info);
            this.SetContentView(viewstapel);

        }

  
        
    }

    
}

