using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LateNightStupidities.XorPersist;
using LateNightStupidities.XorPersist.Attributes;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class ObjectContainment
    {
        #region Private classes

        [XorClass("Root")]
        private class Root : XorObject
        {
            [XorProperty("PrivateField")]
            private XorObject PrivateField;

            [XorProperty("PublicField")]
            public XorObject PublicField;

            [XorProperty("PrivateProperty")]
            private XorObject PrivateProperty { get; set; }

            [XorProperty("PublicProperty")]
            public XorObject PublicProperty { get; set; }

            [XorProperty("PrivateEnumerableField", typeof(XorObject))]
            private IEnumerable<XorObject> PrivateEnumerableField;

            [XorProperty("PublicEnumerableField", typeof(XorObject))]
            public IEnumerable<XorObject> PublicEnumerableField;

            [XorProperty("PrivateEnumerableProperty", typeof(XorObject))]
            private IEnumerable<XorObject> PrivateEnumerableProperty { get; set; }

            [XorProperty("PublicEnumerableProperty", typeof(XorObject))]
            public IEnumerable<XorObject> PublicEnumerableProperty { get; set; }

            public Root()
            {
                
            }

            public Root(XorObject privateField, XorObject publicField, XorObject privateProperty,
                XorObject publicProperty, IEnumerable<XorObject> privateEnumerableField,
                IEnumerable<XorObject> publicEnumerableField, IEnumerable<XorObject> privateEnumerableProperty,
                IEnumerable<XorObject> publicEnumerableProperty)
            {
                this.PrivateField = privateField;
                this.PublicField = publicField;
                this.PrivateProperty = privateProperty;
                this.PublicProperty = publicProperty;
                this.PrivateEnumerableField = privateEnumerableField;
                this.PublicEnumerableField = publicEnumerableField;
                this.PrivateEnumerableProperty = privateEnumerableProperty;
                this.PublicEnumerableProperty = publicEnumerableProperty;
            }

            public ILeaf GetPrivateField()
            {
                return (ILeaf) this.PrivateField;
            }

            public ILeaf GetPrivateProperty()
            {
                return (ILeaf) this.PrivateProperty;
            }

            public IEnumerable<ILeaf> GetPrivateEnumerableField()
            {
                return this.PrivateEnumerableField.Cast<ILeaf>();
            }

            public IEnumerable<ILeaf> GetPrivateEnumerableProperty()
            {
                return this.PrivateEnumerableProperty.Cast<ILeaf>();
            }
        }

        private interface ILeaf
        {
            string Name { get; set; }
        }

        [XorClass("Leaf")]
        private class Leaf : XorObject, ILeaf
        {
            [XorProperty("Name")]
            public string Name { get; set; }

            public Leaf()
            {
                
            }

            public Leaf(string name)
            {
                this.Name = name;
            }
        }

        [XorClass("Leaf2")]
        private class Leaf2 : XorObject, ILeaf
        {
            [XorProperty("Name")]
            public string Name { get; set; }

            public Leaf2()
            {
                
            }

            public Leaf2(string name)
            {
                this.Name = name;
            }
        }

        #endregion

        private Root root, rootCopy;

        [SetUp]
        public void Init()
        {
            var privateField = new Leaf("PrivateField");
            var publicField = new Leaf("PublicField");
            var privateProperty = new Leaf2("PrivateProperty");
            var publicProperty = new Leaf2("PublicProperty");
            var privateEnumerableField = new XorObject[] { new Leaf("1"), new Leaf2("2"), };
            var publicEnumerableField = new XorObject[] { new Leaf2("3"), new Leaf2("4"), };
            var privateEnumerableProperty = new XorObject[] { new Leaf2("5"), new Leaf("6"), };
            var publicEnumerableProperty = new XorObject[] { new Leaf("7"), new Leaf("8"), };

            root = new Root(privateField, publicField, privateProperty, publicProperty, privateEnumerableField,
                publicEnumerableField, privateEnumerableProperty, publicEnumerableProperty);

            rootCopy = SaveAndLoadCopy(root);
        }

        private Root SaveAndLoadCopy(Root obj)
        {
            string file = Path.GetTempFileName();
            XorController.Get().Save(obj, file);
            var copy = XorController.Get().Load<Root>(file);
            return copy;
        }
        
        [Test]
        public void PrivateField()
        {
            Assert.AreSame(root.GetPrivateField().GetType(), rootCopy.GetPrivateField().GetType());
            Assert.AreEqual(root.GetPrivateField().Name, rootCopy.GetPrivateField().Name);
        }

        [Test]
        public void PublicField()
        {
            Assert.AreSame(root.PublicField.GetType(), rootCopy.PublicField.GetType());
            Assert.AreEqual(((ILeaf)root.PublicField).Name, ((ILeaf)rootCopy.PublicField).Name);
        }

        [Test]
        public void PrivateProperty()
        {
            Assert.AreSame(root.GetPrivateProperty().GetType(), rootCopy.GetPrivateProperty().GetType());
            Assert.AreEqual(root.GetPrivateProperty().Name, rootCopy.GetPrivateProperty().Name);
        }

        [Test]
        public void PublicProperty()
        {
            Assert.AreSame(root.PublicProperty.GetType(), rootCopy.PublicProperty.GetType());
            Assert.AreEqual(((ILeaf)root.PublicProperty).Name, ((ILeaf)rootCopy.PublicProperty).Name);
        }

        [Test]
        public void PrivateEnumerableField()
        {
            CollectionAssert.AreEqual(root.GetPrivateEnumerableField().Select(o => o.GetType()), rootCopy.GetPrivateEnumerableField().Select(o => o.GetType()));
            CollectionAssert.AreEqual(root.GetPrivateEnumerableField().Select(o => o.Name), rootCopy.GetPrivateEnumerableField().Select(o => o.Name));
        }

        [Test]
        public void PublicEnumerableField()
        {
            CollectionAssert.AreEqual(root.PublicEnumerableField.Select(o => o.GetType()), rootCopy.PublicEnumerableField.Select(o => o.GetType()));
            CollectionAssert.AreEqual(root.PublicEnumerableField.Cast<ILeaf>().Select(o => o.Name), rootCopy.PublicEnumerableField.Cast<ILeaf>().Select(o => o.Name));
        }

        [Test]
        public void PrivateEnumerableProperty()
        {
            CollectionAssert.AreEqual(root.GetPrivateEnumerableProperty().Select(o => o.GetType()), rootCopy.GetPrivateEnumerableProperty().Select(o => o.GetType()));
            CollectionAssert.AreEqual(root.GetPrivateEnumerableProperty().Select(o => o.Name), rootCopy.GetPrivateEnumerableProperty().Select(o => o.Name));
        }

        [Test]
        public void PublicEnumerableProperty()
        {
            CollectionAssert.AreEqual(root.PublicEnumerableProperty.Select(o => o.GetType()), rootCopy.PublicEnumerableProperty.Select(o => o.GetType()));
            CollectionAssert.AreEqual(root.PublicEnumerableProperty.Cast<ILeaf>().Select(o => o.Name), rootCopy.PublicEnumerableProperty.Cast<ILeaf>().Select(o => o.Name));
        }

    }
}
