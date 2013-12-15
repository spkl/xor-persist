using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LateNightStupidities.XorPersist;
using LateNightStupidities.XorPersist.Attributes;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class Inheritance
    {
        #region Private classes

        private abstract class A : XorObject
        {
            [XorProperty("PropA")]
            private string PropA { get; set; }

            [XorProperty("FieldA")]
            private string FieldA;

            protected A(bool dummy)
            {
                FieldA = Guid.NewGuid().ToString();
                PropA = Guid.NewGuid().ToString();
            }

            protected A()
            {
                
            }

            public string GetPropA()
            {
                return PropA;
            }

            public string GetFieldA()
            {
                return FieldA;
            }
        }

        [XorClass("B")]
        private class B : A
        {
            [XorProperty("PropB")]
            private string PropB { get; set; }

            [XorProperty("FieldB")]
            private string FieldB;

            [XorProperty("DefinedAtB_NewAtC")]
            protected string DefinedAtB_NewAtC { get; set; }

            public virtual string DefinedAtB_OverrideAtC { get; set; }

            public B(bool dummy) : base(dummy)
            {
                FieldB = Guid.NewGuid().ToString();
                PropB = Guid.NewGuid().ToString();

                DefinedAtB_NewAtC = Guid.NewGuid().ToString();
                DefinedAtB_OverrideAtC = Guid.NewGuid().ToString();
            }

            public B()
            {
                
            }

            public string GetPropB()
            {
                return PropB;
            }

            public string GetFieldB()
            {
                return FieldB;
            }

            public string GetDefinedAtB_NewAtC_FromB()
            {
                return DefinedAtB_NewAtC;
            }
        }

        [XorClass("C")]
        private class C : B
        {
            [XorProperty("PropC")]
            private string PropC { get; set; }

            [XorProperty("FieldC")]
            private string FieldC;

            [XorProperty("NewPropB")]
            private string PropB { get; set; }

            [XorProperty("NewFieldB")]
            private string FieldB;

            [XorProperty("DefinedAtB_NewAtC2")]
            protected new string DefinedAtB_NewAtC { get; set; }

            [XorProperty("DefinedAtB_OverrideAtC")]
            public override string DefinedAtB_OverrideAtC { get; set; }

            public C(bool dummy) : base(dummy)
            {
                FieldC = Guid.NewGuid().ToString();
                PropC = Guid.NewGuid().ToString();

                FieldB = Guid.NewGuid().ToString();
                PropB = Guid.NewGuid().ToString();

                DefinedAtB_NewAtC = Guid.NewGuid().ToString();
                DefinedAtB_OverrideAtC = Guid.NewGuid().ToString();
            }

            public C()
            {
                
            }

            public string GetPropC()
            {
                return PropC;
            }

            public string GetFieldC()
            {
                return FieldC;
            }

            public string GetNewPropB()
            {
                return PropB;
            }

            public string GetNewFieldB()
            {
                return FieldB;
            }

            public string GetDefinedAtB_NewAtC_FromC()
            {
                return DefinedAtB_NewAtC;
            }
        }

        #endregion

        private B b;
        private C c;
        private B bCopy;
        private C cCopy;

        [TestFixtureSetUp]
        public void Init()
        {
            b = new B(true);
            c = new C(true);
            bCopy = TestHelper.SaveAndLoad(b);
            cCopy = TestHelper.SaveAndLoad(c);
        }

        [Test]
        public void Property()
        {
            Assert.AreEqual(b.GetPropB(), bCopy.GetPropB());
            Assert.AreEqual(b.GetPropA(), bCopy.GetPropA());

            Assert.AreEqual(c.GetPropC(), cCopy.GetPropC());
            Assert.AreEqual(c.GetPropB(), cCopy.GetPropB());
            Assert.AreEqual(c.GetPropA(), cCopy.GetPropA());
        }

        [Test]
        public void PropertyWithSameName()
        {
            Assert.AreEqual(c.GetNewPropB(), cCopy.GetNewPropB());
        }

        [Test]
        public void PropertyNew()
        {
            Assert.AreEqual(b.GetDefinedAtB_NewAtC_FromB(), bCopy.GetDefinedAtB_NewAtC_FromB());

            Assert.AreEqual(c.GetDefinedAtB_NewAtC_FromB(), cCopy.GetDefinedAtB_NewAtC_FromB());
            Assert.AreEqual(c.GetDefinedAtB_NewAtC_FromC(), cCopy.GetDefinedAtB_NewAtC_FromC());
            Assert.AreNotEqual(c.GetDefinedAtB_NewAtC_FromB(), cCopy.GetDefinedAtB_NewAtC_FromC());
        }

        [Test]
        public void PropertyOverride()
        {
            Assert.AreEqual(null, bCopy.DefinedAtB_OverrideAtC);
            Assert.AreEqual(c.DefinedAtB_OverrideAtC, cCopy.DefinedAtB_OverrideAtC);
        }

        [Test]
        public void Field()
        {
            Assert.AreEqual(b.GetFieldB(), bCopy.GetFieldB());
            Assert.AreEqual(b.GetFieldA(), bCopy.GetFieldA());

            Assert.AreEqual(c.GetFieldC(), cCopy.GetFieldC());
            Assert.AreEqual(c.GetFieldB(), cCopy.GetFieldB());
            Assert.AreEqual(c.GetFieldA(), cCopy.GetFieldA());
        }

        [Test]
        public void FieldWithSameName()
        {
            Assert.AreEqual(c.GetNewFieldB(), cCopy.GetNewFieldB());
        }
    }
}
