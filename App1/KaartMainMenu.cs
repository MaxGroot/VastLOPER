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
    [Activity(Theme = "@android:style/Theme.NoTitleBar", Label = "MainMenu", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainMenu : Activity
    {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);



            // Knoppen
            Button nieuw = new Button(this);
            nieuw.Text = "Nieuwe tocht";

            Button tochtjes = new Button(this);
            tochtjes.Text = "Tocht laden";

            Button afsluiten = new Button(this);
            afsluiten.Text = "Afsluiten";




            LinearLayout knoppen = new LinearLayout(this);
            knoppen.Orientation = Orientation.Vertical;
            knoppen.AddView(nieuw);
            knoppen.AddView(tochtjes);
            knoppen.AddView(afsluiten);



            // Handlers voor klikken op knoppen
            nieuw.Click += startTocht;
            tochtjes.Click += laadTocht;
            afsluiten.Click += Afsluiten; 

            // Stapel bovenstaande views op elkaar en zet ze op het scherm. 
            LinearLayout viewstapel = new LinearLayout(this);

            viewstapel.Orientation = Orientation.Vertical;
            
            viewstapel.AddView(knoppen);


            this.SetContentView(viewstapel);

        }


        public void startTocht(object o, EventArgs ea) {
            Intent i;
            i = new Intent(this,typeof(KaartInterface));
            this.StartActivity(i);

        }

        public void laadTocht(object o, EventArgs ea) {

            Intent i = new Intent(this, typeof(LoadInterface));
            this.StartActivity(i);

        }

        public void Afsluiten(object o, EventArgs ea) {
            System.Environment.Exit(0);
        }

    }


}

