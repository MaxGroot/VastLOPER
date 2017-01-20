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
    [Activity(Theme = "@android:style/Theme.NoTitleBar", Label = "vastLOPER", MainLauncher = true, Icon = "@drawable/icon")]

    // Functie: het startscherm van de app.
    public class MainMenu : Activity
    {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            BitmapFactory.Options opt = new BitmapFactory.Options();
            opt.InScaled = false;
            Bitmap Plaatje = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.Icon, opt);

            ImageView plaatjeview = new ImageView(this);
            plaatjeview.SetImageBitmap(Plaatje);



            // Knoppen
            Button nieuw = new Button(this);
            nieuw.Text = "Nieuwe tocht";

            nieuw.OffsetTopAndBottom(30);
            nieuw.SetPadding(10, 10, 10, 10);

            Button tochtjes = new Button(this);
            tochtjes.Text = "Tocht laden";

            Button afsluiten = new Button(this);
            afsluiten.Text = "Afsluiten";



            // Linearlayout opbouwen
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
            TextView titel = new TextView(this);
            titel.SetTextColor(Color.White); titel.Text = "vastLOPER"; titel.TextSize = 40; titel.Gravity = GravityFlags.CenterHorizontal;
            TextView credits = new TextView(this);
            credits.SetTextColor(Color.White); credits.Text = "Max Groot en Joost Kwakkel, 2017"; titel.TextSize = 18;

            LinearLayout titelview = new LinearLayout(this);
            titelview.Orientation = Orientation.Vertical;
            titelview.AddView(titel);


            LinearLayout viewstapel = new LinearLayout(this);

            viewstapel.Orientation = Orientation.Vertical;

            viewstapel.AddView(plaatjeview);
            viewstapel.AddView(titelview);
            viewstapel.AddView(knoppen);
            viewstapel.AddView(credits);

            this.SetContentView(viewstapel);

        }

        // De gebruiker heeft start tocht aangeroepen
        public void startTocht(object o, EventArgs ea) {
            Intent i;
            i = new Intent(this,typeof(KaartInterface));
            this.StartActivity(i);

        }
        // De gebuiker heeft laden aangeroepen
        public void laadTocht(object o, EventArgs ea) {

            Intent i = new Intent(this, typeof(LoadInterface));
            this.StartActivity(i);

        }
        // De gebruiker heeft afsluiten aangeroepen.
        public void Afsluiten(object o, EventArgs ea) {
            System.Environment.Exit(0);
        }

    }


}

