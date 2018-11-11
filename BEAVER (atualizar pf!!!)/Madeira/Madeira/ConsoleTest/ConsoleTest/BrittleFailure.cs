using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madeira
{
    class BrittleFailure
    {
        public double a1;
        public double a1_n;
        public double a2;
        public double a3t;
        public double a3c;
        public double a4t;
        public double a4c;

        public BrittleFailure(Fastener fastener, double pk, double alfa, bool preDrilled)
        {
            if (fastener.type == "nail" || (fastener.type == "screw" && fastener.d <= 6))
            {
                this.CalculateForNails(pk, fastener.d, alfa);
            }
            else if (fastener.type == "bolt" || (fastener.type == "screw" && fastener.d > 6))
            { 
                this.CalculateForBolt(alfa, fastener.d);
            }
            else if (fastener.type == "dowel")
            {
                this.CalculateForDowel(alfa, fastener.d);

            }
        }

        void CalculateForNails(double pk, double d, double alfa)
        {
            double inRad = alfa * Math.PI / 180;
            double cosAlfa = Math.Cos(inRad);
            double sinAlfa = Math.Sin(inRad);
            this.a1_n = 14*d;
            if (pk <= 420 && d <= 6)
            {
                if (0 <= alfa && alfa <= 360)
                {
                    if (d < 5) this.a1 = (5 + 5 * Math.Abs(cosAlfa)) * d;
                    else if (d >= 5) this.a1 = (5 + 7 * Math.Abs(cosAlfa)) * d;
                }

                if ( 0 <= alfa && alfa <= 360 ) this.a2 = 5 * d;

                if ( -90 <= alfa && alfa <= 90 ) this.a3t = (10 + 5 * cosAlfa) * d;
                 
                if ( 90 <= alfa && alfa <= 270 ) this.a3c = 10 * d;

                if (0 <= alfa && alfa <= 360)
                {
                    if (d < 5) this.a4t = (5 + 2 * sinAlfa) * d;
                    else if (d >= 5) this.a4t = (5 + 5 * sinAlfa) * d;
                }

                if (180 <= alfa && alfa <= 360) this.a4c = 5 * d;
            }
            else if ( 420 < pk && pk <= 500 && d <= 6)
            {
                if (0 <= alfa && alfa <= 360) this.a1 = (7 + 8 * Math.Abs(cosAlfa)) * d;

                if (0 <= alfa && alfa <= 360) this.a2 = 7 * d;

                if (-90 <= alfa && alfa <= 90) this.a3t = (15 + 5 * cosAlfa) * d;

                if (90 <= alfa && alfa <= 270) this.a3c = 15 * d;

                if (0 <= alfa && alfa <= 360)
                {
                    if (d < 5) this.a4t = (7 + 2 * sinAlfa) * d;
                    else if (d >= 5) this.a4t = (7 + 5 * sinAlfa) * d;
                }

                if (180 <= alfa && alfa <= 360) this.a4c = 7 * d;
            }
            else if ( pk > 500 || d > 6)
            {
                if (0 <= alfa && alfa <= 360) this.a1 = (4 + 1 * Math.Abs(cosAlfa)) * d;

                if (0 <= alfa && alfa <= 360) this.a2 = (3 + Math.Abs(sinAlfa)) * d;

                if (-90 <= alfa && alfa <= 90) this.a3t = (7 + 5 * cosAlfa) * d;

                if (90 <= alfa && alfa <= 270) this.a3c = 7 * d;

                if (0 <= alfa && alfa <= 360)
                {
                    if (d < 5) this.a4t = (3 + 2 * sinAlfa) * d;
                    else if (d >= 5) this.a4t = (3 + 4* sinAlfa) * d;
                }
                if (180 <= alfa && alfa <= 360) this.a4c = 3 * d;

            }
        }

        void CalculateForBolt(double alfa, double d) 
        {
            double inRad = alfa * Math.PI / 180;
            double cosAlfa = Math.Cos(inRad);
            double sinAlfa = Math.Sin(inRad);

            if (0 <= alfa && alfa <= 360) this.a1 = (4 + Math.Abs(cosAlfa)) * d;

            if (0 <= alfa && alfa <= 360) this.a2 = 4 * d;

            if (-90 <= alfa && alfa <= 90) this.a3t = Math.Max(7 * d, 80);

            if (90 <= alfa && alfa < 150) this.a3c = Math.Max((1 + 6 * sinAlfa) * d, 4 * d);
            else if (150 <= alfa && alfa < 210) this.a3c = 4 * d;
            else if (210 <= alfa && alfa <= 270) this.a3c = Math.Max((1 + 6 * sinAlfa) * d, 4 * d);

            if (0 <= alfa && alfa <= 180) this.a4t = Math.Max((2 + 2 * sinAlfa) * d, 3 * d);

            if (180 <= alfa && alfa <= 360) this.a4c = 3 * d;
        }

        void CalculateForDowel(double alfa, double d) 
        {
            double inRad = alfa * Math.PI / 180;
            double cosAlfa = Math.Cos(inRad);
            double sinAlfa = Math.Sin(inRad);

            if (0 <= alfa && alfa <= 360) this.a1 = (3 + 2 * Math.Abs(cosAlfa)) * d;

            if (0 <= alfa && alfa <= 360) this.a2 = 2 * d;

            if (-90 <= alfa && alfa <= 90) this.a3t = Math.Max(7 * d, 80);

            if (90 <= alfa && alfa < 150) this.a3c = Math.Max((this.a3t * Math.Abs(sinAlfa)) * d, 3 * d);
            else if (150 <= alfa && alfa < 210) this.a3c = 3 * d;
            else if (210 <= alfa && alfa <= 270) this.a3c = Math.Max((this.a3t * Math.Abs(sinAlfa)) * d, 3 * d);

            if (0 <= alfa && alfa <= 180) this.a4t = Math.Max((2 + 2 * sinAlfa) * d, 3 * d);

            if (180 <= alfa && alfa <= 360) this.a4c = 3 * d;
        }
    }
}
