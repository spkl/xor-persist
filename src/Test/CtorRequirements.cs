using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LateNightStupidities.XorPersist;
using LateNightStupidities.XorPersist.Attributes;
using LateNightStupidities.XorPersist.Exceptions;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class CtorRequirements
    {
        #region Private classes

        [XorClass(typeof(_PublicParameterless))]
        private class _PublicParameterless : XorObject
        {
            public _PublicParameterless()
            {
                
            }
        }
        
        [XorClass(typeof(_PrivateParameterless))]
        private class _PrivateParameterless : XorObject
        {
            private _PrivateParameterless()
            {

            }

            internal _PrivateParameterless(int dummy)
            {
                
            }
        }

        [XorClass(typeof(_PrivateParameterlessWithXorCreate))]
        private class _PrivateParameterlessWithXorCreate : XorObject
        {
            private _PrivateParameterlessWithXorCreate()
            {

            }

            internal _PrivateParameterlessWithXorCreate(int dummy)
            {
                
            }

            private static object _XorCreate()
            {
                return new _PrivateParameterlessWithXorCreate();
            }
        }

        [XorClass(typeof(_PrivateParameterlessWithPublicXorCreate))]
        private class _PrivateParameterlessWithPublicXorCreate : XorObject
        {
            private _PrivateParameterlessWithPublicXorCreate()
            {

            }

            internal _PrivateParameterlessWithPublicXorCreate(int dummy)
            {

            }

            public static object _XorCreate()
            {
                return new _PrivateParameterlessWithPublicXorCreate();
            }
        }

        #endregion

        [Test]
        public void PublicParameterless()
        {
            Assert.DoesNotThrow(() => TestHelper.SaveAndLoad(new _PublicParameterless()));
        }

        [Test]
        public void PrivateParameterless()
        {
            Assert.Throws<CtorMissingException>(() => TestHelper.SaveAndLoad(new _PrivateParameterless(0)));
        }

        [Test]
        public void PrivateParameterlessWithXorCreate()
        {
            Assert.DoesNotThrow(() => TestHelper.SaveAndLoad(new _PrivateParameterlessWithXorCreate(0)));
        }

        [Test]
        public void PrivateParameterlessWithPublicXorCreate()
        {
            Assert.DoesNotThrow(() => TestHelper.SaveAndLoad(new _PrivateParameterlessWithPublicXorCreate(0)));
        }
    }
}
