using System;             // vanwege Math
using Android.Views;      // vanwege View, TouchEventArgs
using Android.Graphics;   // vanwege Paint, Canvas, PointF
using Android.Content;    // vanwege Context
using Android.Hardware;   // vanwege SensorManager, ISensorEventListener
using Android.Locations;  // vanwege Location, ILocationListener
using Android.OS;         // vanwege Bundle
using System.Collections.Generic;  // vanwege List

namespace Kaart
{
    // Deze class bevat functies voor het coderen, decoderen en analyzeren van tracks en knooppunten.
    class TrackAnalyzer
    {

        public static float PuntAfstand(PointF een, PointF twee) // Returnt de afstand in meters tussen twee  RD punten.

        {
            float afstandX = Math.Abs(een.X - twee.X);
            float afstandY = Math.Abs(een.Y - twee.Y);
            return afstandX + afstandY;
        }

        public static float PuntSnelheid(PointF een, PointF twee, float tijdverschil) // Returnt de snelheid tussen twee punten in kilometers per uur.

        {
            tijdverschil = Math.Abs(tijdverschil);

            float afstand = PuntAfstand(een, twee);

            // Keer 3.6 om van meters per seconde naar kilometers per uur te gaan
            return (afstand / tijdverschil) * 3.6f;


        }

        public static float Track_Total_Distance(List<float[]> track)           // Returnt de totale afstand in meters.


        {
            float totaldistance = 0f;
            PointF oudepunt = new PointF();
            PointF ditpunt = new PointF();
            int i = 0;

            // Alle punten aflopen en de afstand met het vorige punt optellen aan de totale afstand variabele (totaldistance)
            foreach (float[] punt in track)
            {
                if (i == 0)
                {
                    // Eerste punt. Die kunnen we niet vergelijken natuurlijk!

                }
                else
                {
                    // Tweede of later punt. Afstand vergelijken met vorige punt en dit optellen aan de oude afstand
                    ditpunt.X = punt[0]; ditpunt.Y = punt[1];
                    totaldistance += PuntAfstand(ditpunt, oudepunt);




                }

                // Oudepunt voor de volgende, is het huidige punt.
                oudepunt.X = punt[0]; oudepunt.Y = punt[1];

                i++;
            }

            return totaldistance;
        }

        
        public static float Track_Total_Time(List<float[]> track) // Returnt de totale tijd doorgebracht op de track, pauzes meegerekend
        {
            // Deze werking is incorrect!
            float[] eerstepunt = track[0];
            float[] laatstepunt = track[track.Count - 1];

            float verschilinseconden = laatstepunt[2] - eerstepunt[2];
            return verschilinseconden;
        }

        public static float Track_Total_Time_Running(List<float[]> track) // Returnt de totale tijd doorgebracht op de track, pauzes niet meegerekend
        {
            List<float> snelheden = new List<float>();
            int i = 0;
            float[] oudepunt = { };
            float[] nieuwepunt = { };

            // Alle punten afgaan, en de snelheid tov vorige punt toevoegen aan de list 'snelheden'.
            foreach (float[] punt in track)
            {
                nieuwepunt = punt;

               
                if (i == 0)
                {
                    // Eerstepunt, geen snelheidcalculatie mogelijk
                }
                else
                {
                    // Bereken snelheid en voeg toe aan de snelheden list
                    PointF een = new PointF(nieuwepunt[0], nieuwepunt[1]);
                    PointF twee = new PointF(oudepunt[0], oudepunt[1]);

                    float add = PuntSnelheid(een, twee, nieuwepunt[2] - oudepunt[2]);
                    snelheden.Add(add);

                }
                oudepunt = nieuwepunt;
                i++;

            }

            // Nu hebben we onze lijst met snelheden, nu gaan we het gemiddelde berekenen
            float totaltime = 0;
            for (i = 0; i < snelheden.Count; i++)
            {
                totaltime += snelheden[i];
            }
            return totaltime;

        }

        public static float Track_Average_Speed(List<float[]> track, bool includepause) // De gemiddelde snelheid op een track, bool geeft aan of pauzes meetellen voor gem. snelheid
        {
            float trackdistance = Track_Total_Distance(track);

            if (includepause)
            {
                // De pauzes tellen ook mee voor de gemiddelde snelheid. Dat betekent dat we gewoon het tijdsverschil van punt 1 en het laatste punt kunnen gebruiken!
                float verschilinseconden = Track_Total_Time(track);

                return (trackdistance / verschilinseconden) * 3.6f;

            }
            else
            {
                // Er moet een lijst gemaakt worden van alle snelheden die de gebruiker heeft gehad tijdens het lopen. Daarna moet de gemiddelde waarde in die lijst worden gereturned. 
                float totaltime = Track_Total_Time_Running(track);


                return (trackdistance / totaltime) * 3.6f;


            }

        }
        
        public static string Track_Debug_String(List<float[]> track) // Functie is obsolete en eigenlijk niet zo bruikbaar meer nu tracks naar strings omgezet kunnen worden
            
        {
            String ret = "";
            int i = 0;
            float[] oudepunt = { };
            float[] nieuwepunt = { };
            float speed;
            float distance;

            foreach (float[] punt in track)
            {
                nieuwepunt = punt;
                if (i > 0)

                {
                    PointF een = new PointF(nieuwepunt[0], nieuwepunt[1]);
                    PointF twee = new PointF(oudepunt[0], oudepunt[1]);
                    distance = PuntAfstand(een, twee) / 1000f;
                    speed = PuntSnelheid(een, twee, nieuwepunt[2] - oudepunt[2]);
                }
                else
                {
                    speed = 0;
                    distance = 0;
                }
                //ret += "(" + punt[0].ToString() + " , " + punt[1].ToString() + ") , " + punt[2] + " seconds since last start. \r\n"; 
                //ret +=" { (Speed: " + speed.ToString() + " km / u ) , Distance:  " + distance.ToString() + " kilometers } \r\n";

                ret += "faketrack[" + i.ToString() + "] = {" + punt[0].ToString() + "," + punt[1].ToString() + "," + punt[2].ToString() + "}";

                i++;
                oudepunt = nieuwepunt;
            }

            return ret;
        }

        // Returnt de string die gebruikers over hun track kunnen delen.
        public static string Track_Share_String(List<float[]> track)
        {

            // Voorlopig geeft hij de gecodeerde track terug ipv een mooie samenvatting, zo kunnen we 
            // een keer hardlopen en een mooie faketrack naar onszelf sturen
            return Track_Stringify(track);
            

            float totaldistance = Track_Total_Distance(track) / 1000;
            float totaltime = Track_Total_Time(track);
            float totalrunningtime = Track_Total_Time_Running(track);

            float avgspeedincluded = Track_Average_Speed(track, true);
            float avgspeedexcluded = Track_Average_Speed(track, false);


            return $"Totale afstand: {totaldistance} kilometer \r\n Totale tijd: {totaltime} seconden \r\n Totale tijd rennend: {totalrunningtime} Gemiddelde snelheid, pauzes meegerekend: {avgspeedincluded} \r\n pauzes niet meegerekend: {avgspeedexcluded}";

        }

        // Codeer een track naar een string
        public static string Track_Stringify(List<float[]> track) {
            string ret = "";
            int i = 0;
            // Voor alle punten..
            foreach (float[] punt in track) {
                i++;
                string add = "";

                // Formaat: x?y?tijd
                add += punt[0].ToString() + "?" + punt[1].ToString() + "?" + punt[2].ToString();

                // En als het niet het laatste punt is...
                if (i < track.Count)
                {
                    // Scheiden door een |
                    add += "|";
                }

                ret += add;
            }

            return ret;
        }
         
        // Codeer een string naar een track!
        public static List<float[]> String_Trackify(string trackstring) {
            List<float[]> track = new List<float[]>();
            // Splitst de string op in punten die we gaan analyseren
            string[] punten = trackstring.Split('|');
            
            // Voor al die punten..
            foreach (string punt in punten) {
               // Splits de punten in waarden die we kunnen toewijzen.
                string[] puntdata = punt.Split('?');
                int i = 0;
                float[] floatpunt = new float[3];
                // Laad de data van de string in de float
                foreach (string puntstuk in puntdata) {
                    floatpunt[i] = float.Parse(puntstuk);
                    
                    i++;
                }
                // En die float is een track knooppunt!
                track.Add(floatpunt);
            }

            return track;


        }
        
    }

}