using System;

namespace LateNightStupidities.XorPersist.Attributes
{
    /// <summary>
    /// Base class for all XorAttributes.
    /// </summary>
    public class XorAttribute : Attribute
    {
        /// <summary>
        /// The Name of the annotated element in the Xor context.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XorAttribute" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected XorAttribute(string name)
        {
            Name = name;
        }
    }
}