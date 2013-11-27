using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LateNightStupidities.XorPersist.Schema
{
    internal static class XorXsd
    {
        // Elements
        public const string Root = "XorPersist";

        public const string StaticInfo = "StaticInfo";
        public const string Content = "Content";

        public const string Object = "obj";
        public const string Property = "prop";
        public const string Reference = "ref";
        public const string ListItemNonNull = "item";
        public const string ListItemNull = "null";

        // Attributes
        public const string ClassName = "cn";
        public const string MemberName = "mn";
        public const string Multiplicity = "mul";
    }
}
