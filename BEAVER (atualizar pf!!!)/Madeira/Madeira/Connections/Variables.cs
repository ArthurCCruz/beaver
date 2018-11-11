using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Madeira.Connections
{
    class Variables
    {
        public double Myrk;
        public double fh1k;
        public double fh2k;

        public double beta;
        public double Faxrk;

        public void calcMyrk(double d, double fu, string type, bool smooth ) {
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
            Myrk = value;
        }

        public void calcT2TFhk ( string type, bool preDrilled, double d, double pk1, double pk2, double alfa, string woodType) {
            if ( (type == "nail" && d <= 8) || (type == "screw" && d <= 6 ) ){
                if ( preDrilled == true ) {
                    fh1k = 0.082 * pk1 * Math.Pow( d, -0.3);
                    fh2k = 0.082 * pk2 * Math.Pow( d, -0.3);
                } 
                else {
                    fh1k = 0.082 * (1 - 0.01 * d) * pk1;
                    fh2k = 0.082 * (1 - 0.01 * d) * pk2;
                } 
            }
            if ( (type == "nail" && d > 8) || (type == "bolt") || (type == "screw" && d > 6 ) ){
                fh1k = calculateFhAlfak( d, pk1, alfa, woodType);
                fh2k = calculateFhAlfak( d, pk2, alfa, woodType);
            }

            beta = fh2k/fh1k;
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

        public void calcFaxrk (double pk, double d, double dh, double t, double tpen, double alfa, double n, string type, bool smooth) {
            if (type == "nail") {
                if (smooth == true {
                    double Faxrk = calcNailFaxrk;
                }
            }   else if (type == "screw") {
                Faxrk = calcScrewFaxrk(n, d, pk, alfa);
            }
 
        }

        double calcNailFaxrk(double d, double tpen, double dh, double d)
        {

        }

        double calcScrewFaxrk(double n, double d, double pk, double alfa ) {
            double faxk = 3.6 * 0.001 * Math.Pow(pk, 1.5);
            double fax_alfa_k = faxk / (Math.Pow(Math.Sin(alfa), 2) + 1.5 * Math.Pow(Math.Cos(alfa), 2));
            double 
        }
}
    }

} 