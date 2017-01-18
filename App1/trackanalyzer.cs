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
    class TrackAnalyzer
    {

        public static float PuntAfstand(PointF een, PointF twee)
        {
            // Returnt de afstand in meters tussen twee  RD punten.
            float afstandX = Math.Abs(een.X - twee.X);
            float afstandY = Math.Abs(een.Y - twee.Y);
            return afstandX + afstandY;
        }

        public static float PuntSnelheid(PointF een, PointF twee, float tijdverschil)
        {
            // Returnt de snelheid tussen twee punten in kilometers per uur.
            tijdverschil = Math.Abs(tijdverschil);

            float afstand = PuntAfstand(een, twee);

            return (afstand / tijdverschil) * 3.6f;


        }

        public static float Track_Total_Distance(List<float[]> track)
        {            // Returnt de totale afstand in meters.
            float totaldistance = 0f;
            PointF oudepunt = new PointF();
            PointF ditpunt = new PointF();
            int i = 0;
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

                oudepunt.X = punt[0]; oudepunt.Y = punt[1];

                i++;
            }

            return totaldistance;
        }

        public static float Track_Total_Time(List<float[]> track)
        {
            float[] eerstepunt = track[0];
            float[] laatstepunt = track[track.Count - 1];

            float verschilinseconden = laatstepunt[2] - eerstepunt[2];
            return verschilinseconden;
        }

        public static float Track_Total_Time_Running(List<float[]> track)
        {
            List<float> snelheden = new List<float>();
            int i = 0;
            float[] oudepunt = { };
            float[] nieuwepunt = { };

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

        public static float Track_Average_Speed(List<float[]> track, bool includepause)
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

        public static string Track_Debug_String(List<float[]> track)
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

        public static string Track_Share_String(List<float[]> track)
        {
            return Track_Stringify(track);
            return Track_Debug_String(track);


            float totaldistance = Track_Total_Distance(track) / 1000;
            float totaltime = Track_Total_Time(track);
            float totalrunningtime = Track_Total_Time_Running(track);

            float avgspeedincluded = Track_Average_Speed(track, true);
            float avgspeedexcluded = Track_Average_Speed(track, false);


            return $"Totale afstand: {totaldistance} kilometer \r\n Totale tijd: {totaltime} seconden \r\n Totale tijd rennend: {totalrunningtime} Gemiddelde snelheid, pauzes meegerekend: {avgspeedincluded} \r\n pauzes niet meegerekend: {avgspeedexcluded}";

        }

        public static string Track_Stringify(List<float[]> track) {
            string ret = "";
            int i = 0;
            foreach (float[] punt in track) {
                i++;
                string add = "";
                add += punt[0].ToString() + "?" + punt[1].ToString() + "?" + punt[2].ToString();
                if (i < track.Count)
                {
                    add += "|";
                }

                ret += add;
            }

            return ret;
        }

        
        public static List<float[]> String_Trackify(string trackstring) {
            List<float[]> track = new List<float[]>();
            string[] punten = trackstring.Split('|');
            
            foreach (string punt in punten) {
               
                string[] puntdata = punt.Split('?');
                int i = 0;
                float[] floatpunt = new float[3];
                foreach (string puntstuk in puntdata) {
                    floatpunt[i] = float.Parse(puntstuk);
                    
                    i++;
                }
                track.Add(floatpunt);
            }

            return track;


        }
        
    }

}