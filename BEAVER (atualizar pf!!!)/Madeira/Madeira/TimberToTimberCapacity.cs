using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Madeira.Connections
{
    class TimberToTimberCapacity
    {
        public double t1;
        public double t2;
        public double d;
        public string type;
        public bool smooth;
        public double alfa;
        public string timberMaterial;
        public string connectorMaterial;
        public bool preDrilled;
        public double tpen;
        public double dh;

        public TimberToTimberCapacity() {}

        public TimberToTimberCapacity(
            double T1,
            double T2,
            double D,
            string Type,
            bool Smooth,
            double Alfa,
            string TimberMaterial,
            string ConnectorMaterial,
            bool PreDrilled,
            double Tpen,
            double Dh
        ) {
            t1 = T1;
            t2 = T2;
            d = D;
            type = Type;
            smooth = Smooth;
            alfa = Alfa;
            timberMaterial = TimberMaterial;
            connectorMaterial = ConnectorMaterial;
            preDrilled = PreDrilled;
            tpen = Tpen;
            dh = Dh;
        }

        public double FvkSingleShear(){
            double fu = 5;
            Variables Valores = new Variables();
            Valores.calcMyrk(d, fu, type, smooth);
            Valores.calcT2TFhk(type, preDrilled,);
            Valores.calcFaxrk();
            double Mryk = Valores.Myrk;
            double Fh1k = Valores.fh1k;
            double Fh2k = Valores.fh2k;
            double Beta = Valores.beta;
            double Faxrk = Valores.Faxrk;
            double Fvk;
            //1º modo
            double Fvk1 = Fh1k * t1 * d;
            Fvk = Fvk1;
            //2º modo
            double Fvk2 = Fh2k * t2 * d;
            Fvk = Math.Min(Fvk, Fvk2);
            //3º modo
            double Fvk3 = ((Fh1k * t1 * d) / (1 + Beta)) 
                * (Math.Sqrt(Beta + 2 * Math.Pow(Beta, 2) * (1 + (t2 / t1) + Math.Pow(t2 / t1, 2)) + Math.Pow(Beta, 3) * Math.Pow(t2 / t1, 2)) - Beta * (1 + (t2 / t1)))
                +Faxrk/4;
            Fvk = Math.Min(Fvk, Fvk3);
            //4º modo
            double Fvk4 = ((1.05 * Fh1k * t1 * d) / (2 + Beta)) 
                * (Math.Sqrt(2 * Beta * (1 + Beta) + ((4 * Beta * (2 + Beta) * Mryk) / (Fh1k * Math.Pow(t1, 2) * d))) - Beta) 
                + Faxrk / 4;
            Fvk = Math.Min(Fvk, Fvk4);
            //5º modo
            double Fvk5 = ((1.05 * Fh2k * t2 * d) / (2 + Beta))
                * (Math.Sqrt(2 * Beta * (1 + Beta) + ((4 * Beta * (2 + Beta) * Mryk) / (Fh2k * Math.Pow(t2, 2) * d))) - Beta)
                + Faxrk / 4;
            Fvk = Math.Min(Fvk, Fvk5);
            //6º modo
            double Fvk6 = 1.15 * Math.Sqrt((2 * Beta) / (1 + Beta)) 
                * Math.Sqrt(2 * Mryk * Fh1k * d) 
                + Faxrk / 4;
            Fvk = Math.Min(Fvk, Fvk6);
            return Fvk;
        }
    }
}
