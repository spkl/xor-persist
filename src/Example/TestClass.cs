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
        [XorProperty("Rating")]
        public double Rating { get; set; }

        [XorProperty("secret")]
        private string secret;

        public List<ILeafClass> Leaves { get; set; }
        
        [XorProperty("Leaves", XorMultiplicity.List)]
        private IEnumerable<ILeafClass> _Leaves
        {
            get { return Leaves; }
            set { Leaves = new List<ILeafClass>(value); }
        }

        public List<string> Strings { get; set; }

        [XorProperty("Strings", XorMultiplicity.List)]
        private IEnumerable<string> _Strings
        {
            get { return Strings; }
            set { Strings = new List<string>(value); }
        }

        private ISet<uint> Uints { get; set; }

        [XorProperty("Uints", XorMultiplicity.List)]
        private IEnumerable<uint> _Uints
        {
            get { return Uints; }
            set { Uints = new HashSet<uint>(value); }
        }

        [XorProperty("MainLeaf")]
        public ILeafClass MainLeaf { get; set; }

        [XorProperty("Timestamp")]
        public DateTime Timestamp { get; set; }

        [XorProperty("Duration")]
        public TimeSpan Duration { get; set; }

        [XorProperty("Value")]
        public decimal Value { get; set; }

        public IEnumerable<ILeafClass> References { get; set; }

        [XorReference("References", XorMultiplicity.List)]
        public IEnumerable<XorObject> _References
        {
            get { return References.Cast<XorObject>(); }
            set { References = new List<ILeafClass>(value.Cast<ILeafClass>()); }
        }

        [XorReference("Reference")]
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

    [XorClass("LeafClass")]
    internal class LeafClass : XorObject, ILeafClass
    {
        [XorProperty("Leaf")]
        public ILeafClass Leaf { get; set; }

        [XorProperty("Name")]
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
