﻿using System;
using LateNightStupidities.XorPersist.Exceptions;

namespace LateNightStupidities.XorPersist.Attributes
{
    /// <summary>
    /// Base class for all XorAttributes.
    /// </summary>
    public class XorAttribute : Attribute
    {
        /// <summary>
        /// This string is used as default value when a CallerMemberName attribute does not supply a value.
        /// </summary>
        internal const string CallerMemberNameNotAvailable = "CallerMemberName/Not/Available";

        /// <summary>
        /// The Name of the annotated element in the Xor context.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The multiplicity of the property.
        /// </summary>
        public XorMultiplicity Multiplicity { get; protected set; }

        /// <summary>
        /// Only for properties with <see cref="Multiplicity"/> "<see cref="XorMultiplicity.List"/>":
        /// The type of the list items. This must be a class that is derived from <see cref="XorObject"/>, 
        /// an interface or (only for property lists, not reference lists) a supported simple type.
        /// </summary>
        public Type ListItemType { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XorAttribute" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected XorAttribute(string name)
        {
            if (name == CallerMemberNameNotAvailable)
            {
                throw new InvalidXorAttributeNameException(Properties.Exceptions.InvalidXorAttributeNameExceptionCallerMemberName);
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new InvalidXorAttributeNameException(Properties.Exceptions.InvalidXorAttributeNameExceptionNullEmptyWhitespace);
            }

            Name = name;
        }
    }
}