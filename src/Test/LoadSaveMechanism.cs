using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using LateNightStupidities.XorPersist;
using LateNightStupidities.XorPersist.Attributes;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class LoadSaveMechanism
    {
        #region Private classes

        [XorClass("Minimal")]
        private class Minimal : XorObject
        {
            [XorProperty("Minimal")]
            public Minimal Child { get; set; }

            public DateTime? InitializeCalled { get; private set; }

            public DateTime? FinishCalled { get; private set; }

            protected override void XorInitialize()
            {
                InitializeCalled = DateTime.Now;
            }

            protected override void XorFinish()
            {
                Thread.Sleep(20);
                FinishCalled = DateTime.Now;
            }
        }

        #endregion

        [Test]
        public void InitializeCalled()
        {
            var obj = new Minimal { Child = new Minimal { Child = new Minimal() } };
            obj = TestHelper.SaveAndLoad(obj);

            Assert.IsTrue(obj.InitializeCalled.HasValue);
            Assert.IsTrue(obj.Child.InitializeCalled.HasValue);
            Assert.IsTrue(obj.Child.Child.InitializeCalled.HasValue);

            Assert.IsFalse(obj.FinishCalled.HasValue);
            Assert.IsFalse(obj.Child.FinishCalled.HasValue);
            Assert.IsFalse(obj.Child.Child.FinishCalled.HasValue);
        }

        [Test]
        public void FinishCalled()
        {
            var obj = new Minimal { Child = new Minimal { Child = new Minimal() } };
            TestHelper.SaveAndLoad(obj);

            Assert.IsFalse(obj.InitializeCalled.HasValue);
            Assert.IsFalse(obj.Child.InitializeCalled.HasValue);
            Assert.IsFalse(obj.Child.Child.InitializeCalled.HasValue);

            Assert.IsTrue(obj.FinishCalled.HasValue);
            Assert.IsTrue(obj.Child.FinishCalled.HasValue);
            Assert.IsTrue(obj.Child.Child.FinishCalled.HasValue);

            // check order of calls
            Assert.Less(obj.FinishCalled, obj.Child.FinishCalled);
            Assert.Less(obj.Child.FinishCalled, obj.Child.Child.FinishCalled);
        }
    }
}
