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

            public B(bool dummy) : base(dummy)
            {
                FieldB = Guid.NewGuid().ToString();
                PropB = Guid.NewGuid().ToString();
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
        }

        [XorClass("C")]
        private class C : B
        {
            [XorProperty("PropC")]
            private string PropC { get; set; }

            [XorProperty("FieldC")]
            private string FieldC;

            public C(bool dummy) : base(dummy)
            {
                FieldC = Guid.NewGuid().ToString();
                PropC = Guid.NewGuid().ToString();
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
        }

        #endregion

        private B b;
        private C c;
        private B bCopy;
        private C cCopy;

        [SetUp]
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
        public void Field()
        {
            Assert.AreEqual(b.GetFieldB(), bCopy.GetFieldB());
            Assert.AreEqual(b.GetFieldA(), bCopy.GetFieldA());

            Assert.AreEqual(c.GetFieldC(), cCopy.GetFieldC());
            Assert.AreEqual(c.GetFieldB(), cCopy.GetFieldB());
            Assert.AreEqual(c.GetFieldA(), cCopy.GetFieldA());
        }
    }
}
