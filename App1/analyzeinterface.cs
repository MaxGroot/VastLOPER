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
        public int page , maxpages;
        public string name, timestring;
        Button vorige, volgende;
        LinearLayout stapel1, stapel2 , stapel3;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            page = 1; maxpages = 3;
            // Laad de doorgegeven informatie (track, naam en datum) in.
            string trackstring = this.Intent.GetStringExtra("trackstring");
             timestring = this.Intent.GetStringExtra("timestring");
             name = this.Intent.GetStringExtra("name");
            
            if (trackstring == null)
            {
                // Huh: geen informatie.
                
            }
            else {
                // Decodeer de track
                List<knooppunt> track = TrackAnalyzer.String_Trackify(trackstring);

                // Nu een typwerk van jawelste, alle views constructen die op alle drie de paginas zichtbaar zijn... Bleh    

                // De globale statistieken van pagina 1.
                TextView statistieken = new TextView(this); statistieken.TextSize = 18; statistieken.SetTextColor(Color.White);
                statistieken.Text = $"Totale afstand: {(TrackAnalyzer.Track_Total_Distance(track) / 1000).ToString("0.00")} kilometer \r\n";
                statistieken.Text += $"Gemiddelde snelheid, pauzes meegerekend: {TrackAnalyzer.Track_Average_Speed(track,true).ToString("0.0")} km/u \r\n";
                statistieken.Text += $"Gemiddelde snelheid, pauzes niet meegerekend: {TrackAnalyzer.Track_Average_Speed(track,false).ToString("0.0")} km/u \r\n \r\n";

                statistieken.Text += $"Totale pauzetijd: {TrackAnalyzer.Seconds_ToReadAble(TrackAnalyzer.Track_Total_PauseTime(track))}  \r\n";
                statistieken.Text += $"Totale rentijd: {TrackAnalyzer.Seconds_ToReadAble(TrackAnalyzer.Track_Total_Time_Running(track))}  \r\n";

                // De grafiek van snelheid over tijd van pagina 2.
                graphview grafiek = new Kaart.graphview(this);
                grafiek.Set_Axis_List_One(TrackAnalyzer.Track_List_Speed_OverTime(track, true));
                grafiek.Set_Axis_List_Two(TrackAnalyzer.Track_List_Speed_OverTime(track, false));
                grafiek.Axis_Name_One = "Snelheid (km/u)";
                grafiek.Axis_Name_Two = "Tijd";

                graphview grafiek2 = new Kaart.graphview(this);
                grafiek2.Set_Axis_List_One(TrackAnalyzer.Track_List_Distance_OverTime(track, true));
                grafiek2.Set_Axis_List_Two(TrackAnalyzer.Track_List_Distance_OverTime(track, false));
                grafiek2.Axis_Name_One = "Afstand (km)";
                grafiek2.Axis_Name_Two = "Tijds";


                // De grafiek van afstand over tijd van pagina 3.

               

                // De beschrijving van pagina 1
                TextView beschrijving1 = new TextView(this);
                beschrijving1.Text = "Globale statistieken:";
                beschrijving1.SetTextColor(Color.White);
                beschrijving1.TextSize = 16;
                beschrijving1.Gravity = GravityFlags.CenterHorizontal;

                // De beschrijving van pagina 2
                TextView beschrijving2 = new TextView(this);
                beschrijving2.Text = "Snelheid over tijd: ";
                beschrijving2.SetTextColor(Color.White);
                beschrijving2.TextSize = 16;
                beschrijving2.Gravity = GravityFlags.CenterHorizontal;

                // De beschrijving van pagina 3
                TextView beschrijving3 = new TextView(this);
                beschrijving3.Text = "Totale afgelegde afstand over tijd: ";
                beschrijving3.SetTextColor(Color.White);
                beschrijving3.TextSize = 16;
                beschrijving3.Gravity = GravityFlags.CenterHorizontal;
                

                // De stapels van pagina 1,2 en 3. 
                stapel1 = new LinearLayout(this);

                stapel1.Orientation = Orientation.Vertical;

                LinearLayout titel1 = new LinearLayout(this);
                stapel1.AddView(titellayout(titel1, 1));
                stapel1.AddView(knoppenlayout(this,1));
                stapel1.AddView(beschrijving1);
                stapel1.AddView(statistieken);

                stapel2 = new LinearLayout(this);
                stapel2.Orientation = Orientation.Vertical;
                LinearLayout titel2 = new LinearLayout(this);
                stapel2.AddView(titellayout(titel2, 2));
                stapel2.AddView(knoppenlayout(this, 2));
                stapel2.AddView(beschrijving2);
                stapel2.AddView(grafiek);

                stapel3 = new LinearLayout(this);
                stapel3.Orientation = Orientation.Vertical;
                LinearLayout titel3 = new LinearLayout(this);
                stapel3.AddView(titellayout(titel3, 3));
                stapel3.AddView(knoppenlayout(this, 3));
                stapel3.AddView(beschrijving3);
                stapel3.AddView(grafiek2);

                // We beginnen op pagina 1.
                this.SetContentView(stapel1);
            }


        }
        protected LinearLayout titellayout(LinearLayout apply , int currentpage) {
            LinearLayout ret = apply;
            ret.Orientation = Orientation.Vertical; 
            // De namen van de track op pagina 1,2 en 3
            TextView naam = new TextView(this);
            naam.Text = name + $" ({currentpage}/3)";
            naam.SetTextColor(Color.White);
            naam.TextSize = 40;
            naam.Gravity = GravityFlags.CenterHorizontal;

            // De datums op pagina 1,2 en 3
            TextView datum = new TextView(this);
            datum.Text = timestring;
            datum.SetTextColor(Color.White);
            datum.TextSize = 20;
            datum.Gravity = GravityFlags.CenterHorizontal;

            ret.AddView(naam);
            ret.AddView(datum);

            return ret;
        }
        protected LinearLayout knoppenlayout(Context context, int currentpage) {
            LinearLayout ret = new LinearLayout(context);
            ret.Orientation = Orientation.Horizontal;
            
            // Navigatieknoppies.
            vorige = new Button(this);
            vorige.Text = "Vorige";
            vorige.Gravity = GravityFlags.FillHorizontal;
            vorige.Click += naarvorige;

            volgende = new Button(this);
            volgende.Text = "Volgende";
            volgende.Gravity = GravityFlags.FillHorizontal;
            volgende.Click += naarvolgende;

            if (currentpage > 1)
            {
                ret.AddView(vorige);
            }
            else {
                
            }
            if (currentpage < maxpages) {
                ret.AddView(volgende);
            }

            return ret;
        }

        protected void naarvolgende(object o, EventArgs ea) {
            // Naar de volgende pagina
            page++;
            switch (page) {
                case 2: {
                        
                        this.SetContentView(stapel2);
                        break;
                    }
                case 3:
                    {
                        this.SetContentView(stapel3);
                        break;
                    }
            }

        }

        protected void naarvorige(object o, EventArgs ea) {
            // Naar de vorige pagina
            page--;
            switch (page) {
                case 1: {
                        this.SetContentView(stapel1);
                        break;
                    }
                case 2:
                    {
                        this.SetContentView(stapel2);
                        break;
                    }
            }
        }

    }


}

