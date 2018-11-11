using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Madeira;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // t2t();
            t2s();
        }

        private static void t2s()
        {
            double d = 8;
            string type = "screw";
            double fu = 400;
            double dh = 7.54;
            bool smooth = false;
            double l = 130;
            string nailType = "round";
            var fastener = new Fastener(type, d, dh, l, smooth, fu, nailType);
            bool preDrilled = true;
            double pk1 = 340;
            double alfa = 90;
            string woodType = "mlc";
            double t1 = 8;
            double t2 = 150;
            double n = 4;
            var variables = new Madeira.Variables(fastener, preDrilled, pk1, alfa, woodType, t1, t2, n);
            var T2SCapacity = new Madeira.TimberToSteelCapacity(fastener, preDrilled, pk1, alfa, woodType, t2, t1, n, true);
            double cap = T2SCapacity.FvrkSingleShear();
            Console.WriteLine("Myrk: {0}", variables.Myrk);
            Console.WriteLine("fhk: {0}", variables.fhk);
            Console.WriteLine("Faxrk: {0}", variables.Faxrk);
            Console.WriteLine("T2T cap: {0}", cap);
            //var spacements = new BrittleFailure(fastener, pk1, alfa, preDrilled);
            //Console.WriteLine("a1: {0}", spacements.a1);
            //Console.WriteLine("a1_n: {0}", spacements.a1_n);
            //Console.WriteLine("a2: {0}", spacements.a2);
            //Console.WriteLine("a3t: {0}", spacements.a3t);
            //Console.WriteLine("a3c: {0}", spacements.a3c);
            //Console.WriteLine("a4t: {0}", spacements.a4t);
            //Console.WriteLine("a4c: {0}", spacements.a4c);
        }

        private static void t2t()
        {
            double d = 3.35;
            string type = "nail";
            double fu = 600;
            double dh = 7.54;
            bool smooth = true;
            double l = 65;
            string nailType = "round";
            var fastener = new Fastener(type, d, dh, l, smooth, fu, nailType);
            bool preDrilled = false;
            double pk1 = 340;
            double pk2 = 340;
            double alfa = 0;
            string woodType = "mlc";
            double t1 = 36;
            double t2 = 50;
            double n = 1;
            var variables = new Madeira.Variables(fastener, preDrilled, pk1, pk2, alfa, woodType, t1, t2, n);
            var T2TCapacity = new Madeira.TimberToTimberCapacity(fastener, t1, fastener.l - t1, 0, "", "", preDrilled, pk1, pk2, t1, "MLC", n);
            double cap = T2TCapacity.FvkSingleShear();
            Console.WriteLine("Myrk: {0}", variables.Myrk);
            Console.WriteLine("fh1k: {0}", variables.fh1k);
            Console.WriteLine("fh2k: {0}", variables.fh2k);
            Console.WriteLine("beta: {0}", variables.beta);
            Console.WriteLine("Faxrk: {0}", variables.Faxrk);
            Console.WriteLine("T2T cap: {0}", cap);
            var spacements = new BrittleFailure(fastener, pk1, alfa, preDrilled);
            Console.WriteLine("a1: {0}", spacements.a1);
            Console.WriteLine("a1_n: {0}", spacements.a1_n);
            Console.WriteLine("a2: {0}", spacements.a2);
            Console.WriteLine("a3t: {0}", spacements.a3t);
            Console.WriteLine("a3c: {0}", spacements.a3c);
            Console.WriteLine("a4t: {0}", spacements.a4t);
            Console.WriteLine("a4c: {0}", spacements.a4c);
        }
    }   
}