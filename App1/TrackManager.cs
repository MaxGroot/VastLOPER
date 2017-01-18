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

    class TrackManager
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

        public static float Track_Average_Speed(List<float[]> track, bool includepause)
        {
            float trackdistance = Track_Total_Distance(track);

            if (includepause)
            {
                // De pauzes tellen ook mee voor de gemiddelde snelheid. Dat betekent dat we gewoon het tijdsverschil van punt 1 en het laatste punt kunnen gebruiken!
                float[] eerstepunt = track[0];
                float[] laatstepunt = track[track.Count - 1];

                float verschilinseconden = laatstepunt[2] - eerstepunt[2];


                return (trackdistance / verschilinseconden) * 3.6f;

            }
            else
            {
                // Er moet een lijst gemaakt worden van alle snelheden die de gebruiker heeft gehad tijdens het lopen. Daarna moet de gemiddelde waarde in die lijst worden gereturned. 
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
                return (trackdistance / totaltime) * 3.6f;


            }

        }

    }
}