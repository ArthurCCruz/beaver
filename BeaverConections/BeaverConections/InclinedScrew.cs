using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.IO;
using System.Drawing;

namespace BeaverConections
{
    public class InclinedScrew : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;

        /// <summary>
        /// Initializes a new instance of the InclinedScrew class.
        /// </summary>
        public InclinedScrew()
          : base("Inclined Screwed Connections", "X or // Screwed",
              "Analysis of inclined screwed connections (axially loaded screws)",
              "Beaver", "Axially Loaded Fasteners")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Frd", "Frd", "Total Force aplied on the connection parallel to grain", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Nscrews", "Nscrews", "Number of screws on connection", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Headside Member Depth", "t1", "t1 element can be Timber or Steel", GH_ParamAccess.item, 20);
            pManager.AddNumberParameter("Pointside Member Depth", "t2", "tw element must be Timber", GH_ParamAccess.item, 20);
            pManager.AddNumberParameter("Connection Type", "X or //; T2T or S2T", "input 0 for Xs and T2T; 1 for //s and T2T; 2 for //s and S2T", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("AlphaFast", "afast", "Screw Angle related to the grain", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Screw Diameter", "d", "Screw Shank Diameter", GH_ParamAccess.item, 8);
            pManager.AddNumberParameter("Screw Head Diameter", "dh", "Screw Head Diameter", GH_ParamAccess.item, 8);
            pManager.AddNumberParameter("Screw Length", "l", "Screw Total Length", GH_ParamAccess.item, 100);
            pManager.AddNumberParameter("Screw Threaded Length", "lt", "Screw Threaded Length", GH_ParamAccess.item, 50);
            pManager.AddIntegerParameter("WoodType", "wtype", "", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("kMod", "kmod", "", GH_ParamAccess.item, 0.6);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Component = this;
            GrasshopperDocument = this.OnPingDocument();
            if (Component.Params.Input[8].SourceCount == 0)
            {
                                             
                //instantiate  new value list
                var vallist = new Grasshopper.Kernel.Special.GH_ValueList();
                vallist.CreateAttributes();

                //customise value list position
                int inputcount = this.Component.Params.Input[8].SourceCount;
                //vallist.Attributes.Pivot = new PointF((float)this.Component.Attributes.DocObject.Attributes.Bounds.Left - vallist.Attributes.Bounds.Width - 30, 
                //    (float)this.Component.Params.Input[1].Attributes.Bounds.Y + inputcount * 30);
                vallist.Attributes.Pivot = new PointF(Component.Attributes.DocObject.Attributes.Bounds.Left - vallist.Attributes.Bounds.Width - 30, Component.Params.Input[8].Attributes.Bounds.Y + inputcount * 30);
                //populate value list with our own data
                vallist.ListItems.Clear();
                var item1 = new Grasshopper.Kernel.Special.GH_ValueListItem("GL 24h", "0");
                var item2 = new Grasshopper.Kernel.Special.GH_ValueListItem("GL 28h", "1");
                var item3 = new Grasshopper.Kernel.Special.GH_ValueListItem("GL 32h", "2");
                var item4 = new Grasshopper.Kernel.Special.GH_ValueListItem("GL 24c", "3");
                var item5 = new Grasshopper.Kernel.Special.GH_ValueListItem("GL 28c", "4");
                var item6 = new Grasshopper.Kernel.Special.GH_ValueListItem("GL 32c", "5");
                var item7 = new Grasshopper.Kernel.Special.GH_ValueListItem("GL CROSSLAM", "6");
                var item8 = new Grasshopper.Kernel.Special.GH_ValueListItem("GL ITA", "7");
                vallist.ListItems.Add(item1);
                vallist.ListItems.Add(item2);
                vallist.ListItems.Add(item3);
                vallist.ListItems.Add(item4);
                vallist.ListItems.Add(item5);
                vallist.ListItems.Add(item6);
                vallist.ListItems.Add(item7);
                vallist.ListItems.Add(item8);

                //Until now, the slider is a hypothetical object.
                // This command makes it 'real' and adds it to the canvas.
                GrasshopperDocument.AddObject(vallist, false);

                //Connect the new slider to this component
                this.Component.Params.Input[13].AddSource(vallist);
            }

            double t1 = 0;
            double t2 = 0;
            double alfast = 0;
            double Nscrews = 0;
            double d = 0;
            double dh = 0;
            double l = 0;
            double lt = 0;
            int wood = 0;
            double pk = 0;
            double kmod = 0;
            double Frd = 0;


            if (!DA.GetData<double>(0, ref Frd)) { return; }
            if (!DA.GetData<double>(1, ref Nscrews)) { return; }
            if (!DA.GetData<double>(2, ref Frd)) { return; }
            if (!DA.GetData<double>(3, ref Frd)) { return; }
            if (!DA.GetData<double>(4, ref Ctype)) { return; }
            if (!DA.GetData<double>(5, ref afast)) { return; }
            if (!DA.GetData<double>(6, ref d)) { return; }
            if (!DA.GetData<double>(7, ref dh)) { return; }
            if (!DA.GetData<double>(8, ref l)) { return; }
            if (!DA.GetData<double>(9, ref lt)) { return; }
            if (!DA.GetData<int>(10, ref wood)) { return; }
            if (!DA.GetData<double>(11, ref kmod)) { return; }



            //Pegar valores da Madeira do Excel
            string text = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            text = Path.Combine(Directory.GetParent(text).FullName, "Plug-ins");
            var reader = new StreamReader(File.OpenRead(text + "\\Madeira\\MLCPROP.csv"));
            int cont = -1;
            bool stop = false;
            string woodtype = "";
            while (!reader.EndOfStream || stop == false)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                if (cont == wood)
                {
                    pk = 1000 * Double.Parse(values[7]);
                    woodtype = values[13];
                    stop = true;
                }
                cont++;
            }



            double f_ax_k = 3.6 * 0.001 * Math.Pow(pk, 1.5);



        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("fa9b10f0-7ef1-4bc9-a379-bcf0b6f7fc8d"); }
        }
    }
}