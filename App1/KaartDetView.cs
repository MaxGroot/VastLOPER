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
        string faketrackstring = "136269,3?456126,6?2,570979?2,570855?False|136280?456129,7?4,605352?4,605249?False|136294,8?456133,3?7,60808?7,607961?False|136304,9?456133?9,560472?9,560375?False|136319,4?456129,7?12,56349?12,56336?False|136330,2?456135,9?15,61763?15,61745?False|136339?456141,2?17,58376?17,58363?False|136346,2?456149,9?19,6022?19,6021?False|136356,2?456154,2?21,59608?21,59595?False|136369,5?456157,3?24,60511?24,60501?False|136382,1?456159,7?27,56952?27,56939?False|136393,3?456160,9?30,56979?30,5697?False|136407,1?456165,4?34,56471?34,56462?False|136419?456168?37,56009?37,55995?False|136430,4?456170,4?40,60265?40,60254?False|136443,5?456175,7?43,56037?43,56024?False|136455,2?456178,9?46,58278?46,58267?False|136465,2?456176,9?49,57841?49,5783?False|136476,2?456174,5?52,59572?52,59559?False|136486,6?456172,4?55,56348?55,56339?False|136500,3?456171,3?58,56345?58,56334?False|136510,5?456172,6?60,57872?60,57859?False|136520,5?456174,4?62,57833?62,57823?False|136530,6?456174,3?64,60094?64,60081?False|136541?456177,5?66,56069?66,5606?False|136552,2?456178,9?68,5605?68,56035?False|136562,4?456181,7?70,56261?70,5625?False|136572,4?456178,2?72,56542?72,56531?False|136582,9?456179?74,58288?74,58276?False|136595,3?456181,2?77,56107?77,56094?False|136605,4?456184,7?79,6269?79,62675?False|136619,9?456182,2?82,55952?82,55939?False|136629,8?456184,3?84,55818?84,55807?False|136640,1?456186,6?86,59897?86,59885?False|136654,3?456184,8?89,5921?89,592?False|136666,4?456182,5?92,58521?92,58509?False|136681?456176?95,56998?95,56985?False|136692,8?456171,2?97,57023?97,57011?False|136703,5?456172,3?99,56231?99,5622?False|136713,8?456171,1?101,5731?101,5671?False|136729,4?456168,2?104,5767?104,5765?False|136742,8?456164,3?107,6013?107,6012?False|136752?456158,9?109,5673?109,5672?False|136760,9?456150?111,5646?111,5645?False|136767,4?456142,4?113,5636?113,5634?False|136775,7?456136,4?115,5683?115,5682?False|136785,8?456126,3?118,5604?118,5603?False|136796,4?456118,4?121,5745?121,5743?False|136807,3?456115,9?124,5776?124,5775?False|136820,7?456118,3?127,5607?127,5606?False|136830,9?456123,2?130,6253?130,6251?False|136842,3?456127,1?133,594?133,5938?False|136855,9?456128,9?136,6122?136,612?False|136869,7?456131,2?139,5866?139,5864?False|136883,7?456131,2?142,5948?142,5947?False|136895,8?456135,7?145,564?145,5639?False|136907,6?456139,8?148,5629?148,5628?False|136920,4?456142,2?151,5551?151,555?False|136938,3?456140,9?155,5806?155,5805?False|136957,6?456142,1?159,5571?159,557?False|136977?456140,1?163,5612?163,561?False|136991,5?456137,3?166,5565?166,5564?False|137003?456139,4?168,5636?168,5635?False|137013?456135,3?170,579?170,5789?False|137023,2?456134,9?172,5549?172,5548?False|137037?456132,1?175,5638?175,5637?False|137047,1?456128,1?177,5881?177,588?False|137056,4?456121,7?179,555?179,5549?False|137070,4?456120,7?182,5844?182,5843?False|137084,1?456117?185,5743?185,5742?False|137094,2?456119,4?187,5569?187,5568?False|137104,4?456118?189,5583?189,5582?False|137117,5?456115?192,601?192,6008?False|137131,5?456111,8?195,5623?195,5621?False|137145,4?456109,9?198,5677?198,5676?False|137160,3?456109,8?201,5875?201,5874?False|137170?456105,5?203,5589?203,5588?False|137182,2?456101,3?206,5618?206,5616?False|137192,5?456098,5?209,582?209,5819?False|137205?456097,5?213,5642?213,5641?False|137216,3?456098,1?216,5921?216,592?False|137226,7?456096,6?218,6032?218,603?False|137240,1?456094,7?221,5591?221,559?False|137253,4?456092,9?224,5679?224,5678?False|137264,9?456089,5?227,5645?227,5644?False|137275,8?456087,1?230,5966?230,5965?False|137286,8?456084,6?233,5718?233,5717?False|137294,8?456078,3?235,5903?235,5902?False|137310,7?456076,8?239,5953?239,5952?False|137327,3?456072,4?243,5795?243,5794?False|137343,4?456067,2?247,5849?247,5848?False|137352,1?456062,2?249,5758?249,5757?False|137366,6?456053,1?253,5926?253,5924?False|137378,9?456038?257,5878?257,5877?False|137390,7?456029,7?261,5891?261,5889?False|137402?456022?265,5823?265,5821?False|137412,9?456016,5?271,5595?271,5594?False|137422,8?456003,7?275,5735?275,5735?False|137426?455991,8?279,5637?279,5636?False|137437,7?455993,6?306,5782?306,5782?False|137448,8?455991,4?313,5616?313,5615?False|137457,5?455983,3?321,5804?321,5803?False|137468,3?455977?331,6076?331,6075?False|137478,8?455973,2?337,5712?337,5711?False|137485,9?455964,8?343,5578?343,5577?False|137497,1?455964?353,5696?353,5695?False|137505,3?455957,3?359,5673?359,5672?False|137505,3?455957,3?365,2711?365,271?True|137505,1?455950,3?572,5833?6,996379?False|137492,5?455944,4?576,5618?10,97485?False|137483,8?455938,4?579,5615?13,97447?False|137481,2?455950,9?583,5646?17,97761?False|137478,7?455962,8?586,5804?20,99345?False|137468,7?455968?595,5748?29,98787?False|137462,6?455978?603,58?37,99308?False|137453,6?455984,1?610,6031?45,01622?False|137444,6?455991,6?616,5615?50,97454?False|137434,7?455996,1?622,5862?56,99926?False|137434,7?455996,1?633,6946?68,10764?True";
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

