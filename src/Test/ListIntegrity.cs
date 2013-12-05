﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using LateNightStupidities.XorPersist;
using LateNightStupidities.XorPersist.Attributes;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class ListIntegrity
    {
        #region Private classes

        [XorClass("Lists")]
        private class Lists : XorObject
        {
            [XorProperty("Strings", typeof(string))]
            public IEnumerable<string> Strings;

            [XorProperty("Doubles", typeof(double))]
            public IEnumerable<double> Doubles;

            [XorProperty("Objects", typeof(ListItem))]
            public IEnumerable<ListItem> Objects;

            public Lists()
            {
                
            }
        }

        [XorClass("ListItem")]
        private class ListItem : XorObject
        {
            [XorProperty("Guid")]
            private readonly Guid guid;

            public ListItem()
            {
                guid = Guid.NewGuid();
            }

            public override bool Equals(object obj)
            {
                var listItem = obj as ListItem;
                return listItem != null && this.guid.Equals(listItem.guid);
            }

            public override int GetHashCode()
            {
                return guid.GetHashCode();
            }
        }

        #endregion

        [Test]
        public void ValueList()
        {
            var lists = new Lists();
            lists.Doubles = new[] {1.23456789123456789123456789123456789123456789, 2, 3, 4, Math.PI};
            lists.Strings = new[] {null, "This", null, "is", null, "a", null, "", null, "test", null, null, null, "\t", "foo\n\nbar"};

            var copy = TestHelper.SaveAndLoad(lists);

            CollectionAssert.AreEqual(lists.Doubles, copy.Doubles);
            CollectionAssert.AreEqual(lists.Strings, copy.Strings);

            var doubles = new double[1000];
            var strings = new string[doubles.Length];

            for (int i = 0; i < doubles.Length; i++)
            {
                doubles[i] = i;
                strings[i] = i.ToString(CultureInfo.InvariantCulture);

                if (i%13 == 0)
                {
                    doubles[i] = default(double);
                    strings[i] = null;
                }
            }

            lists.Doubles = doubles;
            lists.Strings = strings;

            copy = TestHelper.SaveAndLoad(lists);

            CollectionAssert.AreEqual(lists.Doubles, copy.Doubles);
            CollectionAssert.AreEqual(lists.Strings, copy.Strings);
        }

        [Test]
        public void ObjectList()
        {
            var listItems = new ListItem[1000];

            for (int i = 0; i < listItems.Length; i++)
            {
                listItems[i] = new ListItem();

                if (i%13 == 0)
                {
                    listItems[i] = null;
                }
            }

            var lists = new Lists();
            lists.Objects = listItems;

            var copy = TestHelper.SaveAndLoad(lists);

            CollectionAssert.AreEqual(lists.Objects, copy.Objects);
        }
    }
}