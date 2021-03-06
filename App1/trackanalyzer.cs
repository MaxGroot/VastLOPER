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
    class knooppunt {
        public float x;
        public float y;
        public float timesincelastpause;
        public float timesincestart;
        public bool ispause;

        public knooppunt() {
            // Lege constructor
        }

        public knooppunt(float x, float y, float starttime , float pausetime, bool pause) {
            // Een echt knooppunt aanmaken
            this.x = x;
            this.y = y;
            this.timesincelastpause = pausetime;
            this.timesincestart = starttime;
            this.ispause = pause;
        }
    }

    class TrackAnalyzer
    {

        public static float PuntAfstand(knooppunt een, knooppunt twee) // Returnt de afstand in meters tussen twee  RD punten.

        {
            float afstandX = Math.Abs(een.x - twee.x);
            float afstandY = Math.Abs(een.y - twee.y);
            return afstandX + afstandY;
        }

        public static float PuntTijdVerschil(knooppunt een, knooppunt twee) {
            return Math.Abs(twee.timesincestart - een.timesincestart);
        }

        public static float PuntSnelheid(knooppunt een, knooppunt twee) // Returnt de snelheid tussen twee punten in kilometers per uur.

        {
            float tijdverschil = PuntTijdVerschil(een, twee);
            float afstand = PuntAfstand(een, twee);

            // Keer 3.6 om van meters per seconde naar kilometers per uur te gaan
            return (afstand / tijdverschil) * 3.6f;


        }

        public static float Track_Total_Distance(List<knooppunt> track)           // Returnt de totale afstand in meters.
        {
            float totaldistance = 0f;
            knooppunt oudepunt = new knooppunt();
            knooppunt nieuwepunt;
            int i = 0;

            // Alle punten aflopen en de afstand met het vorige punt optellen aan de totale afstand variabele (totaldistance)
            foreach (knooppunt punt in track)
            {
                nieuwepunt = punt;
                if (i == 0)
                {
                    // Eerste punt. Die kunnen we niet vergelijken natuurlijk!

                }
                else
                {
                    // Tweede of later punt. Afstand vergelijken met vorige punt en dit optellen aan de oude afstand
                    if (oudepunt.ispause)
                    {
                        // Helaas! De afstand afgelegd is de afstand sinds de pauze. deze telt NIET mee.
                    }
                    else {
                        if (nieuwepunt.ispause)
                        {
                            // Dit is een pauze-knooppunt. Het verschil tussen een pauze-knooppunt en het laatste loop-knooppunt daarvoor telt helaas niet mee.
                        }
                        else
                        {
                            // Hoppa! Een afstand tussen twee gelopen punten! Deze telt mee.
                            totaldistance += PuntAfstand(nieuwepunt, oudepunt);
                        }


                    }




                }

                // Oudepunt voor de volgende, is het huidige punt.
                oudepunt = nieuwepunt;
                i++;
            }

            return totaldistance;
        }

        
        public static float Track_Total_Time(List<knooppunt> track) // Returnt de totale tijd doorgebracht op de track, pauzes meegerekend
        {
            // Geef gewoon de tijd van het allerlaatste knooppunt tov de start!
            return (track[track.Count - 1]).timesincestart;
        }

        public static float Track_Total_PauseTime(List<knooppunt> track) {
            float totalpausetime = 0f;
            knooppunt oudepunt = new knooppunt();
            knooppunt nieuwepunt = new knooppunt();
            int i = 0;
            foreach (knooppunt p in track) {
                nieuwepunt = p;
                if (i > 0) {
                    // Niet het eerste punt dus!
                    if (oudepunt.ispause) {
                        // Hoppa! Het vorige punt was een pauze, wat het huidige punt automatisch een nieuw looppunt maakt. Het tijdsverschil tussen
                        // de twee is de pauzetijd!
                        totalpausetime += PuntTijdVerschil(nieuwepunt, oudepunt);
                    }


                }

                i++;
                oudepunt = nieuwepunt;

            }
            return totalpausetime;

        }

        public static float Track_Total_Time_Running(List<knooppunt> track) // Returnt de totale tijd doorgebracht op de track, pauzes niet meegerekend
        {
            float totaltime = 0f;
            int i = 0;
            knooppunt nieuwepunt = new Kaart.knooppunt();
            knooppunt oudepunt = new Kaart.knooppunt();
            
            // Alle punten afgaan, en de snelheid tov vorige punt toevoegen aan de list 'snelheden'.
            foreach (knooppunt punt in track)
            {
                nieuwepunt = punt;

               
                if (i == 0)
                {
                    // Eerste punt. We hoeven dus niet te checken of er niet een pauze voor zat.
                    totaltime += punt.timesincestart;
                }
                else
                {
                    // Kijk eerst of dit punt niet een pauze is.
                    if (punt.ispause)
                    {
                        // Helaas, pauzes tellen niet mee voor de totale rentijd!
                    }
                    else {
                        // Geldig looppunt, pak de tijd tov laatste punt.

                        if (oudepunt.ispause)
                        {
                            // Vorige punt was een pauze. Alleen de tijd pakken van het nieuwe punt tov pauze.
                            totaltime += nieuwepunt.timesincelastpause;
                        }
                        else
                        {
                            // Vorige punt was een looppunt. Tijdverschil pakken tussen de twee.
                            totaltime += nieuwepunt.timesincelastpause - oudepunt.timesincelastpause;
                        }
                    }

                }
                oudepunt = nieuwepunt;
                i++;

            }
            // Geef maar terug!
            return totaltime;

        }


        public static float Track_Average_Speed(List<knooppunt> track, bool includepause) // De gemiddelde snelheid op een track, bool geeft aan of pauzes meetellen voor gem. snelheid
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
        
        // Returnt de string die gebruikers over hun track kunnen delen.
        public static string Track_Share_String(List<knooppunt> track)
        {

            // Voorlopig geeft hij de gecodeerde track terug ipv een mooie samenvatting, zo kunnen we 
            // een keer hardlopen en een mooie faketrack naar onszelf sturen
            String sharetext = $"Totale afstand: {(TrackAnalyzer.Track_Total_Distance(track) / 1000).ToString("0.00")} kilometer \r\n";
            sharetext += $"Gemiddelde snelheid, pauzes meegerekend: {TrackAnalyzer.Track_Average_Speed(track, true).ToString("0.0")} km/u \r\n";
            sharetext += $"Gemiddelde snelheid, pauzes niet meegerekend: {TrackAnalyzer.Track_Average_Speed(track, false).ToString("0.0")} km/u \r\n \r\n";

            sharetext += $"Totale pauzetijd: {TrackAnalyzer.Seconds_ToReadAble(TrackAnalyzer.Track_Total_PauseTime(track))}  \r\n";
            sharetext += $"Totale rentijd: {TrackAnalyzer.Seconds_ToReadAble(TrackAnalyzer.Track_Total_Time_Running(track))}  \r\n";
            sharetext += $"Totale tochttijd: {TrackAnalyzer.Seconds_ToReadAble(TrackAnalyzer.Track_Total_Time(track))} \r\n \r\n";

            sharetext += $"Laagste snelheid: {graphview.List_Lowest_Not_Null(TrackAnalyzer.Track_List_Speed_OverTime(track, true))  } km/u \r\n";
            sharetext += $"Hoogste snelheid: {graphview.List_Highest(TrackAnalyzer.Track_List_Speed_OverTime(track, true))}";


            return sharetext; 
        }

        public static List<float> Track_List_Speed_OverTime(List<knooppunt> track, bool speeds)
        {
            List<float> ret = new List<float>();
            knooppunt oudepunt = new Kaart.knooppunt();
            knooppunt nieuwepunt = new Kaart.knooppunt();
            int i = 0;
            foreach (knooppunt p in track)
            {
                nieuwepunt = p;

                if (i > 0)
                {
                    if (!nieuwepunt.ispause)
                    {
                        // Joepie dit punt is geen pauze. Misschien de vorige?
                        if (!oudepunt.ispause)
                        {
                            // Het vorige ook niet! Nice nu kunnen we de snelheid opschrijven.
                            if (speeds)
                            {
                                // De aanroeper wil de snelheid weten 
                                ret.Add(PuntSnelheid(nieuwepunt, oudepunt));
                            }
                            else
                            {
                                // De aanroeper wil de tijd weten
                                ret.Add(nieuwepunt.timesincestart);
                            }
                        }
                        else
                        {
                            // Het vorige punt was een pauze. 
                        }
                    }
                    else
                    {
                        // Dit punt is een pauze. We moeten hier een snelheid van 0 injecteren zodat er een drop is in de grafiek straks.
                        if (speeds)
                        {
                            ret.Add(0f);
                        }
                        else
                        {
                            ret.Add(nieuwepunt.timesincestart);
                        }
                    }
                }

                i++;
                oudepunt = nieuwepunt;
            }
            return ret;

        }
        public static List<float> Track_List_Distance_OverTime(List<knooppunt> track, bool speeds)
        {
            List<float> ret = new List<float>();
            knooppunt oudepunt = new Kaart.knooppunt();
            knooppunt nieuwepunt = new Kaart.knooppunt();
            float alafgelegd = 0f;
            int i = 0;
            foreach (knooppunt p in track)
            {
                nieuwepunt = p;

                if (i > 0)
                {
                    if (!nieuwepunt.ispause)
                    {
                        // Joepie dit punt is geen pauze. Misschien de vorige?
                        if (!oudepunt.ispause)
                        {
                            // Het vorige ook niet! Nice nu kunnen we de afstand opschrijven.
                            if (speeds)
                            {
                                // De aanroeper wil de snelheid weten 

                                float komterbij = (PuntAfstand(nieuwepunt, oudepunt)/1000f);
                                ret.Add(komterbij + alafgelegd);

                                // Deze regel maakt de lijst cumulatief ipv elke keer de afgelegde afstand erbij.
                                alafgelegd += komterbij;
                            }
                            else
                            {
                                // De aanroeper wil de tijd weten
                                ret.Add(nieuwepunt.timesincestart);
                            }
                        }
                        else
                        {
                            // Het vorige punt was een pauze. 
                        }
                    }
                    else
                    {
                        // Dit punt is een pauze. We moeten hier een zelfde afstand injecteren zodat er een horizontale lijn is in de grafiek straks.
                        if (speeds)
                        {
                            ret.Add(alafgelegd);
                        }
                        else
                        {
                            ret.Add(nieuwepunt.timesincestart);
                        }
                    }
                }

                i++;
                oudepunt = nieuwepunt;
            }
            return ret;

        }

        // Codeer een track naar een string
        public static string Track_Stringify(List<knooppunt> track) {
            string ret = "";
            int i = 0;
            // Voor alle punten..
            foreach (knooppunt punt in track) {
                i++;
                string add = "";

                // Formaat: x?y?tijd
                add += punt.x.ToString() + "?" + punt.y.ToString() + "?" + punt.timesincestart.ToString() + "?" + punt.timesincelastpause.ToString() + "?" + punt.ispause.ToString();

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
        public static List<knooppunt> String_Trackify(string trackstring) {
            List<knooppunt> track = new List<knooppunt>();
            // Splitst de string op in punten die we gaan analyseren
            string[] punten = trackstring.Split('|');
            
            // Voor al die punten..
            foreach (string punt in punten) {
               // Splits de punten in waarden die we kunnen toewijzen.
                string[] puntdata = punt.Split('?');
                
                knooppunt nieuwpunt = new Kaart.knooppunt();
                // Laad de data van de string in de float
                nieuwpunt.x = float.Parse(puntdata[0]);
                nieuwpunt.y = float.Parse(puntdata[1]);
                nieuwpunt.timesincestart = float.Parse(puntdata[2]);
                nieuwpunt.timesincelastpause = float.Parse(puntdata[3]);
                nieuwpunt.ispause = bool.Parse(puntdata[4]);

                // En die float is een track knooppunt!
                track.Add(nieuwpunt);
            }

            return track;


        }

        public static String Track_Debugstring(List<knooppunt> track) {
            String ret = "";
            knooppunt nieuwepunt = new knooppunt();
            knooppunt oudepunt = new knooppunt();
            int i = 0;
            foreach (knooppunt p in track) {
                nieuwepunt = p;

                if (i > 0) {
                    if (oudepunt.ispause)
                    {
                        ret += "\r\n pause: " + PuntAfstand(oudepunt, nieuwepunt).ToString() + " , at " + PuntSnelheid(oudepunt, nieuwepunt).ToString() + "km/u (Duration: " + PuntTijdVerschil(oudepunt,nieuwepunt).ToString() + ")";
                    }
                    else {
                        if (nieuwepunt.ispause)
                        {
                                // Voorafgaand aan een pauze is niks. 
                        }
                        else
                        {
                            ret += "\r\n distance - " + PuntAfstand(oudepunt, nieuwepunt) + "m , at" + PuntSnelheid(oudepunt, nieuwepunt) + " km/u";
                        }
                    }
                }
                ret += "\r\n";
                if (p.ispause)
                {
                    ret += "pause - ";
                }
                else {
                    ret += "point - ";
                }
                ret += p.x.ToString() + ", " + p.y.ToString() + " ||| " + p.timesincestart.ToString() + " - " + p.timesincelastpause.ToString() + "  |||";

                i++;
                oudepunt = nieuwepunt;
            }


            return ret;
        }

        public static String Seconds_ToReadAble(float seconds) {
            String ret = "";
            int hours = (int)seconds / 3600;
            int minutes = (int)seconds / 60 - (hours * 60);
            int secondsint = (int)seconds - (minutes * 60) - (hours *3600);

            if (hours > 0) {
                ret += hours.ToString("0") +":";
            }
            if (minutes > 0) {
                if (minutes < 10) {
                    ret += "0";
                }
                ret += minutes.ToString("0") + ":";
            }
            if (secondsint < 10) {
                ret += "0";
            }

            ret += secondsint.ToString("0") + "";
            if (minutes == 0 && hours ==0) {
                ret += " seconden";
            }

            return ret;
        }

    }

}