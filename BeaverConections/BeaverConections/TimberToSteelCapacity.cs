using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaverConections
{
    class TimberToSteelCapacity
    {
        public Variables variables;
        public Fastener fastener;
        public bool preDrilled;
        public double pk;
        public double alfa;
        public double alfafast;
        public string woodType;
        public double t1;
        public double t_steel;
        public double t_thread;
        public double npar;
        public double npep;
        public bool thickPlate;
        public double Faxrk_upperLimit;
        public int SDt;
        public double a1;

        public TimberToSteelCapacity() {}

        public TimberToSteelCapacity(
            Fastener Fastener,
            bool PreDrilled,
            double Pk,
            double Alfa,
            double Alfafast,
            string WoodType, 
            double T1,
            double T_steel,
            double T_thread,
            double Npar,
            double Npep,
            int SD,
            double A1
        ) {
            this.fastener = Fastener;
            this.preDrilled = PreDrilled;
            this.pk = Pk;
            this.alfa = Alfa;
            this.woodType = WoodType;
            this.t1 = T1;
            this.t_steel = T_steel;
            this.npar = Npar;
            this.npep = Npep;
            this.SDt = SD;
            this.a1 = A1;
            this.variables = new Variables(Fastener, PreDrilled, Pk, Alfa, Alfafast,woodType,T1,T_steel,T_thread);
            this.Faxrk_upperLimit = this.CalcFaxrkUpperLimitValue(Fastener);
        }

        private double CalcFaxrkUpperLimitValue(Fastener fastener)
        {
            string type = fastener.type;

            if (type == "nail")
            {
                return 0.15;
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
            double nef = this.Nef();
            double n = npar * npep;
            double nalfa = (alfa / 90) * (n - nef) + nef;
            double Fvrk;
            //Mode a
            double Fvrk1 = nalfa * 0.4 * variables.fhk * t1 * fastener.d;
            Fvrk = Fvrk1;

            //Mode b
            double Fyrk2 = (1.15 * Math.Sqrt(2 * variables.Myrk * variables.fhk * fastener.d));
            double Fvrk2 = nalfa * Math.Min( Fyrk2 + variables.Faxrk / 4, (1 + this.Faxrk_upperLimit) * Fyrk2);
            Fvrk = Math.Min(Fvrk, Fvrk2);

            //Mode c
            double Fyrk3 = (variables.fhk * t1 * fastener.d * (Math.Sqrt(2 + (4 * variables.Myrk) / (variables.fhk * Math.Pow(t1, 2) * fastener.d)) - 1));
            double Fvrk3 = nalfa * Math.Min( Fyrk3 + variables.Faxrk / 4, (1 + this.Faxrk_upperLimit) * Fyrk3);
            Fvrk = Math.Min(Fvrk, Fvrk3);

            //Mode d
            double Fyrk4 = (2.3 * Math.Sqrt(variables.Myrk * variables.fhk * fastener.d));
            double Fvrk4 = nalfa * Math.Min( Fyrk4 + variables.Faxrk / 4, (1 + this.Faxrk_upperLimit) * Fyrk4);
            Fvrk = Math.Min(Fvrk, Fvrk4);

            //Mode e
            double Fvrk5 = nalfa * variables.fhk * t1 * fastener.d;
            Fvrk = Math.Min(Fvrk, Fvrk5);

            return Fvrk;
        }

        public double FvrkDoubleShear()
        {
            double nef = this.Nef();
            double n = npar * npep;
            double nalfa = (alfa / 90) * (n - nef) + nef;
            double Fvrk=0;
            if (SDt == 1)
            {
                //Mode f
                double Fvrk1 = nalfa*variables.fhk * t1 * fastener.d;
                Fvrk = Fvrk1;

                //Mode g
                double Fyrk2 = (variables.fhk * t1 * fastener.d * (Math.Sqrt(2 + (4 * variables.Myrk) / (variables.fhk * Math.Pow(t1, 2) * fastener.d)) - 1));
                double Fvrk2 =nalfa* Math.Min(Fyrk2 + variables.Faxrk / 4, (1 + this.Faxrk_upperLimit) * Fyrk2);
                Fvrk = Math.Min(Fvrk, Fvrk2);

                //Mode h
                double Fyrk3 = (2.3 * Math.Sqrt(variables.Myrk * variables.fhk * fastener.d));
                double Fvrk3 = nalfa* Math.Min(Fyrk3 + variables.Faxrk / 4, (1 + this.Faxrk_upperLimit) * Fyrk3);
                Fvrk = Math.Min(Fvrk, Fvrk3);
            }
            
            if (SDt == 2)
            {
                //Mode j/l
                double Fvrk4 = nalfa* 0.5 * variables.fh2k * fastener.d;
                Fvrk = Fvrk4;
                double multi = 0;
                if (this.t_steel<=0.5*this.fastener.d)
                {
                    multi = 1.62;
                }
                if (this.t_steel >= this.fastener.d)
                {
                    multi = 2.3;
                }
                else
                {
                    multi = 2.3 - 1.36 * (this.fastener.d - this.t_steel) / this.fastener.d;
                }
                //Mode k/m
                double Fyrk6 = (multi * Math.Sqrt(variables.Myrk * variables.fhk * fastener.d));
                double Fvrk6 = nalfa * Math.Min(Fyrk6 + variables.Faxrk / 4, (1 + this.Faxrk_upperLimit) * Fyrk6);
                Fvrk = Math.Min(Fvrk, Fvrk6);
            }
            

            return Fvrk;
        }
        public double Nef()
        {
            double d = this.fastener.d;
            double nef = 0;
            if (this.fastener.type == "nail" || (this.fastener.type == "screw" & this.fastener.d < 6))
            {
                double kef = 0;
                if (this.a1 >= 4 * d & this.a1 < 7 * d)
                {
                    kef = 0.5 - (0.5 - 0.7) * (4 * d - a1) / (4 * d - 7 * d);
                }
                if (this.a1 >= 7 * d & this.a1 < 10 * d)
                {
                    kef = 0.7 - (0.7 - 0.85) * (7 * d - a1) / (7 * d - 10 * d);
                }
                if (this.a1 >= 10 * d & this.a1 < 14 * d)
                {
                    kef = 0.85 - (0.85 - 1) * (10 * d - a1) / (10 * d - 14 * d);
                }
                if (this.a1 >= 14 * d)
                {
                    kef = 1;
                }
                nef = (Math.Pow(npar, kef)) * npep;
            }
            if (this.fastener.type == "bolt" || (this.fastener.type == "screw" & this.fastener.d > 6))
            {
                nef = Math.Min(npar, Math.Pow(npar, 0.9) * Math.Pow(a1 / (13 * d), 0.25)) * npep;
            }
            return nef;
        }

    }
}
