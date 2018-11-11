using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.IO;
using System.Drawing;

namespace BeaverConections
{
    public class verBFcs : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;
        /// <summary>
        /// Initializes a new instance of the verBFcs class.
        /// </summary>
        public verBFcs()
          : base("Brittle Failure", "Brittle Failure",
              "Verifies Minimum Fastener Distances to prevent from Brittle Failure",
              "Beaver", "T2T Connections")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Fastener Type", "FType", "Fastener Type: input nail, screw or bolt as text", GH_ParamAccess.item, "screw");
            pManager.AddNumberParameter("Diameter", "d", "Fastener Diameter, in the case os screws equivalent to shank diameter", GH_ParamAccess.item, 8);
            pManager.AddIntegerParameter("WoodType", "wtype", "", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Force Alpha", "alpha", "Force Angle related to the grain", GH_ParamAccess.item, 0);
            pManager.AddBooleanParameter("Pre Drilled", "pdrill", "", GH_ParamAccess.item, false);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_DoubleParam("a1", "a1", "Minimum Horizontal Spacing Between Fasteners");
            pManager.Register_DoubleParam("a1best", "a1best", "Best Horizontal Spacing Between Fasteners. Above this condition the effective number of fasteners will be equal to the real number of fasteners, but minimum a1 spacing must always be respected");
            pManager.Register_DoubleParam("a2", "a2", "Minimum Vertical Spacing Between Fasteners");
            pManager.Register_DoubleParam("a3c", "a3c", "Minimum Spacing Between Last Fastener and the Cutted Edge of the Timber Element. (if alpha points towards the cutted edge use a3t, if alpha does not points towards the cutted edge use a3c)");
            pManager.Register_DoubleParam("a3t", "a3t", "Minimum Spacing Between Last Fastener and the Cutted Edge of the Timber Element. (if alpha points towards the cutted edge use a3t, if alpha does not points towards the cutted edge use a3c)");
            pManager.Register_DoubleParam("a4c", "a4c", "Minimum Spacing Between Last Fastener and the lower/upper Edge of the Timber Element. (alpha points towards the a4t lowwer/upper edge, while it does not poits towards the lower/upper a4c edge)");
            pManager.Register_DoubleParam("a4t", "a4t", "Minimum Spacing Between Last Fastener and the Cutted Edge of the Timber Element. (alpha points towards the a4t lowwer/upper edge, while it does not poits towards the lower/upper a4c edge)");

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Component = this;
            GrasshopperDocument = this.OnPingDocument();
            if (Component.Params.Input[2].SourceCount == 0)
            {
                //instantiate  new value list
                var vallist = new Grasshopper.Kernel.Special.GH_ValueList();
                vallist.CreateAttributes();

                //customise value list position
                int inputcount = this.Component.Params.Input[2].SourceCount;
                //vallist.Attributes.Pivot = new PointF((float)this.Component.Attributes.DocObject.Attributes.Bounds.Left - vallist.Attributes.Bounds.Width - 30, 
                //    (float)this.Component.Params.Input[1].Attributes.Bounds.Y + inputcount * 30);
                vallist.Attributes.Pivot = new PointF(Component.Attributes.DocObject.Attributes.Bounds.Left - vallist.Attributes.Bounds.Width - 30, Component.Params.Input[2].Attributes.Bounds.Y + inputcount * 30);
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
                this.Component.Params.Input[2].AddSource(vallist);
            }


            string type = "screw";
            double d = 0;
            double pk = 0;
            double alfa = 0;
            bool pdrill = false;
            int wood = 0;

            
                        
            if (!DA.GetData<string>(0, ref type)) { return; }
            if (!DA.GetData<double>(1, ref d)) { return; }
            if (!DA.GetData<int>(2, ref wood)) { return; }
            if (!DA.GetData<double>(3, ref alfa)) { return; }
            if (!DA.GetData<bool>(4, ref pdrill)) { return; }

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
            //CALCULOS DE BRITTLE FAILURE (minimos espaçamentos aceitáveis)
            Fastener fast = new Fastener(type, d, -1, -1, true, -1);
            BrittleFailure analysis = new BrittleFailure(fast,pk,alfa,pdrill);
            double a1 = analysis.a1;
            double a2 = analysis.a2;
            double a3c = analysis.a3c;
            double a3t = analysis.a3t;
            double a4c = analysis.a4c;
            double a4t = analysis.a4t;
            double a1best = analysis.a1_n;

            DA.SetData(0, a1);
            DA.SetData(1, a1best);
            DA.SetData(2, a2);
            DA.SetData(3, a3c);
            DA.SetData(4, a3t);
            DA.SetData(5, a4c);
            DA.SetData(6, a4t);

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
            get { return new Guid("b7698978-409b-4474-a1f2-00fac6c092e7"); }
        }
    }
}