using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using LateNightStupidities.XorPersist;
using LateNightStupidities.XorPersist.Attributes;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
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
            
            [XorProperty("Double")]
            public double Double { get; set; }
            
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
        
        [SetUp]
        public void Init()
        {
            simpleClass = new SimpleClass()
            {
                Bool = true,
                Byte = byte.MaxValue,
                DateTime = new DateTime(2013, 11, 29, 17, 00, 30, 55),
                Decimal = decimal.MaxValue,
                Double = double.MaxValue,
                Guid = Guid.NewGuid(),
                Int16 = short.MaxValue,
                Int32 = int.MaxValue,
                Int64 = long.MaxValue,
                SByte = sbyte.MaxValue,
                Single = float.MaxValue,
                String = "This is a test\n:)\\<~>\r\n\t\t\r\n.",
                TimeSpan = new TimeSpan(3, 4, 5, 6, 7),
                UInt16 = ushort.MaxValue,
                UInt32 = uint.MaxValue,
                UInt64 = ulong.MaxValue,
            };
        }

        [Test]
        public void TestSave()
        {
            string file = Path.GetTempFileName();
            XorController.Get().Save(simpleClass, file);
        }

        [Test]
        public void TestLoad()
        {
            string file = Path.GetTempFileName();
            XorController.Get().Save(simpleClass, file);
            XorController.Get().Load<SimpleClass>(file);
        }

        #region TestIntegrity<SimpleValueType>

        [Test]
        public void TestIntegrityBool([Values(true, false)] bool vBool)
        {
            simpleClass.Bool = vBool;
            var obj = SaveAndLoad();
            Assert.AreEqual(simpleClass.Bool, obj.Bool);
        }

        [Test]
        public void TestIntegrityByte([Values(byte.MinValue, byte.MaxValue)] byte vByte)
        {
            simpleClass.Byte = vByte;
            var obj = SaveAndLoad();
            Assert.AreEqual(simpleClass.Byte, obj.Byte);
        }

        [Test]
        public void TestIntegrityDecimal()
        {
            simpleClass.Decimal = decimal.MaxValue;
            var obj = SaveAndLoad();
            Assert.AreEqual(simpleClass.Decimal, obj.Decimal);

            simpleClass.Decimal = decimal.MinValue;
            obj = SaveAndLoad();
            Assert.AreEqual(simpleClass.Decimal, obj.Decimal);

            simpleClass.Decimal = 4357843578234.32452346565m;
            obj = SaveAndLoad();
            Assert.AreEqual(simpleClass.Decimal, obj.Decimal);
        }

        [Test]
        public void TestIntegrityDateTime()
        {
            simpleClass.DateTime = DateTime.MaxValue;
            var obj = SaveAndLoad();
            Assert.AreEqual(simpleClass.DateTime, obj.DateTime);

            simpleClass.DateTime = DateTime.MinValue;
            obj = SaveAndLoad();
            Assert.AreEqual(simpleClass.DateTime, obj.DateTime);

            simpleClass.DateTime = DateTime.Now;
            obj = SaveAndLoad();
            Assert.AreEqual(simpleClass.DateTime, obj.DateTime);

            simpleClass.DateTime = DateTime.UtcNow;
            obj = SaveAndLoad();
            Assert.AreEqual(simpleClass.DateTime, obj.DateTime);
        }

        [Test]
        public void TestIntegrityDouble([Values(double.MinValue, double.MaxValue, double.Epsilon, double.NaN, double.NegativeInfinity, double.PositiveInfinity)] Double vDouble)
        {
            simpleClass.Double = vDouble;
            var obj = SaveAndLoad();
            Assert.AreEqual(simpleClass.Double, obj.Double);
        }

        [Test]
        public void TestIntegrityInt16([Values(short.MinValue, short.MaxValue)] Int16 vInt16)
        {
            simpleClass.Int16 = vInt16;
            var obj = SaveAndLoad();
            Assert.AreEqual(simpleClass.Int16, obj.Int16);
        }

        [Test]
        public void TestIntegrityInt32([Values(int.MinValue, int.MaxValue)] Int32 vInt32)
        {
            simpleClass.Int32 = vInt32;
            var obj = SaveAndLoad();
            Assert.AreEqual(simpleClass.Int32, obj.Int32);
        }

        [Test]
        public void TestIntegrityInt64([Values(long.MinValue, long.MaxValue)] Int64 vInt64)
        {
            simpleClass.Int64 = vInt64;
            var obj = SaveAndLoad();
            Assert.AreEqual(simpleClass.Int64, obj.Int64);
        }

        [Test]
        public void TestIntegritySByte([Values(sbyte.MinValue, sbyte.MaxValue)] SByte vSByte)
        {
            simpleClass.SByte = vSByte;
            var obj = SaveAndLoad();
            Assert.AreEqual(simpleClass.SByte, obj.SByte);
        }

        [Test]
        public void TestIntegritySingle([Values(float.MinValue, float.MaxValue, float.Epsilon, float.NaN, float.NegativeInfinity, float.PositiveInfinity)] Single vSingle)
        {
            simpleClass.Single = vSingle;
            var obj = SaveAndLoad();
            Assert.AreEqual(simpleClass.Single, obj.Single);
        }

        [Test]
        public void TestIntegrityString([Values("Test", ":)", "\n", "\r\n", "\t", "This is a test\n:)\\<~>\r\n\t\t\r\n.")] String vString)
        {
            simpleClass.String = vString;
            var obj = SaveAndLoad();
            Assert.AreEqual(simpleClass.String, obj.String);
        }

        [Test]
        public void TestIntegrityTimeSpan()
        {
            simpleClass.TimeSpan = TimeSpan.MaxValue;
            var obj = SaveAndLoad();
            Assert.AreEqual(simpleClass.TimeSpan, obj.TimeSpan);

            simpleClass.TimeSpan = TimeSpan.MinValue;
            obj = SaveAndLoad();
            Assert.AreEqual(simpleClass.TimeSpan, obj.TimeSpan);

            simpleClass.TimeSpan = TimeSpan.Zero;
            obj = SaveAndLoad();
            Assert.AreEqual(simpleClass.TimeSpan, obj.TimeSpan);
        }

        [Test]
        public void TestIntegrityUInt16([Values(ushort.MinValue, ushort.MaxValue)] UInt16 vUInt16)
        {
            simpleClass.UInt16 = vUInt16;
            var obj = SaveAndLoad();
            Assert.AreEqual(simpleClass.UInt16, obj.UInt16);
        }

        [Test]
        public void TestIntegrityUInt32([Values(uint.MinValue, uint.MaxValue)] UInt32 vUInt32)
        {
            simpleClass.UInt32 = vUInt32;
            var obj = SaveAndLoad();
            Assert.AreEqual(simpleClass.UInt32, obj.UInt32);
        }

        [Test]
        public void TestIntegrityUInt64([Values(ulong.MinValue, ulong.MaxValue)] UInt64 vUInt64)
        {
            simpleClass.UInt64 = vUInt64;
            var obj = SaveAndLoad();
            Assert.AreEqual(simpleClass.UInt64, obj.UInt64);
        }

        #endregion

        [Test]
        public void TestIntegrityUninitialized()
        {
            simpleClass = new SimpleClass();
            var obj = SaveAndLoad();

            Assert.AreEqual(simpleClass.Bool, obj.Bool);
            Assert.AreEqual(simpleClass.Byte, obj.Byte);
            Assert.AreEqual(simpleClass.DateTime, obj.DateTime);
            Assert.AreEqual(simpleClass.Decimal, obj.Decimal);
            Assert.AreEqual(simpleClass.Double, obj.Double);
            Assert.AreEqual(simpleClass.Guid, obj.Guid);
            Assert.AreEqual(simpleClass.Int16, obj.Int16);
            Assert.AreEqual(simpleClass.Int32, obj.Int32);
            Assert.AreEqual(simpleClass.Int64, obj.Int64);
            Assert.AreEqual(simpleClass.SByte, obj.SByte);
            Assert.AreEqual(simpleClass.Single, obj.Single);
            Assert.AreEqual(simpleClass.String, obj.String);
            Assert.AreEqual(simpleClass.TimeSpan, obj.TimeSpan);
            Assert.AreEqual(simpleClass.UInt16, obj.UInt16);
            Assert.AreEqual(simpleClass.UInt32, obj.UInt32);
            Assert.AreEqual(simpleClass.UInt64, obj.UInt64);
        }

        private SimpleClass SaveAndLoad()
        {
            string file = Path.GetTempFileName();
            XorController.Get().Save(simpleClass, file);
            var obj = XorController.Get().Load<SimpleClass>(file);
            return obj;
        }
    }
}
