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
        /// The multiplicity of the property.
        /// </summary>
        public XorMultiplicity Multiplicity { get; private set; }

        /// <summary>
        /// Only for properties with <see cref="Multiplicity"/> "<see cref="XorMultiplicity.List"/>":
        /// The type of the list items. This must be a class that is derived from <see cref="XorObject"/> or an interface.
        /// </summary>
        public Type ListItemType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XorPropertyAttribute" /> class with single multiplicity.
        /// </summary>
        /// <param name="name">The name.</param>
        public XorPropertyAttribute(string name)
            : this(name, XorMultiplicity.Single)
        {
        }

        private XorPropertyAttribute(string name, XorMultiplicity multiplicity)
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