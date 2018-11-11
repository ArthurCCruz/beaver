using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Madeira
{
    public class MadeiraInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Madeira";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("abcd2eea-0c46-4db1-9ce7-8434015ef6ca");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
