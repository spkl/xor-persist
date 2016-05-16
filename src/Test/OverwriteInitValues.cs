using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LateNightStupidities.XorPersist;
using LateNightStupidities.XorPersist.Attributes;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class OverwriteInitValues
    {
        #region Private classes

        [XorClass(typeof(ClassWithInitValues))]
        private class ClassWithInitValues : XorObject
        {
            [XorProperty(nameof(ExplicitStringInit))]
            public string ExplicitStringInit = "foo";

            [XorProperty(nameof(CtorStringInit))]
            public string CtorStringInit;

            [XorProperty(nameof(ExplicitDoubleInit))]
            public double? ExplicitDoubleInit = 2;

            [XorProperty(nameof(CtorDoubleInit))]
            public double? CtorDoubleInit;

            [XorProperty(nameof(ExplicitDoubleListInit), XorMultiplicity.List)]
            public List<double?> ExplicitDoubleListInit = new List<double?>(new double?[] {2, null});

            [XorProperty(nameof(CtorDoubleListInit), XorMultiplicity.List)]
            public List<double?> CtorDoubleListInit;

            [XorProperty(nameof(CtorChildInit))]
            public ClassWithInitValues CtorChildInit;

            [XorProperty(nameof(CtorChildListInit), XorMultiplicity.List)]
            public IEnumerable<ClassWithInitValues> CtorChildListInit;

            [XorReference(nameof(CtorRefInit))]
            public ClassWithInitValues CtorRefInit;

            [XorReference(nameof(CtorRefListInit), XorMultiplicity.List)]
            public IEnumerable<ClassWithInitValues> CtorRefListInit;

            public ClassWithInitValues() : this(true)
            {

            }

            public ClassWithInitValues(bool createChild)
            {
                this.CtorStringInit = "foo2";
                this.CtorDoubleInit = 22;
                this.CtorDoubleListInit = new List<double?>(new double?[] {22, null});
                if (createChild)
                {
                    this.CtorChildInit = new ClassWithInitValues(false);
                    this.CtorRefInit = this.CtorChildInit;
                    this.CtorChildListInit = new[] {new ClassWithInitValues(false),};
                    this.CtorRefListInit = this.CtorChildListInit.ToArray();
                }
            }
        }

        #endregion

        [Test]
        public void NullValuesRestored()
        {
            var c = new ClassWithInitValues();
            c.CtorChildInit = null;
            c.CtorRefInit = null;
            c.CtorDoubleInit = null;
            c.CtorDoubleListInit = null;
            c.CtorStringInit = null;
            c.ExplicitDoubleInit = null;
            c.ExplicitDoubleListInit = null;
            c.ExplicitStringInit = null;
            c.CtorChildListInit = null;
            c.CtorRefListInit = null;

            TestHelper.TestIteration(c);
        }
    }
}
