using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LateNightStupidities.XorPersist;
using LateNightStupidities.XorPersist.Attributes;
using LateNightStupidities.XorPersist.Exceptions;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class CallerMemberName
    {
        #region Private classes

        [XorClass(typeof(ClassWithInvalidField))]
        private class ClassWithInvalidField : XorObject
        {
            [XorProperty]
            public string thisIsAPrivateField = "foo";
        }
        
        [XorClass(typeof(TestA))]
        private class TestA : XorObject
        {
            [XorProperty]
            public string TestProperty { get; set; }

            [XorReference]
            public TestA TestReference { get; set; }
        }

        #endregion

        [Test]
        public void ExceptionIsThrown()
        {
            var obj = new ClassWithInvalidField();
            Assert.Throws<InvalidXorAttributeNameException>(() => XorController.Get().Save(obj, Path.GetTempFileName()));
        }

        [Test]
        public void CallerMemberNameIsPassed()
        {
            var testA = new TestA();
            testA.TestProperty = "foo";
            testA.TestReference = testA;

            TestHelper.TestIteration(testA);
        }
    }
}
