using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
namespace Madeira
{
    class Variables
    {
        public double Myrk;
        public double fhk;
        public double fh1k;
        public double fh2k;
        public double beta;
        public double Faxrk;
        public double tpen;
        public string error;

        public Variables() {}

        public Variables( //For timber to timber cases
            Fastener fastener,
            bool preDrilled,
            double pk1,
            double pk2,
            double alfa,
            string woodType,
            double t1,
            double t2,
            double n
        )
        {
            this.Myrk = CalcMyrk(fastener);
            this.fh1k = CalcFhk(preDrilled, fastener, pk1, alfa, woodType);
            this.fh2k = CalcFhk(preDrilled, fastener, pk2, alfa, woodType);
            this.beta = this.fh2k / this.fh1k;
            this.tpen = GetTpen(fastener, t1, t2);
            this.Faxrk = CalcFaxrk(pk1, fastener, t1, this.tpen, alfa, n);
        }

        public Variables( //For timber to steel cases
            Fastener fastener,
            bool preDrilled,
            double pk,
            double alfa,
            string woodType,
            double t1,
            double t_steel,
            double n
        )
        {
            this.Myrk = CalcMyrk(fastener);
            this.fhk = CalcFhk(preDrilled, fastener, pk, alfa, woodType);
            this.Faxrk = CalcFaxrk(pk, fastener, t1, fastener.l - t1, alfa, n);
        }

        public double GetTpen(Fastener fastener, double t1, double t2)
        {
            double tpoint = fastener.l - t1;
            if(t2 - tpoint <= 4* fastener.d)
            {
                this.error = "(t2 - tpoint) must be at least 4d";
            }
            else if (tpoint < 8* fastener.d)
            {
                this.error = "tpoint must be at least 8d";
            }
            return tpoint;
        }

        public double CalcMyrk(Fastener fastener) {
            double value = 0;
            if ( fastener.type == "nail" && fastener.smooth == true ) {
                value = 0.3 * fastener.fu * Math.Pow( fastener.d, 2.6);
            }
            else if ( ( fastener.type == "nail" && fastener.smooth == false ) || ( fastener.type == "screw" && fastener.d <= 6) ) {
                value = 0.45 * fastener.fu * Math.Pow( fastener.d, 2.6);
            }
            else if ( fastener.type == "bolt" || ( fastener.type == "screw" && fastener.d > 6) ) {
                value = 0.3 * fastener.fu * Math.Pow( fastener.d, 2.6);
            }

            return value;
        }

        double CalcFhk ( bool preDrilled, Fastener fastener, double pk, double alfa, string woodType) {
            double fhk = 0;
            if ( (fastener.type == "nail" && fastener.d <= 8) || (fastener.type == "screw" && fastener.d <= 6 ) ){
                if ( preDrilled == false ) {
                    fhk = 0.082 * pk * Math.Pow( fastener.d, -0.3);
                } 
                else {
                    fhk = 0.082 * (1 - 0.01 * fastener.d) * pk;
                } 
            }
            if ( (fastener.type == "nail" && fastener.d > 8) || (fastener.type == "bolt") || (fastener.type == "screw" && fastener.d > 6 ) ){
                fhk = CalculateFhAlfak( fastener.d, pk, alfa, woodType);
            }
            return fhk;
        }

         double CalculateFhAlfak ( double d, double pk, double alfa, string woodType) {
            double f0hk = 0.082 * (1 - 0.01 * d) * pk;
            if ( alfa == 0) {
                return f0hk;
            } else {
                double k90 = CalcK90( d, woodType);
                return f0hk / ( k90*Math.Pow( Math.Sin(alfa * Math.PI / 180), 2) + Math.Pow( Math.Cos(alfa * Math.PI / 180), 2) );
            }

        }

        double CalcK90 ( double d, string woodType ) {
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

        double  CalcFaxrk  (double pk, Fastener fastener, double t1, double tpen, double alfa, double n) {
            double value = 0;
            if (fastener.type == "nail") {
                double fpaxk = CalcNailfaxk(tpen, fastener.d, pk, fastener.smooth);
                double fhaxk = CalcNailfaxk(t1, fastener.d, pk, fastener.smooth);
                double fheadk = CalcNailfheadk(pk);
                value = Math.Min(fpaxk * fastener.d * tpen, fhaxk * fastener.d * t1 + fheadk * Math.Pow(fastener.dh, 2));
            }   else if (fastener.type == "screw") {
                value= CalcScrewFaxrk(n, fastener.d, pk, alfa, tpen, fastener.t_thread);
            }

            return value;
        }

        double CalcNailfaxk(double tpen, double d, double pk, bool smooth)
        {
            double faxk = 20 * Math.Pow(10, -6) * Math.Pow(pk, 2);
            double min; double max;
            if(smooth == true)
            {
                min = 8; max = 12;
            }
            else
            {
                min = 6; max = 8;
            }
            double coef = CalcFaxkCoeficient(d, tpen, min, max);
            return coef * faxk;
        }

        double CalcFaxkCoeficient(double d, double t, double min, double max)
        {
            double coef = (1 / (max*d - min*d)) * (t - min*d);
            if (coef > 1) return 1;
            else if (coef < 0) return 0;
            else return coef;
        }

        double CalcNailfheadk(double pk)
        {
            double fheadk = 70 * Math.Pow(10, -6) * Math.Pow(pk, 2);
            return fheadk;
        }

        double CalcScrewFaxrk (double n, double d, double pk, double alfa, double tpen, double t_thread ) {
            double f_ax_k = 3.6 * 0.001 * Math.Pow(pk, 1.5);
            double f_ax_alfa_k = f_ax_k / (Math.Pow(Math.Sin(alfa * Math.PI / 180), 2) + 1.5 * Math.Pow(Math.Cos(alfa * Math.PI / 180), 2));

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