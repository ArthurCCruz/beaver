using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.IO;
using System.Drawing;

namespace BeaverConections
{
    public class BoltS2T : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;
        /// <summary>
        /// Initializes a new instance of the BoltS2T class.
        /// </summary>
        public BoltS2T()
          : base("BoltS2T", "Nickname",
              "Description",
              "Beaver", "T2S Conections")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Nrd", "Nrd", "Axial Force on Bolt", GH_ParamAccess.item, 0); //0
            pManager.AddNumberParameter("Vrd", "Vrd", "Shear Force on Bolt", GH_ParamAccess.item, 0); //1
            pManager.AddNumberParameter("Timber depth", "t", "", GH_ParamAccess.item, 20); //2
            pManager.AddNumberParameter("Steel depth", "tsteel", "", GH_ParamAccess.item, 20); //3
            pManager.AddNumberParameter("Alpha", "alpha", "Force Angle related to the grain of t", GH_ParamAccess.item, 0); //4
            pManager.AddNumberParameter("Bolts Parallel", "npar", "Number of Bolts parallel to the grain ", GH_ParamAccess.item, 1); //5
            pManager.AddNumberParameter("Bolts Perpendicullar", "npep", "Number of Bolts perpendicular to the grain ", GH_ParamAccess.item, 1); //6
            pManager.AddNumberParameter("Diamenter (mm)", "d", "Bolt Diameter", GH_ParamAccess.item, 8); //7
            pManager.AddNumberParameter("Washer Diameter (mm)", "dw", "Washer's diameter", GH_ParamAccess.item, 8); //8
            pManager.AddIntegerParameter("WoodType", "wtype", "", GH_ParamAccess.item, 0); //9
            pManager.AddBooleanParameter("Shear Type", "St", " 0 for Single Shear, 1 for Double with steel center member, 2 for Double with timber center member", GH_ParamAccess.item, false); //10
            pManager.AddNumberParameter("kMod", "kmod", "", GH_ParamAccess.item, 0.6); //11
            pManager.AddNumberParameter("Parallel Spacing", "a1", "", GH_ParamAccess.item, 0.6); //12
            pManager.AddNumberParameter("Edge Spacing", "a4", "Minimum upper/lower edge spacing", GH_ParamAccess.item, 0.6); //13


        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_DoubleParam("Caracteristic Shear Strenght", "Fvrd", "Connection Design Load Carrying Capacity per Shear Plane");
            pManager.Register_DoubleParam("Caracteristic Withdrawal capacity", "Faxrd", "Connection Design Withdrawal Capacity");
            pManager.Register_DoubleParam("DIV", "DIV", "Designed Load / Load capacity");
            pManager.Register_DoubleParam("DIV Steel", "DIVsteel", "Designed Load / Steel Plate Load capacity (ASTM A36)");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Component = this;
            GrasshopperDocument = this.OnPingDocument();
            if (Component.Params.Input[10].SourceCount == 0)
            {
                //instantiate  new value list
                var vallist = new Grasshopper.Kernel.Special.GH_ValueList();
                vallist.CreateAttributes();

                //customise value list position
                int inputcount = this.Component.Params.Input[12].SourceCount;
                //vallist.Attributes.Pivot = new PointF((float)this.Component.Attributes.DocObject.Attributes.Bounds.Left - vallist.Attributes.Bounds.Width - 30, 
                //    (float)this.Component.Params.Input[1].Attributes.Bounds.Y + inputcount * 30);
                vallist.Attributes.Pivot = new PointF(Component.Attributes.DocObject.Attributes.Bounds.Left - vallist.Attributes.Bounds.Width - 30, Component.Params.Input[12].Attributes.Bounds.Y + inputcount * 30);
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
                this.Component.Params.Input[10].AddSource(vallist);
            }
            if (Component.Params.Input[11].SourceCount == 0)
            {
                //instantiate  new value list
                var vallist = new Grasshopper.Kernel.Special.GH_ValueList();
                vallist.CreateAttributes();

                //customise value list position
                int inputcount = this.Component.Params.Input[12].SourceCount;
                //vallist.Attributes.Pivot = new PointF((float)this.Component.Attributes.DocObject.Attributes.Bounds.Left - vallist.Attributes.Bounds.Width - 30, 
                //    (float)this.Component.Params.Input[1].Attributes.Bounds.Y + inputcount * 30);
                vallist.Attributes.Pivot = new PointF(Component.Attributes.DocObject.Attributes.Bounds.Left - vallist.Attributes.Bounds.Width - 30, Component.Params.Input[14].Attributes.Bounds.Y + inputcount * 30);
                //populate value list with our own data
                vallist.ListItems.Clear();
                var item1 = new Grasshopper.Kernel.Special.GH_ValueListItem("Single Shear", "0");
                var item2 = new Grasshopper.Kernel.Special.GH_ValueListItem("Double Shear Steel In", "1");
                var item3 = new Grasshopper.Kernel.Special.GH_ValueListItem("Double Shear Steel Out", "2");
                //Until now, the slider is a hypothetical object.
                // This command makes it 'real' and adds it to the canvas.
                GrasshopperDocument.AddObject(vallist, false);

                //Connect the new slider to this component
                this.Component.Params.Input[11].AddSource(vallist);
            }

            //AQUI COMEÇA O PLGUIN MESMO
            double Nrd = 0;
            double Vrd = 0;
            double t = 0;
            double tsteel = 0;
            double alfa = 0;
            double n_par = 0;
            double n_pep = 0;
            double d = 0;
            double dw = 0;
            int wood = 0;
            int shear_type = 0;
            double kmod = 0;
            double a1 = 0;
            double a4 = 0;

            string woodtype = "";
            double fc90 = 0;

            //Constant Values
            string type = "bolt";
            bool pre_drilled = true;
            double pk = 0;
            double l = -1; //irrelevant
            bool smooth = true; //irrelevant
            double fu = 1000; //default
            double alfa_fast = -1;//irrelevant
            
            if (!DA.GetData<double>(0, ref Nrd)) { return; }
            if (!DA.GetData<double>(1, ref Vrd)) { return; }
            if (!DA.GetData<double>(2, ref t)) { return; }
            if (!DA.GetData<double>(3, ref tsteel)) { return; }
            if (!DA.GetData<double>(4, ref alfa)) { return; }
            if (!DA.GetData<double>(6, ref n_par)) { return; }
            if (!DA.GetData<double>(7, ref n_pep)) { return; }
            if (!DA.GetData<double>(8, ref d)) { return; }
            if (!DA.GetData<double>(8, ref dw)) { return; }
            if (!DA.GetData<int>(9, ref wood)) { return; }
            if (!DA.GetData<int>(10, ref shear_type)) { return; }
            if (!DA.GetData<double>(11, ref kmod)) { return; }
            if (!DA.GetData<double>(12, ref a1)) { return; }
            if (!DA.GetData<double>(13, ref a4)) { return; }

            string text = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            text = Path.Combine(Directory.GetParent(text).FullName, "Plug-ins");
            var reader = new StreamReader(File.OpenRead(text + "\\Madeira\\MLCPROP.csv"));
            int cont = -1;
            bool stop = false;
            while (!reader.EndOfStream || stop == false)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                if (cont == wood)
                {
                    pk = 1000 * Double.Parse(values[7]);
                    fc90 = 10 * Double.Parse(values[4]);
                    woodtype = values[13];
                    stop = true;
                }
                cont++;
            }

             //CALCULO DAS LIGAÇÕES
            var fast = new Fastener(type, d, dw, l, smooth, fu);
            var analysis = new TimberToSteelCapacity(fast, pre_drilled, pk, alfa, alfa_fast, woodtype, t, tsteel, fc90, n_par, n_pep, shear_type, a1);
            double fvd = 0;
            if (shear_type == 0)
            {
                fvd = kmod * analysis.FvrkSingleShear() / 1.3;
            }
            else
            {
                fvd = kmod * analysis.FvrkDoubleShear() / 1.3;
            }
            double faxd = kmod * analysis.variables.Faxrk / 1.3;
            double DIV = 0;
            DIV =  1000 * Vrd / fvd;

            //Steel Plate
            double Fsrd = Math.Min(1.2 * a4 * tsteel * 400 / 1.35, 2.4 * fast.d * tsteel * 400 / 1.35);
            double DIVsteel = 1000 * Vrd / Fsrd;

            DA.SetData(0, fvd);
            DA.SetData(1, faxd);
            DA.SetData(2, DIV);
            DA.SetData(3, DIVsteel);
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
            get { return new Guid("720ebda3-2e25-4560-a4e7-b55a6392d207"); }
        }
    }
}