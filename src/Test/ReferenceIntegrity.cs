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
    public class ReferenceIntegrity
    {
        // ReSharper disable InconsistentNaming

        #region Private classes

        [XorClass("OwnerList")]
        private class OwnerList : XorObject
        {
            private Guid id = Guid.NewGuid();

            [XorProperty("ID")]
            public Guid Id
            {
                get { return id; }
                set { id = value; }
            }

            [XorProperty("Owners", XorMultiplicity.List)]
            public IEnumerable<Owner> Owners { get; set; }
        }

        [XorClass("Owner")]
        private class Owner : XorObject
        {
            private Guid id = Guid.NewGuid();

            [XorProperty("ID")]
            public Guid Id
            {
                get { return id; }
                set { id = value; }
            }

            [XorProperty("Children", XorMultiplicity.List)]
            public IEnumerable<Child> Children { get; set; }

            public override bool Equals(object obj)
            {
                var owner = obj as Owner;
                return owner != null && Id.Equals(owner.Id);
            }

            public override int GetHashCode()
            {
                return id.GetHashCode();
            }
        }

        [XorClass("Child")]
        private class Child : XorObject
        {
            private Guid id = Guid.NewGuid();

            [XorProperty("ID")]
            public Guid Id
            {
                get { return id; }
                set { id = value; }
            }

            [XorReference("Owner")]
            public Owner Owner { get; set; }

            [XorReference("References", XorMultiplicity.List)]
            public IEnumerable<XorObject> References { get; set; }

            public override bool Equals(object obj)
            {
                var child = obj as Child;
                return child != null && Id.Equals(child.Id);
            }

            public override int GetHashCode()
            {
                return id.GetHashCode();
            }
        }

        #endregion

        private OwnerList oList;
        private Owner o0, o1;
        private Child c0_0, c0_1, c0_2, c1_0, c1_1;

        private OwnerList oList_copy;
        private Owner o0_copy, o1_copy;
        private Child c0_0_copy, c0_1_copy, c0_2_copy, c1_0_copy, c1_1_copy;

        [SetUp]
        public void Init()
        {
            c0_0 = new Child();
            c0_1 = new Child();
            c0_2 = new Child();
            c1_0 = new Child();
            c1_1 = new Child();

            o0 = new Owner();
            o1 = new Owner();
            
            oList = new OwnerList();
            oList.Owners = new[] { o0, o1 };

            o0.Children = new[] { c0_0, c0_1, c0_2 };
            o1.Children = new[] { c1_0, c1_1 };

            c0_0.Owner = o0;
            c0_1.Owner = null;
            c0_2.Owner = o0;
            c1_0.Owner = o1;
            c1_1.Owner = o1;

            c0_0.References = new[] { c0_0, c0_1, c0_2, c1_0, c1_1 };
            c0_1.References = new[] { c1_1, null, null, c1_1, c1_0 };
            c0_2.References = new Child[] { null, null, null, null };
            c1_0.References = null;
            c1_1.References = new Child[] { };


            oList_copy = TestHelper.SaveAndLoad(oList);
            o0_copy = oList_copy.Owners.ElementAt(0);
            o1_copy = oList_copy.Owners.ElementAt(1);

            c0_0_copy = o0_copy.Children.ElementAt(0);
            c0_1_copy = o0_copy.Children.ElementAt(1);
            c0_2_copy = o0_copy.Children.ElementAt(2);
            c1_0_copy = o1_copy.Children.ElementAt(0);
            c1_1_copy = o1_copy.Children.ElementAt(1);
        }

        [Test]
        public void SingleReferenceResolving()
        {
            Assert.AreEqual(c0_0.Owner, c0_0_copy.Owner);
            Assert.AreEqual(c0_1.Owner, c0_1_copy.Owner);
            Assert.AreEqual(c0_2.Owner, c0_2_copy.Owner);

            Assert.AreEqual(c1_0.Owner, c1_0_copy.Owner);
            Assert.AreEqual(c1_1.Owner, c1_1_copy.Owner);
        }

        [Test]
        public void ListReferenceResolving()
        {
            CollectionAssert.AreEqual(c0_0.References, c0_0_copy.References);
            CollectionAssert.AreEqual(c0_1.References, c0_1_copy.References);
            CollectionAssert.AreEqual(c0_2.References, c0_2_copy.References);

            CollectionAssert.AreEqual(c1_0.References, c1_0_copy.References);
            CollectionAssert.AreEqual(c1_1.References, c1_1_copy.References);
        }

        // ReSharper restore InconsistentNaming
    }
}
