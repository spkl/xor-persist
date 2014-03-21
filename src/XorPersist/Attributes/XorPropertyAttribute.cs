using System;
using System.Reflection;

namespace LateNightStupidities.XorPersist.Attributes
{
    internal class XorPropertyTuple : Tuple<MemberInfo, XorPropertyAttribute>
    {
        public MemberInfo Info { get { return Item1; } }
        public XorPropertyAttribute Attr { get { return Item2; } }

        public XorPropertyTuple(MemberInfo info, XorPropertyAttribute attribute)
            : base(info, attribute)
        {
        }
    }

    /// <summary>
    /// Use this to mark XorProperties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class XorPropertyAttribute : XorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XorPropertyAttribute" /> class with single multiplicity.
        /// </summary>
        /// <param name="name">The name.</param>
        public XorPropertyAttribute(string name)
            : this(name, XorMultiplicity.Single)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XorPropertyAttribute" /> class with the supplied multiplicity.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="multiplicity">The multiplicity.</param>
        public XorPropertyAttribute(string name, XorMultiplicity multiplicity)
            : base(name)
        {
            Multiplicity = multiplicity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XorPropertyAttribute" /> class with list multiplicity.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="listItemType">Type of the list item.</param>
        public XorPropertyAttribute(string name, Type listItemType)
            : this(name, XorMultiplicity.List)
        {
            ListItemType = listItemType;
        }
    }
}