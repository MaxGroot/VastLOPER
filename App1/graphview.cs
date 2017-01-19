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
            base.OnDraw(canvas);
            canvas.DrawColor(Color.White);

            float startx = this.Width / 8;
            float starty = this.Height / 8;
            float eindx = this.Width - startx;
            float eindy = this.Height - starty;
            float myheight = eindy - starty;
            float mywidth = eindx - startx;
            Paint black = new Paint(); black.Color = Color.Black;
            Paint red = new Paint(); red.Color = Color.Red;
            Paint blue = new Paint(); blue.Color = Color.Blue; blue.StrokeWidth = 18;
            Paint tekstverf = new Paint(); tekstverf.Color = Color.Black; tekstverf.TextSize = 24;

            // Lijn van linksboven naar linksonder
            canvas.DrawLine(startx, starty, startx, eindy,black);
            // Lijn van linksonder naar rechtsonder
            canvas.DrawLine(startx, eindy, eindx, eindy, black);

            // Teken benoemde beginnen


            // Teken benoemde uiteinden
            canvas.DrawText(Axis_List_One.Max().ToString("0.00"), 0, starty, tekstverf); // Linksboven
            canvas.DrawText(Axis_List_Two.Max().ToString("0.00"), eindx, eindy + 8, tekstverf); // Rechtsonder

            // Teken de punten
            PointF nieuwepunt = new PointF(); ;
            PointF oudepunt = new PointF();

            for (int i = 0; i < Axis_List_One.Count; i++) {
                
                // We gaan ze af!
                float myspeed = Axis_List_One[i];
                
                float mytime = Axis_List_Two[i];

                float xfactor = mywidth / Axis_List_Two.Max();
                float yfactor = myheight / (Axis_List_One.Max() - List_Lowest_Not_Null(Axis_List_One));


                float myx = mytime * xfactor;

                float lowestyy = yfactor * List_Lowest_Not_Null(Axis_List_One);
                float myy = myspeed * yfactor;
                myy = eindy - myy;
                
                myy += lowestyy;

                nieuwepunt.X = myx;
                nieuwepunt.Y = myy;

                if (Axis_List_One[i] == 0f)
                {
                    // Dit is een pauzepunt!
                    

                }
                else
                {

                    if (i > 0) {
                        if (Axis_List_One[i-1] != 0f) {
                            // Dit is niet het eerste punt en het vorige punt was ook geen pauzepunt. Lijn trekken dus!
                            
                            canvas.DrawLine(myx, myy, oudepunt.X, oudepunt.Y, blue);
                        }
                    }

                    canvas.DrawCircle(myx, myy, 24, red);

                }

                oudepunt.X = nieuwepunt.X;
                oudepunt.Y = nieuwepunt.Y;
            }

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