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
    // Functie: Het construeren van de KaartDetView, de zichtbare kaart en tekeningen daarop binnen de kaartinterface.

    class KaartDetView : View, ISensorEventListener, ILocationListener, ScaleGestureDetector.IOnScaleGestureListener, GestureDetector.IOnGestureListener
    {
        Bitmap Plaatje, Pijl;  // de bitmaps van de kaart en de locatie-marker
        float Hoek;            // de rotatie van de locatie-marker
        float Schaal;          // de schaal van de kaar

        public bool log = false;      // wordt er op dit moment een track opgenomen?
        public bool fake = true; // Op het moment wordt nog de fake-track weergegeven
        DateTime startmoment; // Startmoment is voor elk punt om zijn verschil in seconden tov van het starten/resumen van het lopen te bepalen.


        public List<float[]> trackpoints = new List<float[]>(); // Het opgenomen track volgens Max!!
        List<float[]> faketrack = new List<float[]>();

        ScaleGestureDetector Detector;
        GestureDetector Detector2;

        // De huidige positie, en het midden van de kaart, in RD-coordinaten
        PointF huidigPos = new PointF(138300, 454400);  // stadion: hier staat de location-pointer zolang de GPS nog niet wakker is
        PointF centrumPos = new PointF(139000, 455500);  // precies het midden van het kaartblad

        // De faketrackstring, een resultaat van een eerdere track_stringify.
        const String faketrackstring = "137277,3?455166,1?4,225444|137518,4?455141,9?10,98134|137699,3?455126,4?17,30371|137849,3?455130,2?23,40523|137940?454995,3?31,5586|138085,5?454765,6?39,49768";
       
        public KaartDetView(Context c) : base(c)
        {
            // faketrackstring decoderen naar een echte track.
            faketrack = TrackAnalyzer.String_Trackify(faketrackstring);
            trackpoints = faketrack; // Voor nu displayen we nog de fake-track. 

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

            // centrumPos is in RD-meters, reken dit om naar bitmap-pixels
            float middenX = (centrumPos.X - 136000) * 0.4f;
            float middenY = (458000 - centrumPos.Y) * 0.4f;

            // Teken de kaart
            Matrix mat = new Matrix();
            mat.PostTranslate(-middenX, -middenY);
            mat.PostScale(this.Schaal, this.Schaal);
            mat.PostTranslate(this.Width / 2, this.Height / 2);
            canvas.DrawBitmap(this.Plaatje, mat, verf);
            
            // Teken de location-marker vóór het pad, zodat het pad over de pijl heen gaat. Dat vind ik namelijk mooier. :D
            mat = new Matrix();
            mat.PostTranslate(-Pijl.Width / 2, -Pijl.Height / 2);
            mat.PostRotate(this.Hoek);
            mat.PostTranslate(this.Width / 2 + (huidigPos.X - centrumPos.X) * 0.4f * this.Schaal
                             , this.Height / 2 + (centrumPos.Y - huidigPos.Y) * 0.4f * this.Schaal
                             );
            canvas.DrawBitmap(this.Pijl, mat, verf);
            
            // Teken het track
            verf.Color = Color.Magenta;
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

            }

           
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
                // Bepaal aantal seconden dat de gebruiker erover heeft gedaan sinds de resume/start vh lopen om op dit punt te komen.
                TimeSpan verschil = DateTime.Now.Subtract(startmoment);
                
                float verschilseconden = (float) verschil.TotalSeconds;

                // Een trackpunt is niet langer een PointF, maar een float array met zijn x, y en verschil in seconden.
                float[] trackpunt = new float[3];
                trackpunt[0] = huidigPos.X;
                trackpunt[1] = huidigPos.Y;
                trackpunt[2] = verschilseconden;

                trackpoints.Add(trackpunt);
                


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
            if (fake) {
                // De fake track is nog ingeladen in de display-track. Display track legen dus en deze variabele op false zetten. 
                fake = false;
                trackpoints.Clear();


            }
            this.log = !this.log;

            // Meest recente startmoment vastleggen.
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

        // Deze functie wordt van buitenaf gecalled als de track gedeeld moet wortden.
        public string TrackText() {
            return TrackAnalyzer.Track_Share_String(trackpoints);
        }
    }


}

