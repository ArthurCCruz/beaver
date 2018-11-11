using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Madeira;

namespace Madeira
{
    class TimberToTimberCapacity
    {
        public Fastener fastener;
        public double t1;
        public double tpen;
        public double alfa;
        public string timberMaterial;
        public string connectorMaterial;
        public bool preDrilled;
        public double pk1;
        public double pk2;
        public double t_head;
        public double t_thread;
        public double n;
        public string woodType;
        public double Faxrk_upperLimit;

        public Variables variables;

        public TimberToTimberCapacity() {}

        public TimberToTimberCapacity(
            Fastener Fastener,
            double T1,
            double Tpen,
            double Alfa,
            string TimberMaterial,
            string ConnectorMaterial,
            bool PreDrilled,
            double Pk1,
            double Pk2,
            double T_head,
            string WoodType,
            double N
        ) {
            this.t1 = T1;
            this.tpen = Tpen;
            this.alfa = Alfa;
            this.fastener = Fastener;
            this.timberMaterial = TimberMaterial;
            this.connectorMaterial = ConnectorMaterial;
            this.preDrilled = PreDrilled;
            this.pk1 = Pk1;
            this.pk2 = Pk2;
            this.woodType = WoodType;
            this.t_head = T_head;
            this.n = N;
            this.variables = new Variables(Fastener, PreDrilled, Pk1, Pk2, Alfa, WoodType, T1, Tpen, N);
            this.Faxrk_upperLimit = this.CalcFaxrkUpperLimitValue(Fastener);
        }

        private double CalcFaxrkUpperLimitValue(Fastener fastener)
        {
            string type = fastener.type;
            string nailType = fastener.nailType;
            
            if( type == "nail" && nailType == "round")
            {
                return 0.15;
            }
            else if ( type == "nail" && nailType == "square")
            {
                return 0.25;
            }
            else if ( type == "nail")
            {
                return 0.50;
            }
            else if ( type == "screw")
            {
                return 1;
            }
            else if ( type == "bolt")
            {
                return 0.25;
            }
            else if ( type == "dowel")
            {
                return 0;
            }
            return 1;
        }

        public double FvkSingleShear(){
            double Mryk = this.variables.Myrk;
            double Fh1k = this.variables.fh1k;
            double Fh2k = this.variables.fh2k;
            double Beta = this.variables.beta;
            double Faxrk = this.variables.Faxrk;
            double Fvk;
            //1º modo
            double Fvk1 = Fh1k * t1 * this.fastener.d;
            Fvk = Fvk1;
            //2º modo
            double Fvk2 = Fh2k * tpen * this.fastener.d;
            Fvk = Math.Min(Fvk, Fvk2);
            //3º modo
            double Fyk3 = ((Fh1k * t1 * this.fastener.d) / (1 + Beta))
                * (Math.Sqrt(Beta + 2 * Math.Pow(Beta, 2) * (1 + (tpen / t1) + Math.Pow(tpen / t1, 2)) + Math.Pow(Beta, 3) * Math.Pow(tpen / t1, 2)) - Beta * (1 + (tpen / t1)));

            double Fvk3 = Math.Min(Fyk3 + Faxrk / 4, (1 + this.Faxrk_upperLimit) * Fyk3);
            Fvk = Math.Min(Fvk, Fvk3);
            //4º modo
            double Fyk4 = ((1.05 * Fh1k * t1 * this.fastener.d) / (2 + Beta))
                * (Math.Sqrt(2 * Beta * (1 + Beta) + ((4 * Beta * (2 + Beta) * Mryk) / (Fh1k * Math.Pow(t1, 2) * this.fastener.d))) - Beta);

            double Fvk4 = Math.Min(Fyk4 + Faxrk / 4, (1 + this.Faxrk_upperLimit) * Fyk4);
            Fvk = Math.Min(Fvk, Fvk4);
            //5º modo
            double Fyk5 = ((1.05 * Fh2k * tpen * this.fastener.d) / (2 + Beta))
                * (Math.Sqrt(2 * Beta * (1 + Beta) + ((4 * Beta * (2 + Beta) * Mryk) / (Fh2k * Math.Pow(tpen, 2) * this.fastener.d))) - Beta);

            double Fvk5 = Math.Min(Fyk5 + Faxrk / 4, (1 + this.Faxrk_upperLimit) * Fyk5);
            Fvk = Math.Min(Fvk, Fvk5);
            //6º modo
            double Fyk6 = 1.15 * Math.Sqrt((2 * Beta) / (1 + Beta))
                * Math.Sqrt(2 * Mryk * Fh1k * this.fastener.d);

            double Fvk6 = Math.Min(Fyk6 + Faxrk / 4, (1 + this.Faxrk_upperLimit) * Fyk6);
            Fvk = Math.Min(Fvk, Fvk6);
            Console.WriteLine("Fvk1: {0}", Fvk1 );
            Console.WriteLine("Fvk2: {0}", Fvk2 );
            Console.WriteLine("Fvk3: {0}", Fvk3 );
            Console.WriteLine("Fvk4: {0}", Fvk4 );
            Console.WriteLine("Fvk5: {0}", Fvk5 );
            Console.WriteLine("Fvk6: {0}", Fvk6 );
            return Fvk;
        }

        public double FvkDoubleShear()
        {
            double Mryk = this.variables.Myrk;
            double Fh1k = this.variables.fh1k;
            double Fh2k = this.variables.fh2k;
            double Beta = this.variables.beta;
            double Faxrk = this.variables.Faxrk;
            double Fvk;
            // 1º mode
            double Fvk1 = Fh1k * t1 * this.fastener.d;
            Fvk = Fvk1;
            // 2º mode
            double Fvk2 = 0.5 * Fh2k * tpen * this.fastener.d;
            Fvk = Math.Min(Fvk, Fvk2);
            // 3º mode
            double Fyk3 = 1.05 * ((Fh1k * t1 * this.fastener.d) / (2 * Beta))
                * (Math.Sqrt(2 * Beta * (1 + Beta) + (4 * Beta * (2 + Beta) * Mryk) / (Fh1k * Math.Pow(t1, 2) * this.fastener.d)) - Beta);
            double Fvk3 = Math.Min(Fyk3 + Faxrk / 4, (1 + this.Faxrk_upperLimit ) * Fyk3);
            Fvk = Math.Min(Fvk, Fvk3);
            // 4º mode
            double Fyk4 = 1.15 * Math.Sqrt((2 * Beta) / (1 + Beta)) * Math.Sqrt(2 * Mryk * Fh1k * this.fastener.d);
            double Fvk4 = Math.Min(Fyk4 + Faxrk / 4, (1 + this.Faxrk_upperLimit ) * Fyk4);
            Fvk = Math.Min(Fvk, Fvk4);
            return Fvk;
        }
    }
}
