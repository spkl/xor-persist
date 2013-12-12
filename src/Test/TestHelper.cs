using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LateNightStupidities.XorPersist;
using NUnit.Framework;

namespace Test
{
    internal static class TestHelper
    {
        public static TRoot SaveAndLoad<TRoot>(TRoot obj, out string file) where TRoot : XorObject
        {
            file = Path.GetTempFileName();
            XorController.Get().Save(obj, file);
            var copy = XorController.Get().Load<TRoot>(file);
            return copy;
        }

        public static TRoot SaveAndLoad<TRoot>(TRoot obj) where TRoot : XorObject
        {
            string file;
            return SaveAndLoad(obj, out file);
        }

        public static string FileContent(string path)
        {
            using (var reader = new StreamReader(path))
            {
                return reader.ReadToEnd();
            }
        }

        public static void AssertFilesEqual(string expected, string actual)
        {
            Assert.AreEqual(FileContent(expected), FileContent(actual));
        }

        public static void TestIteration(XorObject obj)
        {
            string path1, path2, path3;
            var copy1 = SaveAndLoad(obj, out path1);
            var copy2 = SaveAndLoad(copy1, out path2);
            var copy3 = SaveAndLoad(copy2, out path3);

            AssertFilesEqual(path1, path2);
            AssertFilesEqual(path2, path3);
        }
    }
}
