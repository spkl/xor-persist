using System;

namespace LateNightStupidities.XorPersist.Attributes
{
    /// <summary>
    /// Use this to mark XorObject classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class XorClassAttribute : XorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XorClassAttribute" /> class.
        /// and uses the supplied name.
        /// </summary>
        /// <param name="name">The name.</param>
        public XorClassAttribute(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XorClassAttribute" /> class
        /// and uses the FullName of the supplied Type as name.
        /// </summary>
        /// <param name="type">The type.</param>
        public XorClassAttribute(Type type)
            : base(type.FullName)
        {
            
        }
    }
}