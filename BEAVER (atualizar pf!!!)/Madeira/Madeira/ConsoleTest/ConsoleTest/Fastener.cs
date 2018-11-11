using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madeira
{
    public struct Fastener
    {
        public double d;
        public double dh;
        public double l;
        public double fu;
        public string type;
        public bool smooth;
        public string nailType;
        public double t_thread;

        public Fastener(string fastenerType, double D, double Dh, double L, bool Smooth,double Fu, string NailType)
        {
            d = D;
            dh = Dh;
            l = L;
            fu = Fu;
            type = fastenerType;
            smooth = Smooth;
            nailType = NailType;
            if ( type == "screw")
            {
                t_thread = l * 2 / 3;
            } else
            {
                t_thread = 0;

            }
        }
    }
}
