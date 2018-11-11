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
            alfa = Alfa;
            timberMaterial = TimberMaterial;
            connectorMaterial = ConnectorMaterial;
            preDrilled = PreDrilled;
            tpen = Tpen;
            dh = Dh;
        }

        public double Fvk(){

            Variables va = new Variables();

        }
    }
}
