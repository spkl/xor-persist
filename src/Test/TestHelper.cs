using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LateNightStupidities.XorPersist;

namespace Test
{
    internal static class TestHelper
    {
        public static TRoot SaveAndLoad<TRoot>(TRoot obj) where TRoot : XorObject
        {
            string file = Path.GetTempFileName();
            XorController.Get().Save(obj, file);
            var copy = XorController.Get().Load<TRoot>(file);
            return copy;
        }
    }
}
