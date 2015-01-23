using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using LateNightStupidities.XorPersist;
using LateNightStupidities.XorPersist.Attributes;
using LateNightStupidities.XorPersist.Exceptions;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class FileInputValidation
    {
        #region Private classes

        [XorClass("DummyClass")]
        private class DummyClass : XorObject
        {
            public DummyClass()
            {
                
            }
        }

        #endregion

        [Test]
        [TestCase(@"TestFiles\SchemaViolation1.xml")]
        [TestCase(@"TestFiles\SchemaViolation2.xml")]
        [TestCase(@"TestFiles\SchemaViolation3.xml")]
        [TestCase(@"TestFiles\SchemaViolation4.xml")]
        [TestCase(@"TestFiles\SchemaViolation5.xml")]
        public void SchemaViolation(string path)
        {
            Assert.Throws<SchemaValidationException>(() => XorController.Get().Load<DummyClass>(path));
        }

        [Test]
        [TestCase(@"TestFiles\NonXmlFile.png")]
        public void NonXmlFile(string path)
        {
            Assert.Throws<XmlException>(() => XorController.Get().Load<DummyClass>(path));
        }

        [Test]
        public void NonExistingFile()
        {
            Assert.Throws<FileNotFoundException>(() => XorController.Get().Load<DummyClass>(@"TestFiles\ThisDoesNotEx.ist"));
        }

        [Test]
        public void AccessDeniedFile()
        {
            const string path = @"TestFiles\AccessDenied";
            using (new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                Assert.Throws<IOException>(() => XorController.Get().Load<DummyClass>(path));
            }
        }
    }
}
