using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.IO;
using System.Drawing;

namespace Madeira
{
    public class Cortante : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;

        public Cortante()
          : base("Resistência à Cortante", "Cortante",
              "Verificação de perfil solicitado à força cortante pelo Eurocode 5",
              "Madeira", "Verificação")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddNumberParameter("Força Cortante", "V", "Força Cortante solicitante", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Altura", "h", "Altura da seção", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Largura", "b", "Largura da seção", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Kmod", "Kmod", "Kmod de acordo com o Eurocode", GH_ParamAccess.item, 0.6);
            pManager.AddIntegerParameter("Material", "Material", "Material a ser verificado o perfil", GH_ParamAccess.item, 0);
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_DoubleParam("Sigv/fvd", "DIV", "Razão entre a tensão de cisalhamento solicitante e a resistência do material ao cisalhamento paralelo à fibra");

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
            if (Component.Params.Input[4].SourceCount == 0)
            {
                //instantiate  new value list
                var vallist = new Grasshopper.Kernel.Special.GH_ValueList();
                vallist.CreateAttributes();

                //customise value list position
                int inputcount = this.Component.Params.Input[4].SourceCount;
                //vallist.Attributes.Pivot = new PointF((float)this.Component.Attributes.DocObject.Attributes.Bounds.Left - vallist.Attributes.Bounds.Width - 30, 
                //    (float)this.Component.Params.Input[1].Attributes.Bounds.Y + inputcount * 30);
                vallist.Attributes.Pivot = new PointF(Component.Attributes.DocObject.Attributes.Bounds.Left - vallist.Attributes.Bounds.Width - 30, Component.Params.Input[4].Attributes.Bounds.Y + inputcount * 30);
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
                this.Component.Params.Input[4].AddSource(vallist);
            }
            string text = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            text = Path.Combine(Directory.GetParent(text).FullName, "Plug-ins");
            var reader = new StreamReader(File.OpenRead(text + "\\Madeira\\MLCPROP.csv"));

            double V = 0;
            double h = 0;
            double b = 0;
            double Kmod = 0;
            double Gamm = 0;
            double Fvk = 0;
            int test = 0;
            if (!DA.GetData<double>(0, ref V)) { return; }
            if (!DA.GetData<double>(1, ref h)) { return; }
            if (!DA.GetData<double>(2, ref b)) { return; }
            if (!DA.GetData<double>(3, ref Kmod)) { return; }
            if (!DA.GetData(4, ref test)) { return; }
            int cont = -1;
            bool stop = false;
            while (!reader.EndOfStream || stop == false)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                if (cont == test)
                {
                    Fvk = Double.Parse(values[6]);
                    Gamm = Double.Parse(values[12]);
                    stop = true;
                }
                cont++;
            }
            double bef = 0.67 * b;
            double Sigv = (3/2)*(V/(bef*h));
            double fvd = Kmod * Fvk / Gamm;
            double Div = Sigv / fvd;
            DA.SetData(0, Div);
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
                return Properties.Resources.cortante;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("24a03e02-9505-44de-aca2-1d4b244a89a2"); }
        }
    }
}