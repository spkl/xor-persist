using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using LateNightStupidities.XorPersist;
using LateNightStupidities.XorPersist.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class SimpleTest
    {
        private SimpleClass simpleClass;

        [XorClass("SimpleClass")]
        private class SimpleClass : XorObject
        {
            [XorProperty("Bool")]
            public bool Bool { get; set; }

            [XorProperty("Byte")]
            public byte Byte { get; set; }

            [XorProperty("SByte")]
            public sbyte SByte { get; set; }

            [XorProperty("Int16")]
            public short Int16 { get; set; }

            [XorProperty("Int32")]
            public int Int32 { get; set; }

            [XorProperty("Int64")]
            public long Int64 { get; set; }

            [XorProperty("UInt16")]
            public ushort UInt16 { get; set; }

            [XorProperty("UInt32")]
            public uint UInt32 { get; set; }

            [XorProperty("UInt64")]
            public ulong UInt64 { get; set; }

            [XorProperty("Single")]
            public float Single { get; set; }

            [XorProperty("SingleEpsilon")]
            public float SEpsilon { get; set; }

            [XorProperty("Double")]
            public double Double { get; set; }

            [XorProperty("DoubleEpsilon")]
            public double DEpsilon { get; set; }

            [XorProperty("DoubleInfinity")]
            public double DInfinity { get; set; }

            [XorProperty("DoubleNaN")]
            public double DNan { get; set; }

            [XorProperty("String")]
            public string String { get; set; }

            [XorProperty("Guid")]
            public Guid Guid { get; set; }

            [XorProperty("DateTime")]
            public DateTime DateTime { get; set; }

            [XorProperty("TimeSpan")]
            public TimeSpan TimeSpan { get; set; }

            [XorProperty("Decimal")]
            public decimal Decimal { get; set; }
        }
        
        [TestInitialize]
        public void Init()
        {
            simpleClass = new SimpleClass()
            {
                Bool = true,
                Byte = byte.MaxValue,
                DateTime = new DateTime(2013, 11, 29, 17, 00, 30, 55),
                Decimal = decimal.MaxValue,
                Double = double.MaxValue,
                DEpsilon = double.Epsilon,
                DInfinity = double.PositiveInfinity,
                DNan = double.NaN,
                Guid = Guid.NewGuid(),
                Int16 = short.MaxValue,
                Int32 = int.MaxValue,
                Int64 = long.MaxValue,
                SByte = sbyte.MaxValue,
                Single = float.MaxValue,
                SEpsilon = float.Epsilon,
                String = "This is a test\n:)\\<~>\r\n\t\t\r\n.",
                TimeSpan = new TimeSpan(3, 4, 5, 6, 7),
                UInt16 = ushort.MaxValue,
                UInt32 = uint.MaxValue,
                UInt64 = ulong.MaxValue,
            };
        }

        [TestMethod]
        public void TestSave()
        {
            string file = Path.GetTempFileName();
            XorController.Get().Save(simpleClass, file);
        }

        [TestMethod]
        public void TestLoad()
        {
            string file = Path.GetTempFileName();
            XorController.Get().Save(simpleClass, file);
            XorController.Get().Load<SimpleClass>(file);
        }

        [TestMethod]
        public void TestIntegrity()
        {
            string file = Path.GetTempFileName();
            XorController.Get().Save(simpleClass, file);
            var obj = XorController.Get().Load<SimpleClass>(file);

            Assert.AreEqual(simpleClass.Bool, obj.Bool);
            Assert.AreEqual(simpleClass.Byte, obj.Byte);
            Assert.AreEqual(simpleClass.DateTime, obj.DateTime);
            Assert.AreEqual(simpleClass.Decimal, obj.Decimal);
            Assert.AreEqual(simpleClass.Double, obj.Double);
            Assert.AreEqual(simpleClass.DEpsilon, obj.DEpsilon);
            Assert.AreEqual(simpleClass.Guid, obj.Guid);
            Assert.AreEqual(simpleClass.Int16, obj.Int16);
            Assert.AreEqual(simpleClass.Int32, obj.Int32);
            Assert.AreEqual(simpleClass.Int64, obj.Int64);
            Assert.AreEqual(simpleClass.SByte, obj.SByte);
            Assert.AreEqual(simpleClass.Single, obj.Single);
            Assert.AreEqual(simpleClass.SEpsilon, obj.SEpsilon);
            Assert.AreEqual(simpleClass.String, obj.String);
            Assert.AreEqual(simpleClass.TimeSpan, obj.TimeSpan);
            Assert.AreEqual(simpleClass.UInt16, obj.UInt16);
            Assert.AreEqual(simpleClass.UInt32, obj.UInt32);
            Assert.AreEqual(simpleClass.UInt64, obj.UInt64);
        }
    }
}
