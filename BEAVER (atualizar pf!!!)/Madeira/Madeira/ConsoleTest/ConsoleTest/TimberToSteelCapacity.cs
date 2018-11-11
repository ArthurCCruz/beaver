using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madeira
{
    class TimberToSteelCapacity
    {
        public Variables variables;
        public Fastener fastener;
        public bool preDrilled;
        public double pk;
        public double alfa;
        public string woodType;
        public double t1;
        public double t_steel;
        public double n;
        public bool thickPlate;
        public double Faxrk_upperLimit;

        public TimberToSteelCapacity() {}

        public TimberToSteelCapacity(
            Fastener Fastener,
            bool PreDrilled,
            double Pk,
            double Alfa,
            string WoodType, 
            double T1,
            double T_steel,
            double N,
            bool ThickPlate
        ) {
            this.fastener = Fastener;
            this.preDrilled = PreDrilled;
            this.pk = Pk;
            this.alfa = Alfa;
            this.woodType = WoodType;
            this.t1 = T1;
            this.t_steel = T_steel;
            this.n = N;
            this.thickPlate = ThickPlate;
            this.variables = new Variables(Fastener, PreDrilled, Pk, Alfa, WoodType, T1, T_steel, N);
            this.Faxrk_upperLimit = this.CalcFaxrkUpperLimitValue(Fastener);
        }

        private double CalcFaxrkUpperLimitValue(Fastener fastener)
        {
            string type = fastener.type;
            string nailType = fastener.nailType;

            if (type == "nail" && nailType == "round")
            {
                return 0.15;
            }
            else if (type == "nail" && nailType == "square")
            {
                return 0.25;
            }
            else if (type == "nail")
            {
                return 0.50;
            }
            else if (type == "screw")
            {
                return 1;
            }
            else if (type == "bolt")
            {
                return 0.25;
            }
            else if (type == "dowel")
            {
                return 0;
            }
            return 1;
        }

        public double FvrkSingleShear()
        {
            double Fvrk;
            //Mode a
            double Fvrk1 = 0.4 * variables.fhk * t1 * fastener.d;
            Fvrk = Fvrk1;

            //Mode b
            double Fyrk2 = (1.15 * Math.Sqrt(2 * variables.Myrk * variables.fhk * fastener.d));
            double Fvrk2 = Math.Min( Fyrk2 + variables.Faxrk / 4, (1 + this.Faxrk_upperLimit) * Fyrk2);
            Fvrk = Math.Min(Fvrk, Fvrk2);

            //Mode c
            double Fyrk3 = (variables.fhk * t1 * fastener.d * (Math.Sqrt(2 + (4 * variables.Myrk) / (variables.fhk * Math.Pow(t1, 2) * fastener.d)) - 1));
            double Fvrk3 = Math.Min( Fyrk3 + variables.Faxrk / 4, (1 + this.Faxrk_upperLimit) * Fyrk3);
            Fvrk = Math.Min(Fvrk, Fvrk3);

            //Mode d
            double Fyrk4 = (2.3 * Math.Sqrt(variables.Myrk * variables.fhk * fastener.d));
            double Fvrk4 = Math.Min( Fyrk4 + variables.Faxrk / 4, (1 + this.Faxrk_upperLimit) * Fyrk4);
            Fvrk = Math.Min(Fvrk, Fvrk4);

            //Mode e
            double Fvrk5 = variables.fhk * t1 * fastener.d;
            Fvrk = Math.Min(Fvrk, Fvrk5);

            return Fvrk;
        }

        public double FvrkDoubleShear()
        {
            double Fvrk;
            //Mode f
            double Fvrk1 = variables.fhk * t1 * fastener.d;
            Fvrk = Fvrk1;

            //Mode g
            double Fyrk2 = (variables.fhk * t1 * fastener.d * (Math.Sqrt(2 + (4 * variables.Myrk) / (variables.fhk * Math.Pow(t1, 2) * fastener.d)) - 1));
            double Fvrk2 = Math.Min( Fyrk2 + variables.Faxrk / 4, (1 + this.Faxrk_upperLimit) * Fyrk2);
            Fvrk = Math.Min(Fvrk, Fvrk2);

            //Mode h
            double Fyrk3 = (2.3 * Math.Sqrt(variables.Myrk * variables.fhk * fastener.d));
            double Fvrk3 = Math.Min( Fyrk3 + variables.Faxrk / 4, (1 + this.Faxrk_upperLimit) * Fyrk3);
            Fvrk = Math.Min(Fvrk, Fvrk3);

            //Mode j/l
            double Fvrk4 = 0.5 * variables.fh2k * fastener.d;
            Fvrk = Math.Min(Fvrk, Fvrk4);

            if (!thickPlate)
            {
                //Mode k
                double Fyrk5 = (1.15 * Math.Sqrt(2 * variables.Myrk * variables.fhk * fastener.d));
                double Fvrk5 = Math.Min( Fyrk5 + variables.Faxrk / 4, (1 + this.Faxrk_upperLimit) * Fyrk5);
                Fvrk = Math.Min(Fvrk, Fvrk5);
            }
            if (thickPlate)
            {
                //Mode m
                double Fyrk6 = (2.3 * Math.Sqrt(variables.Myrk * variables.fhk * fastener.d));
                double Fvrk6 = Math.Min( Fyrk6 + variables.Faxrk / 4, (1 + this.Faxrk_upperLimit) * Fyrk6);
                Fvrk = Math.Min(Fvrk, Fvrk6);

            }

            return Fvrk;
        }
    }
}
