using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Madeira
{
    class Variables
    {
        public double Myrk;
        public double fh1k;
        public double fh2k;
        public double beta;
        public double Faxrk;

        public Variables() {}

        public Variables(
            string type,
            bool smooth,
            bool preDrilled,
            double d,
            double pk1,
            double pk2,
            double alfa,
            string woodType,
            double fu,
            double dh,
            double t,
            double tpen,
            double n,
            double t_thread
        )
        {
            Console.WriteLine("initialized");
            this.Myrk = calcMyrk(d, fu, type, smooth);
            this.fh1k = calcT2TFhk(type, preDrilled, d, pk1, alfa, woodType);
            this.fh2k = calcT2TFhk(type, preDrilled, d, pk2, alfa, woodType);
            this.beta = this.fh2k / this.fh1k;
            this.Faxrk = calcFaxrk(pk1, d, dh, t, tpen, alfa, n, type, smooth, t_thread);
        }



        public double calcMyrk(double d, double fu, string type, bool smooth ) {
            double value = 0;
            if ( type == "nail" && smooth == true ) {
                value = 0.3 * fu * Math.Pow( d, 2.6);
            }
            else if ( ( type == "nail" && smooth == false ) || ( type == "screw" && d <= 6) ) {
                value = 0.45 * fu * Math.Pow( d, 2.6);
            }
            else if ( type == "bolt" || ( type == "screw" && d > 6) ) {
                value = 0.3 * fu * Math.Pow( d, 2.6);
            }

            return value;
        }

        double calcT2TFhk ( string type, bool preDrilled, double d, double pk, double alfa, string woodType) {
            double fhk = 0;
            if ( (type == "nail" && d <= 8) || (type == "screw" && d <= 6 ) ){
                if ( preDrilled == true ) {
                    fhk = 0.082 * pk * Math.Pow( d, -0.3);
                } 
                else {
                    fhk = 0.082 * (1 - 0.01 * d) * pk;
                } 
            }
            if ( (type == "nail" && d > 8) || (type == "bolt") || (type == "screw" && d > 6 ) ){
                fhk = calculateFhAlfak( d, pk, alfa, woodType);
            }
            return fhk;
        }

         double calculateFhAlfak ( double d, double pk, double alfa, string woodType) {
            double f0hk = 0.082 * (1 - 0.01 * d) * pk;
            if ( alfa == 0) {
                return f0hk;
            } else {
                double k90 = calcK90( d, woodType);
                return f0hk / ( k90*Math.Pow( Math.Sin(alfa), 2) + Math.Pow( Math.Cos(alfa), 2) );
            }

        }

        double calcK90 ( double d, string woodType ) {
            double k90 = 0;
            if ( woodType == "softwood") {
                k90 = (1.35 + 0.015 * d );
            }
            else if ( woodType == "hardwood") {
                k90 = (0.9 + 0.015 * d );
            }
            else if ( woodType == "lvl" || woodType == "mlc" ) {
                k90 = (1.3 + 0.015 * d );
            }
            return k90;
        }

        double calcFaxrk (double pk, double d, double dh, double t, double tpen, double alfa, double n, string type, bool smooth, double t_thread) {
            double value = 0;
            if (type == "nail") {
                if (smooth == true) {
                    double faxk = calcNailfaxk(tpen, d, pk, smooth);
                    double fheadk = calcNailfheadk(t, d, pk, smooth);
                    value = Math.Min(faxk * d * tpen, fheadk * Math.Pow(dh, 2));
                }
                else if (smooth == false)
                {
                    double faxk = calcNailfaxk(tpen, d, pk, smooth);
                    double fheadk = calcNailfheadk(t, d, pk, smooth);
                    value = Math.Min((faxk * d * tpen), (faxk * d * t + fheadk * Math.Pow(dh, 2)));
                }
            }   else if (type == "screw") {
                value= calcScrewFaxrk(n, d, pk, alfa, tpen, t_thread);
            }

            return value;
        }

        double calcNailfaxk(double tpen, double d, double pk, bool smooth)
        {
            double faxk = 20 * Math.Pow(10, -6) * Math.Pow(pk, 2);
            if ((8*d < tpen  && tpen <= 12 * d) && smooth == true){
                faxk = faxk * ((tpen / (4 * d)) - 2);
            }
            else if ((8 * d >= tpen) && smooth == true)
            {
                faxk = 0;
            }
            else if ((6 * d < tpen && tpen <= 8 * d) && smooth == false)
            {
                faxk = faxk * ((tpen / (2 * d)) - 3);
            }
            else if ((6 * d >= tpen) && smooth == false)
            {
                faxk = 0;
            }
            return faxk;

        }

        double calcNailfheadk(double t, double d, double pk, bool smooth)
        {

            double fheadk = 70 * Math.Pow(10, -6) * Math.Pow(pk, 2);
            if ((8 * d < t && t <= 12 * d) && smooth == true)
            {
                fheadk = fheadk * ((t / (4 * d)) - 2);
            }
            else if ((8 * d >= t) && smooth == true)
            {
                fheadk = 0;
            }
            else if ((6 * d < t && t <= 8 * d) && smooth == false)
            {
                fheadk = fheadk * ((t / (2 * d)) - 3);
            }
            else if ((6 * d >= t) && smooth == false)
            {
                fheadk = 0;
            }
            return fheadk;

        }

        double calcScrewFaxrk (double n, double d, double pk, double alfa, double tpen, double t_thread ) {
            double f_ax_k = 3.6 * 0.001 * Math.Pow(pk, 1.5);
            double f_ax_alfa_k = f_ax_k / (Math.Pow(Math.Sin(alfa), 2) + 1.5 * Math.Pow(Math.Cos(alfa), 2));

            double l_ef;
            if ( tpen <= t_thread) {
                l_ef = Math.Max(tpen - d, 6 * d);
            } else {
                l_ef = Math.Max(t_thread - d, 6 * d);
            }
            double F_1_ax_afla_k = Math.Pow(Math.PI * d * l_ef, 0.8) * f_ax_alfa_k;
            double nef = Math.Pow(n, 0.9);

            return F_1_ax_afla_k * nef;
        }
        }
}