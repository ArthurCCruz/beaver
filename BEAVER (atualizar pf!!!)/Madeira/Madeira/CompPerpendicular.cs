using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.IO;
using System.Drawing;

namespace Madeira
{
    public class CompPerpendicular : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;

        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public CompPerpendicular()
          : base("Resistência à Compressão Perpendicular", "Comp.Perpendicular",
              "Verificação de perfil solicitado à compressão perpendicular ou em ângulo segundo Eurocode 5",
              "Madeira", "Verificação")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Fcad", "Fcad", "Força de compressão perpendicular (ou angular) à fibra [kN]", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Ângulo de Fcad", "acomp", "ângulo de aplicação da força de compressão em relação à fibra [graus de 0 a 90] só existe em caso de cortes na seção comprimida, caso contrário usar resultante vertical de compressão e entrar com acomp = 90", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("l de aplicação de Fcad", "lFcad", "Comprimento de aplicação de Fcad em direção paralela à fibra [cm]", GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("distância 1", "d1", "Distância entre o lado esquerdo da apliação de Fcad e (o fim do elemento comprimido) ou (até outra força de compressão perpendicular para cima ou para baixo) [cm]", GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("d2 é uma distância com relação a uma força ou o fim do elemento?", "d1tipo", "entre 0 para (o fim do elemento) ou 1 para (outra força de compressão perpendicular)", GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("distância 2", "d2", "Distância entre o lado direito da apliação de Fcad e (o fim do elemento comprimido) ou (até outra força de compressão perpendicular para cima ou para baixo) [cm]", GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("d2 é uma distância com relação a uma força ou o fim do elemento?", "d2tipo", "entre 0 para (o fim do elemento) ou 1 para (outra força de compressão perpendicular)", GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("Base", "b", "Base da Seção [cm]", GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("Altura", "h", "Altura da Seção [cm]", GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("Coeficiente de Modificação", "Kmod", "Coeficiente que relaciona umidade local, temperatura local, tipo de madeira e permanência da solicitação (vide Eurocode 5)", GH_ParamAccess.item, 0.6);
            pManager.AddIntegerParameter("Material", "Material", "Material a ser verificado o perfil", GH_ParamAccess.item, 0);

            // pManager.AddNumberParameter("Km da seção", "Km", "Coeficiente determinado pelo tipo de seção (0.7 para retangulares e 1.0 para outras)", GH_ParamAccess.item,0.7);
            // pManager.AddNumberParameter("Gama m", "Ym", "Coeficiente de redução do material", GH_ParamAccess.item);
            // pManager.AddNumberParameter("Resistência à Compressão", "fc0k", "Resistência à Compressão paralela à fibra [KN/cm2]", GH_ParamAccess.item);
            // pManager.AddNumberParameter("Resistência à Compressão", "fc90k", "Resistência à Compressão perpendicular à fibra [KN/cm2]", GH_ParamAccess.item);
            // pManager.AddNumberParameter("Módulo de Elasticidade", "E05", "Módulo de Young do material, que caracteriza sua rigidez à um escoamento de 5% [KN/cm2]", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_DoubleParam("Sigc90d/(fc90d*kc90)", "DIV", "Razão entre a tensão de compressão perpendicular solicitante e a resistência do material à essa tensão");
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
                this.Component.Params.Input[8].AddSource(vallist);
            }
            string text = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            text = Path.Combine(Directory.GetParent(text).FullName, "Plug-ins");
            var reader = new StreamReader(File.OpenRead(text + "\\Madeira\\MLCPROP.csv"));

            //inputs 
            double Fcad = 0;
            double acomp = 0;
            double lFcad = 0;
            double d1 = 0;
            double d1tipo = 0;
            double d2 = 0;
            double d2tipo = 0;
            double b = 0;
            double h = 0;
            double Kmod = 0;
            double Ym = 0;
            double fc0k = 0;
            double fc90k = 0;
            double E05 = 0;
            int test = 0;
            if (!DA.GetData<double>(0, ref Fcad)) { return; }
            if (!DA.GetData<double>(1, ref acomp)) { return; }
            if (!DA.GetData<double>(2, ref lFcad)) { return; }
            if (!DA.GetData<double>(3, ref d1)) { return; }
            if (!DA.GetData<double>(3, ref d1tipo)) { return; }
            if (!DA.GetData<double>(4, ref d2)) { return; }
            if (!DA.GetData<double>(4, ref d2tipo)) { return; }
            if (!DA.GetData<double>(5, ref b)) { return; }
            if (!DA.GetData<double>(6, ref h)) { return; }
            if (!DA.GetData<double>(7, ref Kmod)) { return; }
            if (!DA.GetData<int>(8, ref test)) { return; }
            int cont = -1;
            bool stop = false;
            double tipodemadeira = 0;

            while (!reader.EndOfStream || stop == false)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                if (cont == test)
                {
                    Ym = Double.Parse(values[12]);
                    fc0k = Double.Parse(values[1]);
                    fc90k = Double.Parse(values[4]);
                    E05 = Double.Parse(values[9]);
                    if (values[13] == "SOLID")
                    {
                        tipodemadeira = 1;
                    }
                    stop = true;
                }
                cont++;
            }

            //Definição de valores do material 
            double fc0d = Kmod * fc0k / Ym;
            double fc90d = Kmod * fc90k / Ym;

            //compressão perpendicular ou em ângulo (garantir que acomp foi fornecido corretamente)
            if (acomp <= 90)
            {
                //Definição de valores geométricos e de tensão efetiva
                double d1min = Math.Min(d1 / 2, lFcad);
                double d1ef = Math.Min(d1min, 3);
                double d2min = Math.Min(d2 / 2, lFcad);
                double d2ef = Math.Min(d2min, 3);
                double lef = lFcad + d1ef + d2ef;
                double Aef = lef * b;
                double sigc90d = Fcad / Aef;
                double kc90 = 1;

                // determinando valor de kc90 (caso não seja majorado por um if abaixo, deve valer igual a 1.0)
                // ifs também perguntam se a madeira é MLC ou SOLID (coluna 13 do excel deve ser preenchida)

                if (d1tipo == 0 && d2tipo == 1 && d2 >= 2 * h)
                {
                    if (tipodemadeira == 0 && lFcad <= 40)
                    {
                        kc90 = 1.75;
                    }
                    if (tipodemadeira == 1)
                    {
                        kc90 = 1.5;
                    }
                }

                if (d2tipo == 0 && d1tipo == 1 && d1 >= 2 * h)
                {
                    if (tipodemadeira == 0 && lFcad <= 40)
                    {
                        kc90 = 1.75;
                    }
                    if (tipodemadeira == 1)
                    {
                        kc90 = 1.5;
                    }
                }

                if (d2tipo == 1 && d1tipo == 1)
                {
                    if (d1 >= 2 * h && d2 >= 2 * h)
                    {
                        if (tipodemadeira == 0 && lFcad <= 40)
                        {
                            kc90 = 1.75;
                        }
                        if (tipodemadeira == 1)
                        {
                            kc90 = 1.5;
                        }
                    }
                }

                //Verificação de compressão perpendicular ou em ângulo

                double acompR = Math.PI * acomp / 180;
                double DIV = sigc90d * ( fc0d * Math.Pow(Math.Sin (acompR),2) / (kc90 * fc90d) + Math.Pow(Math.Cos(acompR), 2) ) / fc0d ;
                DA.SetData(0, DIV);

            }

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
            get { return new Guid("fa016e35-fc46-453c-917e-090443e35b19"); }
        }
    }
}