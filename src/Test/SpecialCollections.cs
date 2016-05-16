using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LateNightStupidities.XorPersist;
using LateNightStupidities.XorPersist.Attributes;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class SpecialCollections
    {
        [XorClass("SpecialListItem")]
        private class SpecialListItem : XorObject, IComparable
        {
            [XorProperty("Guid")]
            private readonly Guid guid;

            [XorProperty("IntVal")] 
            private readonly int intVal;

            public SpecialListItem()
            {
                
            }

            public SpecialListItem(int intVal)
            {
                this.guid = Guid.NewGuid();
                this.intVal = intVal;
            }

            public override bool Equals(object obj)
            {
                var listItem = obj as SpecialListItem;
                return listItem != null && this.guid.Equals(listItem.guid);
            }

            public override int GetHashCode()
            {
                return guid.GetHashCode();
            }

            public int CompareTo(object obj)
            {
                return ((SpecialListItem)obj).intVal - this.intVal;
            }
        }

        private static SpecialListItem[] GetObjects()
        {
            return new[] { new SpecialListItem(1), new SpecialListItem(3), new SpecialListItem(2) };
        }

        private static int[] GetInts()
        {
            return new[] { -100, -10, 0, 10, 100, 1234567 };
        }

        private static char[] GetChars()
        {
            return new[] { char.MinValue, char.MaxValue, 'a', 'A', 'ö', 'Ö', 'ß', ' ' };
        }

        // ReSharper disable InconsistentNaming

        #region IEnumerable

        [XorClass("IEnumerables")]
        private class IEnumerables : XorObject
        {
            [XorProperty("Ints", typeof(int))]
            public IEnumerable Ints;

            [XorProperty("Chars", typeof(char))]
            public IEnumerable Chars;

            [XorProperty("Objects", typeof(SpecialListItem))]
            public IEnumerable Objects;

            [XorReference("ObjectReferences", XorMultiplicity.List)]
            public IEnumerable ObjectReferences;
        }

        [Test]
        public void SupportsIEnumerable()
        {
            var listHolder = new IEnumerables { Ints = GetInts(), Chars = GetChars(), Objects = GetObjects() };
            listHolder.ObjectReferences = listHolder.Objects;
            var copy = TestHelper.SaveAndLoad(listHolder);

            CollectionAssert.AreEqual(listHolder.Ints, copy.Ints);
            CollectionAssert.AreEqual(listHolder.Chars, copy.Chars);
            CollectionAssert.AreEqual(listHolder.Objects, copy.Objects);
            CollectionAssert.AreEqual(listHolder.ObjectReferences, copy.ObjectReferences);

            
        }

        #endregion

        #region Array

        [XorClass("Arrays")]
        private class Arrays : XorObject
        {
            [XorProperty("Ints", XorMultiplicity.List)]
            public int[] Ints;

            [XorProperty("Chars", XorMultiplicity.List)]
            public char[] Chars;

            [XorProperty("Objects", XorMultiplicity.List)]
            public SpecialListItem[] Objects;

            [XorReference("ObjectReferences", XorMultiplicity.List)]
            public SpecialListItem[] ObjectReferences;
        }

        [Test]
        public void SupportsArray()
        {
            var listHolder = new Arrays { Ints = GetInts(), Chars = GetChars(), Objects = GetObjects() };
            listHolder.ObjectReferences = listHolder.Objects.ToArray();
            var copy = TestHelper.SaveAndLoad(listHolder);

            CollectionAssert.AreEqual(listHolder.Ints, copy.Ints);
            CollectionAssert.AreEqual(listHolder.Chars, copy.Chars);
            CollectionAssert.AreEqual(listHolder.Objects, copy.Objects);
            CollectionAssert.AreEqual(listHolder.ObjectReferences, copy.ObjectReferences);

            
        }

        #endregion

        #region System.Collections.ArrayList

        [XorClass("ArrayLists")]
        private class ArrayLists : XorObject
        {
            [XorProperty("Ints", typeof(int))]
            public ArrayList Ints;

            [XorProperty("Chars", typeof(char))]
            public ArrayList Chars;

            [XorProperty("Objects", typeof(SpecialListItem))]
            public ArrayList Objects;

            [XorReference("ObjectReferences", XorMultiplicity.List)]
            public ArrayList ObjectReferences;
        }

        [Test]
        public void SupportsArrayList()
        {
            var listHolder = new ArrayLists {Ints = new ArrayList(GetInts()), Chars = new ArrayList(GetChars()), Objects = new ArrayList(GetObjects())};
            listHolder.ObjectReferences = new ArrayList(listHolder.Objects);
            var copy = TestHelper.SaveAndLoad(listHolder);

            CollectionAssert.AreEqual(listHolder.Ints, copy.Ints);
            CollectionAssert.AreEqual(listHolder.Chars, copy.Chars);
            CollectionAssert.AreEqual(listHolder.Objects, copy.Objects);
            CollectionAssert.AreEqual(listHolder.ObjectReferences, copy.ObjectReferences);

            
        }

        #endregion

        #region System.Collections.ICollection

        [XorClass("ICollections")]
        private class ICollections : XorObject
        {
            [XorProperty("Ints", typeof(int))]
            public ICollection Ints;

            [XorProperty("Chars", typeof(char))]
            public ICollection Chars;

            [XorProperty("Objects", typeof(SpecialListItem))]
            public ICollection Objects;

            [XorReference("ObjectReferences", XorMultiplicity.List)]
            public ICollection ObjectReferences;
        }

        [Test]
        public void SupportsICollection()
        {
            var listHolder = new ICollections {Ints = new ArrayList(GetInts()), Chars = new ArrayList(GetChars()), Objects = new ArrayList(GetObjects())};
            listHolder.ObjectReferences = new ArrayList(listHolder.Objects);
            var copy = TestHelper.SaveAndLoad(listHolder);

            CollectionAssert.AreEqual(listHolder.Ints, copy.Ints);
            CollectionAssert.AreEqual(listHolder.Chars, copy.Chars);
            CollectionAssert.AreEqual(listHolder.Objects, copy.Objects);
            CollectionAssert.AreEqual(listHolder.ObjectReferences, copy.ObjectReferences);

            
        }

        #endregion

        #region System.Collections.IList

        [XorClass("ILists")]
        private class ILists : XorObject
        {
            [XorProperty("Ints", typeof(int))]
            public IList Ints;

            [XorProperty("Chars", typeof(char))]
            public IList Chars;

            [XorProperty("Objects", typeof(SpecialListItem))]
            public IList Objects;

            [XorReference("ObjectReferences", XorMultiplicity.List)]
            public IList ObjectReferences;
        }

        [Test]
        public void SupportsIList()
        {
            var listHolder = new ILists {Ints = new ArrayList(GetInts()), Chars = new ArrayList(GetChars()), Objects = new ArrayList(GetObjects())};
            listHolder.ObjectReferences = new ArrayList(listHolder.Objects);
            var copy = TestHelper.SaveAndLoad(listHolder);

            CollectionAssert.AreEqual(listHolder.Ints, copy.Ints);
            CollectionAssert.AreEqual(listHolder.Chars, copy.Chars);
            CollectionAssert.AreEqual(listHolder.Objects, copy.Objects);
            CollectionAssert.AreEqual(listHolder.ObjectReferences, copy.ObjectReferences);

            
        }

        #endregion

        #region System.Collections.Generic.HashSet<T>

        [XorClass("HashSets")]
        private class HashSets : XorObject
        {
            [XorProperty("Ints", XorMultiplicity.List)]
            public HashSet<int> Ints;

            [XorProperty("Chars", XorMultiplicity.List)]
            public HashSet<char> Chars;

            [XorProperty("Objects", XorMultiplicity.List)]
            public HashSet<SpecialListItem> Objects;

            [XorReference("ObjectReferences", XorMultiplicity.List)]
            public HashSet<SpecialListItem> ObjectReferences;
        }

        [Test]
        public void SupportsHashSet()
        {
            var listHolder = new HashSets { Ints = new HashSet<int>(GetInts()), Chars = new HashSet<char>(GetChars()), Objects = new HashSet<SpecialListItem>(GetObjects()) };
            listHolder.ObjectReferences = new HashSet<SpecialListItem>(listHolder.Objects);
            var copy = TestHelper.SaveAndLoad(listHolder);

            CollectionAssert.AreEquivalent(listHolder.Ints, copy.Ints);
            CollectionAssert.AreEquivalent(listHolder.Chars, copy.Chars);
            CollectionAssert.AreEquivalent(listHolder.Objects, copy.Objects);
            CollectionAssert.AreEquivalent(listHolder.ObjectReferences, copy.ObjectReferences);

            
        }

        #endregion

        #region System.Collections.Generic.List<T>

        [XorClass("ListTs")]
        private class ListTs : XorObject
        {
            [XorProperty("Ints", XorMultiplicity.List)]
            public List<int> Ints;

            [XorProperty("Chars", XorMultiplicity.List)]
            public List<char> Chars;

            [XorProperty("Objects", XorMultiplicity.List)]
            public List<SpecialListItem> Objects;

            [XorReference("ObjectReferences", XorMultiplicity.List)]
            public List<SpecialListItem> ObjectReferences;
        }

        [Test]
        public void SupportsList()
        {
            var listHolder = new ListTs { Ints = new List<int>(GetInts()), Chars = new List<char>(GetChars()), Objects = new List<SpecialListItem>(GetObjects()) };
            listHolder.ObjectReferences = new List<SpecialListItem>(listHolder.Objects);
            var copy = TestHelper.SaveAndLoad(listHolder);

            CollectionAssert.AreEqual(listHolder.Ints, copy.Ints);
            CollectionAssert.AreEqual(listHolder.Chars, copy.Chars);
            CollectionAssert.AreEqual(listHolder.Objects, copy.Objects);
            CollectionAssert.AreEqual(listHolder.ObjectReferences, copy.ObjectReferences);

            
        }

        #endregion

        #region System.Collections.Generic.SortedSet<T>

        [XorClass("SortedSets")]
        private class SortedSets : XorObject
        {
            [XorProperty("Ints", XorMultiplicity.List)]
            public SortedSet<int> Ints;

            [XorProperty("Chars", XorMultiplicity.List)]
            public SortedSet<char> Chars;

            [XorProperty("Objects", XorMultiplicity.List)]
            public SortedSet<SpecialListItem> Objects;

            [XorReference("ObjectReferences", XorMultiplicity.List)]
            public SortedSet<SpecialListItem> ObjectReferences;
        }

        [Test]
        public void SupportsSortedSet()
        {
            var listHolder = new SortedSets { Ints = new SortedSet<int>(GetInts()), Chars = new SortedSet<char>(GetChars()), Objects = new SortedSet<SpecialListItem>(GetObjects()) };
            listHolder.ObjectReferences = new SortedSet<SpecialListItem>(listHolder.Objects);
            var copy = TestHelper.SaveAndLoad(listHolder);

            CollectionAssert.AreEqual(listHolder.Ints, copy.Ints);
            CollectionAssert.AreEqual(listHolder.Chars, copy.Chars);
            CollectionAssert.AreEqual(listHolder.Objects, copy.Objects);
            CollectionAssert.AreEqual(listHolder.ObjectReferences, copy.ObjectReferences);

            
        }

        #endregion

        #region System.Collections.Generic.ICollection<T>

        [XorClass("ICollectionTs")]
        private class ICollectionTs : XorObject
        {
            [XorProperty("Ints", XorMultiplicity.List)]
            public ICollection<int> Ints;

            [XorProperty("Chars", XorMultiplicity.List)]
            public ICollection<char> Chars;

            [XorProperty("Objects", XorMultiplicity.List)]
            public ICollection<SpecialListItem> Objects;

            [XorReference("ObjectReferences", XorMultiplicity.List)]
            public ICollection<SpecialListItem> ObjectReferences;
        }

        [Test]
        public void SupportsICollectionT()
        {
            var listHolder = new ICollectionTs { Ints = new List<int>(GetInts()), Chars = new List<char>(GetChars()), Objects = new List<SpecialListItem>(GetObjects()) };
            listHolder.ObjectReferences = new List<SpecialListItem>(listHolder.Objects);
            var copy = TestHelper.SaveAndLoad(listHolder);

            CollectionAssert.AreEqual(listHolder.Ints, copy.Ints);
            CollectionAssert.AreEqual(listHolder.Chars, copy.Chars);
            CollectionAssert.AreEqual(listHolder.Objects, copy.Objects);
            CollectionAssert.AreEqual(listHolder.ObjectReferences, copy.ObjectReferences);

            
        }

        #endregion

        #region System.Collections.Generic.IList<T>

        [XorClass("IListTs")]
        private class IListTs : XorObject
        {
            [XorProperty("Ints", XorMultiplicity.List)]
            public IList<int> Ints;

            [XorProperty("Chars", XorMultiplicity.List)]
            public IList<char> Chars;

            [XorProperty("Objects", XorMultiplicity.List)]
            public IList<SpecialListItem> Objects;

            [XorReference("ObjectReferences", XorMultiplicity.List)]
            public IList<SpecialListItem> ObjectReferences;
        }

        [Test]
        public void SupportsIListT()
        {
            var listHolder = new IListTs { Ints = new List<int>(GetInts()), Chars = new List<char>(GetChars()), Objects = new List<SpecialListItem>(GetObjects()) };
            listHolder.ObjectReferences = new List<SpecialListItem>(listHolder.Objects);
            var copy = TestHelper.SaveAndLoad(listHolder);

            CollectionAssert.AreEqual(listHolder.Ints, copy.Ints);
            CollectionAssert.AreEqual(listHolder.Chars, copy.Chars);
            CollectionAssert.AreEqual(listHolder.Objects, copy.Objects);
            CollectionAssert.AreEqual(listHolder.ObjectReferences, copy.ObjectReferences);

            
        }

        #endregion

        #region System.Collections.Generic.ISet<T>

        [XorClass("ISets")]
        private class ISets : XorObject
        {
            [XorProperty("Ints", XorMultiplicity.List)]
            public ISet<int> Ints;

            [XorProperty("Chars", XorMultiplicity.List)]
            public ISet<char> Chars;

            [XorProperty("Objects", XorMultiplicity.List)]
            public ISet<SpecialListItem> Objects;

            [XorReference("ObjectReferences", XorMultiplicity.List)]
            public ISet<SpecialListItem> ObjectReferences;
        }

        [Test]
        public void SupportsISet()
        {
            var listHolder = new ISets { Ints = new HashSet<int>(GetInts()), Chars = new HashSet<char>(GetChars()), Objects = new HashSet<SpecialListItem>(GetObjects()) };
            listHolder.ObjectReferences = new HashSet<SpecialListItem>(listHolder.Objects);
            var copy = TestHelper.SaveAndLoad(listHolder);

            CollectionAssert.AreEquivalent(listHolder.Ints, copy.Ints);
            CollectionAssert.AreEquivalent(listHolder.Chars, copy.Chars);
            CollectionAssert.AreEquivalent(listHolder.Objects, copy.Objects);
            CollectionAssert.AreEquivalent(listHolder.ObjectReferences, copy.ObjectReferences);

            
        }

        #endregion

        #region System.Collections.Queue

        [XorClass("Queues")]
        private class Queues : XorObject
        {
            [XorProperty("Ints", typeof(int))]
            public Queue Ints;

            [XorProperty("Chars", typeof(char))]
            public Queue Chars;

            [XorProperty("Objects", typeof(SpecialListItem))]
            public Queue Objects;

            [XorReference("ObjectReferences", XorMultiplicity.List)]
            public Queue ObjectReferences;
        }

        [Test]
        public void SupportsQueue()
        {
            var listHolder = new Queues { Ints = new Queue(GetInts()), Chars = new Queue(GetChars()), Objects = new Queue(GetObjects()) };
            listHolder.ObjectReferences = new Queue(listHolder.Objects);
            var copy = TestHelper.SaveAndLoad(listHolder);

            CollectionAssert.AreEqual(listHolder.Ints, copy.Ints);
            CollectionAssert.AreEqual(listHolder.Chars, copy.Chars);
            CollectionAssert.AreEqual(listHolder.Objects, copy.Objects);
            CollectionAssert.AreEqual(listHolder.ObjectReferences, copy.ObjectReferences);

            
        }

        #endregion

        #region System.Collections.Stack

        [XorClass("Stacks")]
        private class Stacks : XorObject
        {
            [XorProperty("Ints", typeof(int))]
            public Stack Ints;

            [XorProperty("Chars", typeof(char))]
            public Stack Chars;

            [XorProperty("Objects", typeof(SpecialListItem))]
            public Stack Objects;

            [XorReference("ObjectReferences", XorMultiplicity.List)]
            public Stack ObjectReferences;
        }

        [Test]
        public void SupportsStack()
        {
            var listHolder = new Stacks { Ints = new Stack(GetInts()), Chars = new Stack(GetChars()), Objects = new Stack(GetObjects()) };
            listHolder.ObjectReferences = new Stack(listHolder.Objects);
            var copy = TestHelper.SaveAndLoad(listHolder);

            CollectionAssert.AreEqual(listHolder.Ints, copy.Ints);
            CollectionAssert.AreEqual(listHolder.Chars, copy.Chars);
            CollectionAssert.AreEqual(listHolder.Objects, copy.Objects);
            CollectionAssert.AreEqual(listHolder.ObjectReferences, copy.ObjectReferences);

            
        }

        #endregion

        #region System.Collections.Generic.LinkedList<T>

        [XorClass("LinkedListTs")]
        private class LinkedListTs : XorObject
        {
            [XorProperty("Ints", XorMultiplicity.List)]
            public LinkedList<int> Ints;

            [XorProperty("Chars", XorMultiplicity.List)]
            public LinkedList<char> Chars;

            [XorProperty("Objects", XorMultiplicity.List)]
            public LinkedList<SpecialListItem> Objects;

            [XorReference("ObjectReferences", XorMultiplicity.List)]
            public LinkedList<SpecialListItem> ObjectReferences;
        }

        [Test]
        public void SupportsLinkedListT()
        {
            var listHolder = new LinkedListTs { Ints = new LinkedList<int>(GetInts()), Chars = new LinkedList<char>(GetChars()), Objects = new LinkedList<SpecialListItem>(GetObjects()) };
            listHolder.ObjectReferences = new LinkedList<SpecialListItem>(listHolder.Objects);
            var copy = TestHelper.SaveAndLoad(listHolder);

            CollectionAssert.AreEqual(listHolder.Ints, copy.Ints);
            CollectionAssert.AreEqual(listHolder.Chars, copy.Chars);
            CollectionAssert.AreEqual(listHolder.Objects, copy.Objects);
            CollectionAssert.AreEqual(listHolder.ObjectReferences, copy.ObjectReferences);

            
        }

        #endregion

        #region System.Collections.Generic.Queue<T>

        [XorClass("QueueTs")]
        private class QueueTs : XorObject
        {
            [XorProperty("Ints", XorMultiplicity.List)]
            public Queue<int> Ints;

            [XorProperty("Chars", XorMultiplicity.List)]
            public Queue<char> Chars;

            [XorProperty("Objects", XorMultiplicity.List)]
            public Queue<SpecialListItem> Objects;

            [XorReference("ObjectReferences", XorMultiplicity.List)]
            public Queue<SpecialListItem> ObjectReferences;
        }

        [Test]
        public void SupportsQueueT()
        {
            var listHolder = new QueueTs { Ints = new Queue<int>(GetInts()), Chars = new Queue<char>(GetChars()), Objects = new Queue<SpecialListItem>(GetObjects()) };
            listHolder.ObjectReferences = new Queue<SpecialListItem>(listHolder.Objects);
            var copy = TestHelper.SaveAndLoad(listHolder);

            CollectionAssert.AreEqual(listHolder.Ints, copy.Ints);
            CollectionAssert.AreEqual(listHolder.Chars, copy.Chars);
            CollectionAssert.AreEqual(listHolder.Objects, copy.Objects);
            CollectionAssert.AreEqual(listHolder.ObjectReferences, copy.ObjectReferences);

            
        }

        #endregion

        #region System.Collections.Generic.Stack<T>

        [XorClass("StackTs")]
        private class StackTs : XorObject
        {
            [XorProperty("Ints", XorMultiplicity.List)]
            public Stack<int> Ints;

            [XorProperty("Chars", XorMultiplicity.List)]
            public Stack<char> Chars;

            [XorProperty("Objects", XorMultiplicity.List)]
            public Stack<SpecialListItem> Objects;

            [XorReference("ObjectReferences", XorMultiplicity.List)]
            public Stack<SpecialListItem> ObjectReferences;
        }

        [Test]
        public void SupportsStackT()
        {
            var listHolder = new StackTs { Ints = new Stack<int>(GetInts()), Chars = new Stack<char>(GetChars()), Objects = new Stack<SpecialListItem>(GetObjects()) };
            listHolder.ObjectReferences = new Stack<SpecialListItem>(listHolder.Objects);
            var copy = TestHelper.SaveAndLoad(listHolder);

            CollectionAssert.AreEqual(listHolder.Ints, copy.Ints);
            CollectionAssert.AreEqual(listHolder.Chars, copy.Chars);
            CollectionAssert.AreEqual(listHolder.Objects, copy.Objects);
            CollectionAssert.AreEqual(listHolder.ObjectReferences, copy.ObjectReferences);

            
        }

        #endregion

        // ReSharper restore InconsistentNaming
    }
}
