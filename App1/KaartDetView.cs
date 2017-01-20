using System;             // vanwege Math
using Android.Views;      // vanwege View, TouchEventArgs
using Android.Graphics;   // vanwege Paint, Canvas, PointF
using Android.Content;    // vanwege Context
using Android.Hardware;   // vanwege SensorManager, ISensorEventListener
using Android.Locations;  // vanwege Location, ILocationListener
using Android.OS;         // vanwege Bundle
using System.Collections.Generic;  // vanwege Listusing Android.App;
using Android.Widget;
using Android.App;


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
        public bool havelocation = false;
        DateTime pauzemoment; // Pauzemoment is voor elk punt om zijn verschil in seconden tov van het laatste starten/resumen van het lopen te bepalen.
        DateTime startmoment; // Startmoment is voor elk punt om zijn verschil in seconden te bepalen tov de eerste start. 
        

        public List<knooppunt> trackpoints = new List<knooppunt>(); // Het opgenomen track volgens Max!!
        List<knooppunt> faketrack = new List<knooppunt>();
        private Context toastercontext;
        ScaleGestureDetector Detector;
        GestureDetector Detector2;

        // De huidige positie, en het midden van de kaart, in RD-coordinaten
        PointF huidigPos = new PointF(138300, 454400);  // stadion: hier staat de location-pointer zolang de GPS nog niet wakker is
        PointF centrumPos = new PointF(139000, 455500);  // precies het midden van het kaartblad
        
        // De faketrackstring, een resultaat van een eerdere track_stringify.
        const String faketrackstring = "140153,1?455083,7?7,754532?7,522213?False|140164,9?455034,4?22,75727?22,5254?False|140195,7?455033,9?30,79465?30,56279?False|140195,7?455033,9?35,94473?35,71283?True|140269,1?455056,6?63,11162?6,333959?False|140269,6?455078,7?69,25266?12,475?False|140262,2?455097,4?75,90218?19,12452?False|140246?455111,4?84,58216?27,8045?False";
        public KaartDetView(Context c) : base(c)
        {
            toastercontext = c;
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
            Toast.MakeText(c, " Locatie vaststellen... ", ToastLength.Short).Show();


        }


        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            if (Schaal == 0)
                Schaal = Math.Max(((float)this.Width) / this.Plaatje.Width, ((float)this.Height) / this.Plaatje.Height);

            Paint verf = new Paint();
            Paint rodeverf = new Paint();

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
            rodeverf.Color = Color.Red;
            foreach (knooppunt p in trackpoints)
            {
                // p is een track-point in RD-meters
                // bereken de afstand tot het midden van de bitmap, gerekend in bitmap-pixels
                float bpx = (p.x - centrumPos.X) * 0.4f;
                float bpy = (centrumPos.Y - p.y) * 0.4f;
                
                // bereken de afstand tot het midden van de bitmap, gerekend in scherm-pixels
                float sx = bpx * this.Schaal;
                float sy = bpy * this.Schaal;
                
                // reken dit om naar absolute scherm-pixels
                float x = this.Width / 2 + sx;
                float y = this.Height / 2 + sy;

                if (p.ispause)
                {
                    // Pauzepunt tekenen
                    canvas.DrawCircle(x, y, 15, rodeverf);
                }
                else
                {
                    canvas.DrawCircle(x, y, 13, verf);
                }
            }

            // Teken een debugvierkant
            /*
            Rect vierkantje = new Rect(0, 0, this.Width, 120);
            Paint emoverf = new Paint(); emoverf.Color = Color.Black;
            Paint tekstverf = new Paint(); tekstverf.Color = Color.White;
            canvas.DrawRect(vierkantje, emoverf);
            canvas.DrawText("Totale pauzetijd: " + pauseseconds.ToString(), 24, 24, tekstverf);
            */
           
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
            if (!havelocation)
            {
                // We hebben de locatie voor het eerst!
                havelocation = true;
                Toast.MakeText(toastercontext, " Locatie vastgesteld! Je kunt nu beginnen.  ", ToastLength.Short).Show();

            }
            huidigPos = Projectie.Geo2RD(loc);
           


            if (log)
            {
                // Bepaal aantal seconden dat de gebruiker erover heeft gedaan sinds de resume/start vh lopen om op dit punt te komen.
                TimeSpan pauzeverschil = DateTime.Now.Subtract(pauzemoment);
                TimeSpan tijdverschil = DateTime.Now.Subtract(startmoment);
                float pauzeverschilseconden = (float) pauzeverschil.TotalSeconds;
                float startverschilseconden = (float) tijdverschil.TotalSeconds;
                // Een trackpunt is niet langer een PointF, maar een float array met zijn x, y en verschil in seconden.
                /*
                float[] trackpunt = new float[3];
                trackpunt[0] = huidigPos.X;
                trackpunt[1] = huidigPos.Y;
                trackpunt[2] = verschilseconden;
                */

                knooppunt trackpunt = new knooppunt(huidigPos.X,huidigPos.Y,startverschilseconden,pauzeverschilseconden,false);

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
            if (!havelocation) {
                // Deze knop mag GEEN effect hebben als er nog geen locatie lock is.
                Toast.MakeText(toastercontext, " Je locatie is nog onbekend!", ToastLength.Short).Show();

                return;
            }
            if (fake)
            {
                // De fake track is nog ingeladen in de display-track. Display track legen dus en deze variabele op false zetten. 
                
                fake = false;
                trackpoints.Clear();
                Toast.MakeText(toastercontext, " Daar gaan we! ", ToastLength.Short).Show();


                // Ook het startmoment vastleggen.
                startmoment = DateTime.Now;
                pauzemoment = DateTime.Now;

            }
            else {
                // Hé! Er is op stop gedrukt terwijl een echte track bezig was. Een pauze dus!
                if (log)
                {
                    // Er is op stop gedrukt! Pauze!! Die pauze moet in de track worden vastgelegd.
                    Toast.MakeText(toastercontext, " Lopen gepauzeerd. ", ToastLength.Short).Show();

                    TimeSpan pauzeverschil = DateTime.Now.Subtract(pauzemoment);
                    TimeSpan tijdverschil = DateTime.Now.Subtract(startmoment);
                    float pauzeverschilseconden = (float)pauzeverschil.TotalSeconds;
                    float startverschilseconden = (float)tijdverschil.TotalSeconds;

                    knooppunt trackpunt = new Kaart.knooppunt(huidigPos.X, huidigPos.Y,startverschilseconden, pauzeverschilseconden, true);

                    trackpoints.Add(trackpunt);

                }
                else {
                    // Er is op start gedrukt! Dat betekent dat er hiervoor een pauze was. 

                    Toast.MakeText(toastercontext," Lopen hervat! ", ToastLength.Short).Show();

                }

            }


            // Meest recente pauzemoment vastleggen.
            pauzemoment = DateTime.Now;
            log = !log;

            this.Invalidate();
        }
        public void Schoon(object o, EventArgs ea)
        {
            this.trackpoints.Clear();
            this.Invalidate();
            Toast.MakeText(toastercontext, " Track gewist. ", ToastLength.Short).Show();

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

