using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper;
using Rhino.Geometry;
using System.IO;
using System.Drawing;

namespace BeaverConections
{
    public class MODELOT2T : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public MODELOT2T()
          : base("Timber to Timber", "T to T",
              "Analysis of ductile failure of timber connections according to Eurocode 5.",
              "Beaver", "T2T Connections")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Timber depth 1", "t1", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Timber depth 2", "t2", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Alpha", "a", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Number of screws", "n", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Diameter", "d", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Diameter", "dh", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Lenght", "l", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Threaded lenght", "l_th", "", GH_ParamAccess.item);
            pManager.AddTextParameter("FastType", "ftype", "", GH_ParamAccess.item);
            pManager.AddIntegerParameter("WoodType", "wtype", "", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Pre Drilled", "pdrill", "", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Single or Double Shear", "S/D", "false for Single Shear, true for Double", GH_ParamAccess.item);
            pManager.AddNumberParameter("kMod", "kmod", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Vrd", "Vrd", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Nrd", "Nrd", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_DoubleParam("Caracteristic Shear Strenght", "Fvrk", "");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Component = this;
            GrasshopperDocument = this.OnPingDocument();
            if (Component.Params.Input[9].SourceCount == 0)
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
                this.Component.Params.Input[9].AddSource(vallist);
            }
            //AQUI COMEÇA O PLGUIN MESMO
            double t1 = 0;
            double t2 = 0;
            double a = 0;
            double n = 0;
            double d = 0;
            double dh = 0;
            double l = 0;
            double lt = 0;
            string type = "";
            int wood = 0;
            bool pdrill = false;
            double pk = 0;
            bool sd = false;
            double kmod = 0 ;
            double Nrd = 0;
            double Vrd = 0;
            if (!DA.GetData<double>(0, ref t1)) { return; }
            if (!DA.GetData<double>(1, ref t2)) { return; }
            if (!DA.GetData<double>(2, ref a)) { return; }
            if (!DA.GetData<double>(3, ref n)) { return; }
            if (!DA.GetData<double>(4, ref d)) { return; }
            if (!DA.GetData<double>(5, ref dh)) { return; }
            if (!DA.GetData<double>(6, ref l)) { return; }
            if (!DA.GetData<double>(7, ref lt)) { return; }
            if (!DA.GetData<string>(8, ref type)) { return; }
            if (!DA.GetData<int>(9, ref wood)) { return; }
            if (!DA.GetData<bool>(11, ref sd)) { return; }
            if (!DA.GetData<double>(12, ref kmod)) { return; }
            if (!DA.GetData<double>(13, ref Vrd)) { return; }
            if (!DA.GetData<double>(14, ref Nrd)) { return; }
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
            //CALCULO DAS LIGAÇÕES
            Fastener fast = new Fastener(type, d, dh, l, true, 1000);
            TimberToTimberCapacity analysis = new TimberToTimberCapacity(fast, t1, t2, a, woodtype, "steel", pdrill, pk, pk, dh, woodtype, lt, n);
            double fvd = 0;
            if (sd == false)
            {
                fvd = kmod*analysis.FvkSingleShear()/1.3;
            }
            else
            {
                fvd = kmod*analysis.FvkDoubleShear()/1.3;
            }
            double faxd =kmod* analysis.variables.Faxrk/1.3;
            double DIV = 0;
            if (fast.smooth == true)
            {
                DIV = 1000*Nrd / faxd + 1000*Vrd / fvd;
            }
            else
            {
                DIV = Math.Pow(1000*Nrd / faxd,2) + Math.Pow(1000*Vrd / fvd,2);
            }
            DA.SetData(0, DIV);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("8ada5c3d-ef08-45f0-b10c-e7f9b168b0ae"); }
        }
    }
}
