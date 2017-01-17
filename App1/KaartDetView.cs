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
    class KaartDetView : View, ISensorEventListener, ILocationListener, ScaleGestureDetector.IOnScaleGestureListener, GestureDetector.IOnGestureListener
    {
        Bitmap Plaatje, Pijl;  // de bitmaps van de kaart en de locatie-marker
        float Hoek;            // de rotatie van de locatie-marker
        float Schaal;          // de schaal van de kaart
        bool log = false;      // wordt er op dit moment een track opgenomen?

        DateTime startmoment;


        List<float[]> trackpoints = new List<float[]>(); // Het opgenomen track volgens Max!!


        ScaleGestureDetector Detector;
        GestureDetector Detector2;

        // De huidige positie, en het midden van de kaart, in RD-coordinaten
        PointF huidigPos = new PointF(138300, 454400);  // stadion: hier staat de location-pointer zolang de GPS nog niet wakker is
        PointF centrumPos = new PointF(139000, 455500);  // precies het midden van het kaartblad

        public KaartDetView(Context c) : base(c)
        {
            Detector = new ScaleGestureDetector(c, this);
            Detector2 = new GestureDetector(c, this);
            this.SetBackgroundColor(Color.White);

            // Laad de bitmaps voor de kaart en het location-pijltje
            BitmapFactory.Options opt = new BitmapFactory.Options();
            opt.InScaled = false;
            Plaatje = BitmapFactory.DecodeResource(c.Resources, Resource.Drawable.Utrecht, opt);
            Pijl = BitmapFactory.DecodeResource(c.Resources, Resource.Drawable.maxgroot, opt);

            // Abonneren op kompas-bewegingen
            SensorManager sm = (SensorManager)c.GetSystemService(Context.SensorService);
            sm.RegisterListener(this, sm.GetDefaultSensor(SensorType.Orientation), SensorDelay.Ui);

            // Abonneren op GPS
            LocationManager lm = (LocationManager)c.GetSystemService(Context.LocationService);
            Criteria crit = new Criteria();
            crit.Accuracy = Accuracy.Fine;
            string lp = lm.GetBestProvider(crit, true);
            if (lp != null)
                lm.RequestLocationUpdates(lp, 0, 10, this);

            // Abonneren op scherm-aanrakingen
            this.Touch += RaakAan;
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            if (Schaal == 0)
                Schaal = Math.Max(((float)this.Width) / this.Plaatje.Width, ((float)this.Height) / this.Plaatje.Height);

            Paint verf = new Paint();
            Paint tekstverf = new Paint();

            // centrumPos is in RD-meters, reken dit om naar bitmap-pixels
            float middenX = (centrumPos.X - 136000) * 0.4f;
            float middenY = (458000 - centrumPos.Y) * 0.4f;

            // Teken de kaart
            Matrix mat = new Matrix();
            mat.PostTranslate(-middenX, -middenY);
            mat.PostScale(this.Schaal, this.Schaal);
            mat.PostTranslate(this.Width / 2, this.Height / 2);
            canvas.DrawBitmap(this.Plaatje, mat, verf);

            // Teken het track
            verf.Color = Color.Magenta;
            tekstverf.Color = Color.White;
            foreach (float[] p in trackpoints)
            {
                // p is een track-point in RD-meters
                // bereken de afstand tot het midden van de bitmap, gerekend in bitmap-pixels
                float bpx = (p[0] - centrumPos.X) * 0.4f;
                float bpy = (centrumPos.Y - p[1]) * 0.4f;
                
                // bereken de afstand tot het midden van de bitmap, gerekend in scherm-pixels
                float sx = bpx * this.Schaal;
                float sy = bpy * this.Schaal;
                
                // reken dit om naar absolute scherm-pixels
                float x = this.Width / 2 + sx;
                float y = this.Height / 2 + sy;
                canvas.DrawCircle(x, y, 13, verf);
                canvas.DrawText(p[2].ToString(),x, y, tekstverf);

            }

            // Teken de location-marker
            mat = new Matrix();
            mat.PostTranslate(-Pijl.Width / 2, -Pijl.Height / 2);
            mat.PostRotate(this.Hoek);
            mat.PostTranslate(this.Width / 2 + (huidigPos.X - centrumPos.X) * 0.4f * this.Schaal
                             , this.Height / 2 + (centrumPos.Y - huidigPos.Y) * 0.4f * this.Schaal
                             );
            canvas.DrawBitmap(this.Pijl, mat, verf);
        }

        // Implementatie van ISensorEventListener
        public void OnSensorChanged(SensorEvent e)
        {
            this.Hoek = e.Values[0];
            this.Invalidate();
        }
        public void OnAccuracyChanged(Sensor s, SensorStatus accuracy)
        {
        }

        // Implementatie van ILocationListener
        public void OnLocationChanged(Location loc)
        {
            huidigPos = Projectie.Geo2RD(loc);
           


            if (log)
            {
                TimeSpan verschil = DateTime.Now.Subtract(startmoment);

                int verschilseconden = verschil.Seconds;

                float[] trackpunt = new float[3];
                trackpunt[0] = huidigPos.X;
                trackpunt[1] = huidigPos.Y;
                trackpunt[2] = verschilseconden;

                trackpoints.Add(trackpunt);

                Console.WriteLine(TrackManager.Track_Total_Distance(trackpoints).ToString());


            }
            this.Invalidate();
        }
        public void OnProviderDisabled(string s) { }
        public void OnProviderEnabled(string s) { }
        public void OnStatusChanged(string provider, Availability status, Bundle extras) { }


        public void RaakAan(object o, TouchEventArgs tea)
        {
            Detector.OnTouchEvent(tea.Event);
            Detector2.OnTouchEvent(tea.Event);
        }

        // Event-handlers voor de drie buttons in het hoofdscherm
        public void Centreer(object o, EventArgs ea)
        {
            this.centrumPos = this.huidigPos;
            this.Invalidate();
        }
        public void Start(object o, EventArgs ea)
        {

            this.log = !this.log;
            startmoment = DateTime.Now; 


            this.Invalidate();
        }
        public void Schoon(object o, EventArgs ea)
        {
            this.trackpoints.Clear();
            this.Invalidate();
        }

        // Implementatie IOnScaleGestureListener
        public bool OnScale(ScaleGestureDetector detector)
        {
            this.Schaal *= detector.ScaleFactor;
            // Zet een limiet op de schaalfactor
            if (this.Schaal > 10) this.Schaal = 10;
            if (this.Schaal < 0.1) this.Schaal = 0.1f;
            this.Invalidate();
            return true;
        }

        public bool OnScaleBegin(ScaleGestureDetector detector)
        {
            return true;
        }

        public void OnScaleEnd(ScaleGestureDetector detector)
        {
        }

        // Implementatie IOnGestureListener
        public bool OnScroll(MotionEvent e1, MotionEvent e2, float dx, float dy)
        {
            float ax = (dx / this.Schaal) / 0.4f;  // afstand tot het midden in meters
            float ay = (dy / this.Schaal) / 0.4f;

            this.centrumPos = new PointF(centrumPos.X + ax, centrumPos.Y - ay);
            return true;
        }

        public bool OnDown(MotionEvent e)
        {
            return true;
        }

        public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            return true;
        }

        public void OnLongPress(MotionEvent e)
        {
        }

        public void OnShowPress(MotionEvent e)
        {
        }

        public bool OnSingleTapUp(MotionEvent e)
        {
            return true;
        }
    }

    class TrackManager {

        public static float PuntAfstand(PointF een, PointF twee) {
            // Returnt de afstand in meters tussen twee  RD punten.
            float afstandX = Math.Abs(een.X - twee.X);
            float afstandY = Math.Abs(een.Y - twee.Y);
            return afstandX + afstandY;
        }

        public static float PuntSnelheid(PointF een, PointF twee , float tijdverschil) {
            // Returnt de snelheid tussen twee punten in kilometers per uur.
            tijdverschil = Math.Abs(tijdverschil);

            float afstand = PuntAfstand(een, twee);

            return (afstand / tijdverschil) * 3.6f;
            

        }

        public static float Track_Total_Distance(List<float[]> track)        {            // Returnt de totale afstand in meters.
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
                else {
                    // Tweede of later punt. Afstand vergelijken met vorige punt en dit optellen aan de oude afstand
                    ditpunt.X = punt[0]; ditpunt.Y = punt[1];
                    totaldistance += PuntAfstand(ditpunt, oudepunt);


                

                }

                oudepunt.X = punt[0]; oudepunt.Y = punt[1];

                i++;
            }

            return totaldistance;
        }

        public static float Track_Average_Speed(List<float[]> track , bool includepause) {
            float trackdistance = Track_Total_Distance(track);

            if (includepause)
            {
                // De pauzes tellen ook mee voor de gemiddelde snelheid. Dat betekent dat we gewoon het tijdsverschil van punt 1 en het laatste punt kunnen gebruiken!
                float[] eerstepunt = track[0];
                float[] laatstepunt = track[track.Count - 1];

                float verschilinseconden = laatstepunt[2] - eerstepunt[2];


                return (trackdistance / verschilinseconden) * 3.6f;

            }
            else {
                // Er moet een lijst gemaakt worden van alle snelheden die de gebruiker heeft gehad tijdens het lopen. Daarna moet de gemiddelde waarde in die lijst worden gereturned. 
                List<float> snelheden = new List<float> ();
                int i = 0;
                float[] oudepunt = { };
                float[] nieuwepunt = { };

                foreach (float[] punt in track) {
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
                for (i = 0; i < snelheden.Count; i++) {
                    totaltime += snelheden[i];
                }
                return (trackdistance / totaltime) * 3.6f;


            }
  
        }

    }
}

