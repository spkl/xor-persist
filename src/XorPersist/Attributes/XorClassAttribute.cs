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
        /// </summary>
        /// <param name="name">The name.</param>
        public XorClassAttribute(string name)
            : base(name)
        {
        }
    }
}