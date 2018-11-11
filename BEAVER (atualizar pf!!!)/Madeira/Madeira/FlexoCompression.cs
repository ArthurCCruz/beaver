using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.IO;
using System.Drawing;

namespace Madeira
{
    public class FlexoCompression : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public FlexoCompression()
          : base("Flexo-Compressão", "Flex-Compr",
             "Verificação de perfil solicitado à flexo-compressão pelo Eurocode 5",
              "Madeira", "Verificação")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Nd", "Nd", "Normal de compressão paralela à fibra", GH_ParamAccess.item,0);
            pManager.AddNumberParameter("Myd", "Myd", "Momento fletor solicitante no eixo y", GH_ParamAccess.item,0);
            pManager.AddNumberParameter("Mzd", "Mzd", "Momento fletor solicitante no eixo z", GH_ParamAccess.item,0);
            pManager.AddNumberParameter("Base", "b", "Base da Seção [cm]", GH_ParamAccess.item,10);
            pManager.AddNumberParameter("Altura", "h", "Altura da Seção [cm]", GH_ParamAccess.item,10);
            pManager.AddNumberParameter("Comprimento", "l", "Comprimento do Elemento [m]", GH_ParamAccess.item,4);
            pManager.AddNumberParameter("Coeficiente de Flambagem Lateral", "kflam", "lef/l do Elemento", GH_ParamAccess.item,1);
            pManager.AddNumberParameter("Coeficiente de Modificação", "Kmod", "Coeficiente que relaciona umidade local, temperatura local, tipo de madeira e permanência da solicitação (vide Eurocode 5)", GH_ParamAccess.item, 0.6);
            pManager.AddIntegerParameter("Material", "Material", "Material a ser verificado o perfil", GH_ParamAccess.item, 0);
            
            // pManager.AddNumberParameter("Km da seção", "Km", "Coeficiente determinado pelo tipo de seção (0.7 para retangulares e 1.0 para outras)", GH_ParamAccess.item,0.7);
            // pManager.AddNumberParameter("Gama m", "Ym", "Coeficiente de redução do material", GH_ParamAccess.item);
            // pManager.AddNumberParameter("Resistência à Compressão", "fc0k", "Resistência à Compressão paralela à fibra [KN/cm2]", GH_ParamAccess.item);
            // pManager.AddNumberParameter("Resistência à Flexão", "fmk", "Resistência à Flexão [KN/cm2]", GH_ParamAccess.item);
            //pManager.AddNumberParameter("Módulo de Elasticidade", "E05", "Módulo de Young do material, que caracteriza sua rigidez à um escoamento de 5% [KN/cm2]", GH_ParamAccess.item);
            // pManager.AddNumberParameter("Beta c", "Bc", "Coeficiente influente na analise de pilares, 0.1 para MLC e 0.2 para solido", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_DoubleParam("DIVY", "DIVY", "Verificação da razão das tensões solicitantes sobre as resistentes com efeitos majorados em y");
            pManager.Register_DoubleParam("DIVZ", "DIVZ", "Verificação da razão das tensões solicitantes sobre as resistentes com efeitos majorados em z");
            pManager.Register_DoubleParam("Lambida Relativo m", "lamm", "Para saber se o elemento se comporta como pilar (<0.75) ou viga (>0.75)");
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
                this.Component.Params.Input[8].AddSource(vallist);
            }
            string text = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            text = Path.Combine(Directory.GetParent(text).FullName, "Plug-ins");
            var reader = new StreamReader(File.OpenRead(text + "\\Madeira\\MLCPROP.csv"));
            //inputs 
            double Nd = 0;
            double Myd = 0;
            double Mzd = 0;
            double b = 0;
            double h = 0;
            double l = 0;
            double kflam = 0;
            double Kmod = 0;
            double Km = 0.7;
            double Ym = 0;
            double fc0k = 0;
            double fmk = 0;
            double E05 = 0;
            double Bc = 0.2;
            int test = 0;
            if (!DA.GetData<double>(0, ref Nd)) { return; }
            if (!DA.GetData<double>(1, ref Myd)) { return; }
            if (!DA.GetData<double>(2, ref Mzd)) { return; }
            if (!DA.GetData<double>(3, ref b)) { return; }
            if (!DA.GetData<double>(4, ref h)) { return; }
            if (!DA.GetData<double>(5, ref l)) { return; }
            if (!DA.GetData<double>(6, ref kflam)) { return; }
            if (!DA.GetData<int>(8, ref test)) { return; }
            if (!DA.GetData<double>(7, ref Kmod)) { return; }
            int cont = -1;
            bool stop = false;
            
            while (!reader.EndOfStream || stop == false)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                if (cont == test)
                {
                    Ym = Double.Parse(values[12]);
                    fc0k = Double.Parse(values[1]);
                    fmk = Double.Parse(values[3]);
                    E05 = Double.Parse(values[9]);
                    if (values[13] == "MLC")
                    {
                        Bc = 0.1;
                    }
                    stop = true;
                }
                cont++;
            }
            //Definição de valores geométricos
            double A = h * b;
            double Iy = b * Math.Pow(h, 3) / 12;
            double Iz = h * Math.Pow(b, 3) / 12;
            double Wy = Iy * (2 / h);
            double Wz = Iz * (2 / b);
            double ry = Math.Sqrt(Iy / A);
            double rz = Math.Sqrt(Iz / A);
            double lamy = ry / (100 * l);
            double lamz = rz / (100 * l);
            double lef = kflam * l * 100;

            //Definição de valores do material (ver se os sigmas devem mesmo serem multiplicados por 100)
            double lampi = Math.Sqrt(fc0k / E05) / Math.PI;
            double fc0d = Kmod * fc0k / Ym;
            double fmd = Kmod * fmk / Ym;
            double lamyrel = lamy * lampi;
            double lamzrel = lamz * lampi;
            double sigN = Nd / A;
            double sigMy = 100 * Myd / Wy;
            double sigMz = 100 * Mzd / Wz;
            double G05 = E05 / 16;

            //Definição dos valores de cálculo necessários para verificação em pilares ou vigas (exclui parte em que dizia que lamrely=lamm e lamrelz=sgmcrit)
            double sigMcrit = (Math.PI * Math.Pow(b, 2) * Math.Sqrt(E05 * G05 * (1 - 0.63 * (b / h)))) / (h * lef);
            double lamm = Math.Sqrt(fmk / sigMcrit);
            double ky = 0.5 * (1 + Bc * (lamyrel - 0.3) + Math.Pow(lamyrel, 2));
            double kz = 0.5 * (1 + Bc * (lamzrel - 0.3) + Math.Pow(lamzrel, 2));
            double kyc = 1 / (ky + Math.Sqrt(Math.Pow(ky, 2) - Math.Pow(lamyrel, 2)));
            double kzc = 1 / (kz + Math.Sqrt(Math.Pow(kz, 2) - Math.Pow(lamzrel, 2)));

            //Verificação de comportamento de Pilares
            if (lamm < 0.75)
            {
                if (lamyrel <= 0.3 && lamzrel <= 0.3)
                {
                    double DIVY = Math.Pow(sigN / fc0d, 2) + (sigMy / fmd) + Km * (sigMz / fmd);
                    double DIVZ = Math.Pow(sigN / fc0d, 2) + Km * (sigMy / fmd) + (sigMz / fmd);
                    DA.SetData(0, DIVY);
                    DA.SetData(1, DIVZ);
                }
                else
                {
                    List<double> divs = new List<double>();
                    double DIVY = (sigN / (kyc * fc0d)) + (sigMy / fmd) + Km * (sigMz / fmd);
                    double DIVZ = (sigN / (kzc * fc0d)) + Km * (sigMy / fmd) + (sigMz / fmd);
                    DA.SetData(0, DIVY);
                    DA.SetData(1, DIVZ);
                }
            }
            //Verificação de comportamento de Vigas
            else
            {

                if (lamm >= 0.75 && lamm < 1.4)
                {
                    double kcrit = 1.56 - 0.75 * lamm;
                    double DIVY = Math.Pow(sigMy / (kcrit * fmd), 2) + (sigN / (kzc * fc0d));
                    double DIVZ = 0;
                    DA.SetData(0, DIVY);
                    DA.SetData(1, DIVZ);
                }
                if (lamm >= 1.4)
                {
                    double kcrit = 1 / Math.Pow(lamm, 2);
                    double DIVY = Math.Pow(sigMy / (kcrit * fmd), 2) + (sigN / (kzc * fc0d));
                    double DIVZ = 0;
                    DA.SetData(0, DIVY);
                    DA.SetData(1, DIVZ);
                }
            }

            //Outros Outputs
            DA.SetData(2, lamm);

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
                return Properties.Resources.flexao;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("019a3f0d-52c4-4ad4-8fdd-02b1b23249a7"); }
        }
    }
}