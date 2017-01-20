using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Kaart
{
    public class graphview : View
    {
        public List<float> Axis_List_One;
        public List<float> Axis_List_Two;
        public String Axis_Name_One;
        public String Axis_Name_Two;
        public graphview(Context context) : base(context) {

        }
        public void Set_Axis_List_One(List<float> lijst1) {
            this.Axis_List_One = lijst1;
        }
        public void Set_Axis_List_Two(List<float> lijst2) {
            this.Axis_List_Two = lijst2;
        }

        protected override void OnDraw(Canvas canvas)
        {
            // Hoe de grafiekview werkt:
            // Eerst alle definities, globale variabelen die handig zijn. Daarna de eenheden aan de uiteinden van de assen benoemen en weergeven. 
            // Daarna streepjes over de X-as, met bijbehorende eenheden, dan lijnen voor de grafiek, dan de uitlijning van de grafiek en het 0,0 punt!

            base.OnDraw(canvas);
            canvas.DrawColor(Color.White);
            
            // De vier hoeken van de grafiek
            float startx = this.Width / 8;
            float starty = this.Height / 8;
            float eindx = this.Width - startx;
            float eindy = this.Height - starty;

            

            // Hoogte en breedte vd grafiek
            float myheight = eindy - starty;
            float mywidth = eindx - startx;

            float aantalstreepjes = 4f;
            float aantalkwadranten = 6f;

            // Offsettoground is zodat de laagste waarde niet op de grond van de grafiek zit, zodat duidelijk is dat die waarde niet 0 is. 
            float offsettoground = myheight / aantalkwadranten;

            // Verfjes
            Paint black = new Paint(); black.Color = Color.Black;
            Paint red = new Paint(); red.Color = Color.Red;
            Paint blue = new Paint(); blue.Color = Color.Blue; blue.StrokeWidth = 18;
            Paint pink = new Paint(); pink.Color = Color.Pink; pink.StrokeWidth = 18;
            Paint tekstverf = new Paint(); tekstverf.Color = Color.Black; tekstverf.TextSize = 24;
            Paint grijsverf = new Paint(); grijsverf.Color = Color.LightGray;
            Paint witverf = new Paint(); witverf.Color = Color.White;
            

            // Teken benoemde uiteinden
            canvas.DrawText(Axis_Name_One + "\r\n " + Axis_List_One.Max().ToString("0.0"), 0, starty - 24, tekstverf); // Linksboven
            canvas.DrawText(TrackAnalyzer.Seconds_ToReadAble(Axis_List_Two.Max()) + "\r\n" + Axis_Name_Two , eindx, eindy + 34, tekstverf); // Rechtsonder

            // Bereken de impact van verschillen in de waarden op de assen tov coordinaten op het scherm

            float xfactor = mywidth / Axis_List_Two.Max();
            float yfactor = (myheight - offsettoground) / (Axis_List_One.Max() - List_Lowest_Not_Null(Axis_List_One));
            float laagstepunt = eindy - offsettoground;

            // Teken de kwadranten vertical, met tekst. 

            float snelheidverschil = Axis_List_One.Max() - List_Lowest_Not_Null(Axis_List_One);
            float tijdverschil = Axis_List_Two.Max();


            float kwadrantsnelheidfactor = snelheidverschil / aantalkwadranten;
            
            // Teken omstebeurt grijze en witte vierkanten met links ervan welke Y-waarde erbij hoort.
            for (int i = 1; i <= (aantalkwadranten - 1); i++) {
                float kwadranty = laagstepunt - i * offsettoground;
                Paint verfje = new Paint();
                if (i % 2 == 0)
                {
                    // Even, we gaan voor wit.
                    verfje = witverf;
                }
                else {
                     verfje = grijsverf;
                }
                canvas.DrawRect(startx, kwadranty, eindx, kwadranty + offsettoground, verfje);
                canvas.DrawText((List_Lowest_Not_Null(Axis_List_One) + (kwadrantsnelheidfactor * i)).ToString("0.0"), 0, kwadranty + offsettoground, tekstverf);
            }


            // Nu de horizontale streepjes voor de X-as
            float kwadranttijdfactor = tijdverschil / aantalstreepjes;
            float streepjespauze = (eindx - startx) / aantalstreepjes;
            for (int i = 1; i <= (aantalstreepjes - 1); i++) {

                float kwadrantx = startx + (i * streepjespauze);
                canvas.DrawLine(kwadrantx, eindy - 8, kwadrantx, eindy + 8, black);
                canvas.DrawText(TrackAnalyzer.Seconds_ToReadAble(kwadranttijdfactor * i), kwadrantx, eindy + 34, tekstverf);

            }

            // Teken de punten
            PointF nieuwepunt = new PointF(); ;
            PointF oudepunt = new PointF();

            for (int i = 0; i < Axis_List_One.Count; i++) {
                
                // We gaan ze af!
                float myspeed = Axis_List_One[i];
                
                float mytime = Axis_List_Two[i];

               
                float myx = mytime * xfactor;

                float lowestyy = yfactor * List_Lowest_Not_Null(Axis_List_One);
                float myy = myspeed * yfactor;

                myy = laagstepunt - myy;
                
                myy += lowestyy;

                nieuwepunt.X = myx;
                nieuwepunt.Y = myy;

                if (Axis_List_One[i] == 0f)
                {
                    // Dit is een pauzepunt!

                    if ((i + 1) < Axis_List_One.Count)
                    {
                        // Na deze pauze gaat de track weer verder. 

                        
                    }


                }
                else
                {

                    if (i > 0) {
                        if (Axis_List_One[i-1] != 0f) {
                            // Dit is niet het eerste punt en het vorige punt was ook geen pauzepunt. Lijn trekken dus!
                            
                            canvas.DrawLine(myx, myy, oudepunt.X, oudepunt.Y, blue);
                        }
                    }

                    // canvas.DrawCircle(myx, myy, 24, red);

                }

                oudepunt.X = nieuwepunt.X;
                oudepunt.Y = nieuwepunt.Y;
            }


            // Als laatste de uitlijning van de grafiek!
            // Lijn van linksboven naar linksonder
            canvas.DrawLine(startx, starty, startx, eindy, black);
            // Lijn van linksonder naar rechtsonder
            canvas.DrawLine(startx, eindy, eindx, eindy, black);
            // Lijn van rechtsonder naar rechtsboven
            canvas.DrawLine(eindx, eindy, eindx, starty, black);
            // Lijn van linksboven naar rechtsboven
            canvas.DrawLine(startx, starty, eindx, starty, black);

            // Teken de 0,0 linksonder
            canvas.DrawText("0 , 0", startx - 40, eindy + 24, tekstverf);

        }

        protected float List_Lowest_Not_Null(List<float> list) {
            float ret;
            ret = 9999f;
            foreach (float getal in list) {
                if (getal != 0f) {
                    if (getal < ret) {
                        ret = getal;
                    }
                }
            }
            return ret;
        }

    }
}