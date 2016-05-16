using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LateNightStupidities.XorPersist.Attributes;

namespace LateNightStupidities.XorPersist.Example
{
    [XorClass("RootClass")]
    class RootClass : XorObject
    {
        [XorProperty]
        public double Rating { get; set; }

        [XorProperty("secret")]
#pragma warning disable 414
        private string secret;
#pragma warning restore 414

        [XorProperty(XorMultiplicity.List)]
        public List<ILeafClass> Leaves { get; set; }

        [XorProperty(XorMultiplicity.List)]
        public List<string> Strings { get; set; }

        [XorProperty(XorMultiplicity.List)]
        private ISet<uint> Uints { get; set; }

        [XorProperty]
        public ILeafClass MainLeaf { get; set; }

        [XorProperty]
        public DateTime Timestamp { get; set; }

        [XorProperty]
        public TimeSpan Duration { get; set; }

        [XorProperty]
        public decimal Value { get; set; }

        [XorReference(XorMultiplicity.List)]
        public IEnumerable<ILeafClass> References { get; set; }
        
        [XorReference]
        public ILeafClass Reference { get; set; }

        //[XorProperty("NullableIntIsNull")]
        //public int? NullableIntIsNull { get; set; }

        //[XorProperty("NullableIntIsNotNull")]
        //public int? NullableIntIsNotNull { get; set; }

        public RootClass()
        {
            
        }

        public RootClass(bool foo)
        {
            secret = "<test>§(/%§=98540239485ß032945-#-+ü34-t+4";
            Rating = 3.555555555555555555555555;
            Timestamp = DateTime.Now;
            Thread.Sleep(30);
            Duration = DateTime.Now.AddMinutes(1) - Timestamp;
            Value = 0.54444444444444444441m;
            Strings = new List<string>() { "1", null, "2" };
            Leaves = new List<ILeafClass>() { new LeafClass(this, "numma 1"), new LeafClass(this, "numma 2") };
            References = Leaves.ToList().Concat(new[] { null, Leaves.ElementAt(0) });
            Uints = new HashSet<uint>() { 1, 2, 3, 1, 5 };
            Reference = Leaves.ElementAt(1);
            //NullableIntIsNull = null;
            //NullableIntIsNotNull = 4;
        }
    }

    [XorClass(typeof(LeafClass))]
    internal class LeafClass : XorObject, ILeafClass
    {
        [XorProperty]
        public ILeafClass Leaf { get; set; }

        [XorProperty]
        public string Name { get; set; }

        [XorReference("LeafOwner")]
        private XorObject leafOwner;

        public LeafClass()
        {

        }

        public LeafClass(XorObject owner, string name)
        {
            Name = name;
            leafOwner = owner;
        }
    }

    internal interface ILeafClass
    {
        ILeafClass Leaf { get; set; }
        string Name { get; set; }
    }
}
