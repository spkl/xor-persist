using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LateNightStupidities.XorPersist.Attributes
{
    internal class XorReferenceTuple : Tuple<MemberInfo, XorReferenceAttribute>
    {
        public MemberInfo Info { get { return Item1; } }
        public XorReferenceAttribute Attr { get { return Item2; } }

        public XorReferenceTuple(MemberInfo info, XorReferenceAttribute attribute)
            : base(info, attribute)
        {
        }
    }

    /// <summary>
    /// Use this to mark XorReferences.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class XorReferenceAttribute : XorAttribute
    {
        /// <summary>
        /// Gets the multiplicity of the reference.
        /// </summary>
        public XorMultiplicity Multiplicity { get; private set; }

        private List<Guid> ReferencedIds { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XorReferenceAttribute" /> class with single multiplicity.
        /// </summary>
        /// <param name="name">The name.</param>
        public XorReferenceAttribute(string name)
            : this(name, XorMultiplicity.Single)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XorReferenceAttribute" /> class with the supplied multiplicity.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="multiplicity">The multiplicity.</param>
        public XorReferenceAttribute(string name, XorMultiplicity multiplicity)
            : base(name)
        {
            Multiplicity = multiplicity;
            ReferencedIds = new List<Guid>();
        }

        internal void AddId(Guid id)
        {
            ReferencedIds.Add(id);
        }

        internal Guid GetId()
        {
            if (Multiplicity != XorMultiplicity.Single)
            {
                throw new Exception("GetId() is only allowed for multiplicity Single");
            }

            return ReferencedIds.SingleOrDefault();
        }

        internal IEnumerable<Guid> GetIds()
        {
            if (Multiplicity != XorMultiplicity.List)
            {
                throw new Exception("GetIds() is only allowed for multiplicity List");
            }

            return ReferencedIds;
        }
    }
}