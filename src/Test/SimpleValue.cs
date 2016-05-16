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
    public class SimpleValue
    {
        #region Private classes

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

            [XorProperty("NullableInt32")]
            public int? NullableInt32 { get; set; }

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

            [XorProperty("Char")]
            public char Char { get; set; }

            [XorProperty("NullableChar")]
            public char? NullableChar { get; set; }

            [XorProperty("String")]
            public string String { get; set; }

            [XorProperty("Guid")]
            public Guid Guid { get; set; }

            [XorProperty("NullableGuid")]
            public Guid? NullableGuid { get; set; }

            [XorProperty("DateTime")]
            public DateTime DateTime { get; set; }

            [XorProperty("NullableDateTime")]
            public DateTime? NullableDateTime { get; set; }

            [XorProperty("TimeSpan")]
            public TimeSpan TimeSpan { get; set; }

            [XorProperty("Decimal")]
            public decimal Decimal { get; set; }
        }

        #endregion

        private SimpleClass simpleClass;

        [SetUp]
        public void Init()
        {
            this.simpleClass = new SimpleClass()
            {
                Bool = true,
                Byte = byte.MaxValue,
                DateTime = new DateTime(2013, 11, 29, 17, 00, 30, 55),
                NullableDateTime = new DateTime(2013, 11, 29, 17, 00, 30, 55),
                Decimal = decimal.MaxValue,
                Double = double.MaxValue,
                Guid = Guid.NewGuid(),
                NullableGuid = Guid.NewGuid(),
                Int16 = short.MaxValue,
                Int32 = int.MaxValue,
                NullableInt32 = int.MaxValue,
                Int64 = long.MaxValue,
                SByte = sbyte.MaxValue,
                Single = float.MaxValue,
                Char = 'A',
                NullableChar = 'A',
                String = "This is a test\n:)\\<~>\r\n\t\t\r\n.",
                TimeSpan = new TimeSpan(3, 4, 5, 6, 7),
                UInt16 = ushort.MaxValue,
                UInt32 = uint.MaxValue,
                UInt64 = ulong.MaxValue,
            };
        }

        [Test]
        public void Save()
        {
            string file = Path.GetTempFileName();
            XorController.Get().Save(this.simpleClass, file);
        }

        [Test]
        public void Load()
        {
            string file = Path.GetTempFileName();
            XorController.Get().Save(this.simpleClass, file);
            XorController.Get().Load<SimpleClass>(file);
        }

        [Test]
        public void LoadSaveIteration()
        {
            TestHelper.TestIteration(this.simpleClass);
        }

        #region Integrity<SimpleValueType>

        [Test]
        public void IntegrityBool([Values(true, false)] bool vBool)
        {
            this.simpleClass.Bool = vBool;
            var obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.Bool, obj.Bool);
        }

        [Test]
        public void IntegrityByte([Values(byte.MinValue, byte.MaxValue)] byte vByte)
        {
            this.simpleClass.Byte = vByte;
            var obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.Byte, obj.Byte);
        }

        [Test]
        public void IntegrityDecimal()
        {
            this.simpleClass.Decimal = decimal.MaxValue;
            var obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.Decimal, obj.Decimal);

            this.simpleClass.Decimal = decimal.MinValue;
            obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.Decimal, obj.Decimal);

            this.simpleClass.Decimal = 4357843578234.32452346565m;
            obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.Decimal, obj.Decimal);
        }

        [Test]
        public void IntegrityGuid()
        {
            this.simpleClass.Guid = Guid.NewGuid();
            var obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.Guid, obj.Guid);

            this.simpleClass.Guid = Guid.Empty;
            obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.Guid, obj.Guid);
        }

        [Test]
        public void IntegrityNullableGuid()
        {
            this.simpleClass.NullableGuid = Guid.NewGuid();
            var obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.NullableGuid, obj.NullableGuid);

            this.simpleClass.NullableGuid = Guid.Empty;
            obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.NullableGuid, obj.NullableGuid);

            this.simpleClass.NullableGuid = null;
            obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.NullableGuid, obj.NullableGuid);
        }

        [Test]
        public void IntegrityDateTime()
        {
            this.simpleClass.DateTime = DateTime.MaxValue;
            var obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.DateTime, obj.DateTime);

            this.simpleClass.DateTime = DateTime.MinValue;
            obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.DateTime, obj.DateTime);

            this.simpleClass.DateTime = DateTime.Now;
            obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.DateTime, obj.DateTime);

            this.simpleClass.DateTime = DateTime.UtcNow;
            obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.DateTime, obj.DateTime);
        }

        [Test]
        public void IntegrityNullableDateTime()
        {
            this.simpleClass.NullableDateTime = DateTime.MaxValue;
            var obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.NullableDateTime, obj.NullableDateTime);

            this.simpleClass.NullableDateTime = DateTime.MinValue;
            obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.NullableDateTime, obj.NullableDateTime);

            this.simpleClass.NullableDateTime = DateTime.Now;
            obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.NullableDateTime, obj.NullableDateTime);

            this.simpleClass.NullableDateTime = DateTime.UtcNow;
            obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.NullableDateTime, obj.NullableDateTime);

            this.simpleClass.NullableDateTime = null;
            obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.NullableDateTime, obj.NullableDateTime);
        }

        [Test]
        public void IntegrityDouble([Values(double.MinValue, double.MaxValue, double.Epsilon, double.NaN, double.NegativeInfinity, double.PositiveInfinity)] Double vDouble)
        {
            this.simpleClass.Double = vDouble;
            var obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.Double, obj.Double);
        }

        [Test]
        public void IntegrityDoubleRandom([Random(double.MinValue, double.MaxValue, 10)] Double vDouble)
        {
            this.simpleClass.Double = vDouble;
            var obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.Double, obj.Double);
        }

        [Test]
        public void IntegrityInt16([Values(short.MinValue, short.MaxValue)] Int16 vInt16)
        {
            this.simpleClass.Int16 = vInt16;
            var obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.Int16, obj.Int16);
        }

        [Test]
        public void IntegrityInt32([Values(int.MinValue, int.MaxValue)] Int32 vInt32)
        {
            this.simpleClass.Int32 = vInt32;
            var obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.Int32, obj.Int32);
        }

        [Test]
        public void IntegrityNullableInt32([Values(null, int.MinValue, int.MaxValue)] Int32? vNullableInt32)
        {
            this.simpleClass.NullableInt32 = vNullableInt32;
            var obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.NullableInt32, obj.NullableInt32);
        }

        [Test]
        public void IntegrityInt64([Values(long.MinValue, long.MaxValue)] Int64 vInt64)
        {
            this.simpleClass.Int64 = vInt64;
            var obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.Int64, obj.Int64);
        }

        [Test]
        public void IntegritySByte([Values(sbyte.MinValue, sbyte.MaxValue)] SByte vSByte)
        {
            this.simpleClass.SByte = vSByte;
            var obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.SByte, obj.SByte);
        }

        [Test]
        public void IntegritySingle([Values(float.MinValue, float.MaxValue, float.Epsilon, float.NaN, float.NegativeInfinity, float.PositiveInfinity)] Single vSingle)
        {
            this.simpleClass.Single = vSingle;
            var obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.Single, obj.Single);
        }

        [Test]
        public void IntegritySingleRandom([Random(float.MinValue, float.MaxValue, 10)] Double vSingle)
        {
            this.simpleClass.Single = (float) vSingle;
            var obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.Single, obj.Single);
        }

        [Test]
        public void IntegrityChar([Values(char.MinValue, char.MaxValue, 'a', 'A', 'ö', 'Ö', 'ß', ' ')] char vChar)
        {
            this.simpleClass.Char = vChar;
            var obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.Char, obj.Char);
        }

        [Test]
        public void IntegrityNullableChar([Values(null, char.MinValue, char.MaxValue, 'a', 'A', 'ö', 'Ö', 'ß', ' ')] char? vNullableChar)
        {
            this.simpleClass.NullableChar = vNullableChar;
            var obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.NullableChar, obj.NullableChar);
        }

        [Test]
        public void IntegrityString([Values("Test", ":)", "\n", "\n\n", "\t", " leading and trailing whitespace ")] String vString)
        {
            this.simpleClass.String = vString;
            var obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.String, obj.String);
        }

        [Ignore]
        [Test]
        public void IntegrityStringCrLfLineBreaks([Values("\r\n\r\n", "\r\n", "This is a test\n:)\\<~>\r\n\t\t\r\n.")] String vString)
        {
            this.simpleClass.String = vString;
            var obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.String, obj.String);
        }

        [Test]
        public void IntegrityTimeSpan()
        {
            this.simpleClass.TimeSpan = TimeSpan.MaxValue;
            var obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.TimeSpan, obj.TimeSpan);

            this.simpleClass.TimeSpan = TimeSpan.MinValue;
            obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.TimeSpan, obj.TimeSpan);

            this.simpleClass.TimeSpan = TimeSpan.Zero;
            obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.TimeSpan, obj.TimeSpan);
        }

        [Test]
        public void IntegrityUInt16([Values(ushort.MinValue, ushort.MaxValue)] UInt16 vUInt16)
        {
            this.simpleClass.UInt16 = vUInt16;
            var obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.UInt16, obj.UInt16);
        }

        [Test]
        public void IntegrityUInt32([Values(uint.MinValue, uint.MaxValue)] UInt32 vUInt32)
        {
            this.simpleClass.UInt32 = vUInt32;
            var obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.UInt32, obj.UInt32);
        }

        [Test]
        public void IntegrityUInt64([Values(ulong.MinValue, ulong.MaxValue)] UInt64 vUInt64)
        {
            this.simpleClass.UInt64 = vUInt64;
            var obj = TestHelper.SaveAndLoad(this.simpleClass);
            Assert.AreEqual(this.simpleClass.UInt64, obj.UInt64);
        }

        #endregion

        [Test]
        public void IntegrityUninitialized()
        {
            this.simpleClass = new SimpleClass();
            var obj = TestHelper.SaveAndLoad(this.simpleClass);

            Assert.AreEqual(this.simpleClass.Bool, obj.Bool);
            Assert.AreEqual(this.simpleClass.Byte, obj.Byte);
            Assert.AreEqual(this.simpleClass.DateTime, obj.DateTime);
            Assert.AreEqual(this.simpleClass.NullableDateTime, obj.NullableDateTime);
            Assert.AreEqual(this.simpleClass.Decimal, obj.Decimal);
            Assert.AreEqual(this.simpleClass.Double, obj.Double);
            Assert.AreEqual(this.simpleClass.Guid, obj.Guid);
            Assert.AreEqual(this.simpleClass.NullableGuid, obj.NullableGuid);
            Assert.AreEqual(this.simpleClass.Int16, obj.Int16);
            Assert.AreEqual(this.simpleClass.Int32, obj.Int32);
            Assert.AreEqual(this.simpleClass.NullableInt32, obj.NullableInt32);
            Assert.AreEqual(this.simpleClass.Int64, obj.Int64);
            Assert.AreEqual(this.simpleClass.SByte, obj.SByte);
            Assert.AreEqual(this.simpleClass.Single, obj.Single);
            Assert.AreEqual(this.simpleClass.String, obj.String);
            Assert.AreEqual(this.simpleClass.Char, obj.Char);
            Assert.AreEqual(this.simpleClass.NullableChar, obj.NullableChar);
            Assert.AreEqual(this.simpleClass.TimeSpan, obj.TimeSpan);
            Assert.AreEqual(this.simpleClass.UInt16, obj.UInt16);
            Assert.AreEqual(this.simpleClass.UInt32, obj.UInt32);
            Assert.AreEqual(this.simpleClass.UInt64, obj.UInt64);
        }
    }
}
