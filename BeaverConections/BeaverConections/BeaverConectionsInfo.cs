﻿using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace BeaverConections
{
    public class BeaverConectionsInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Beaver";
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
                return new Guid("90e18c2b-b29b-4aa9-af96-90f5dbb303b7");
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
                return "test";
            }
        }
    }
}
