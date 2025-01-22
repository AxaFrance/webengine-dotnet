using System;
using System.ComponentModel;

namespace AxaFrance.WebEngine.Web
{
    /// <summary>
    /// Inspired from original Selenium Page Objects, this attribute allows you to quickly create 
    /// <see cref="ElementDescription"/> inside a <see cref="PageModel"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// You can use this attribute by specifying the <see cref="How"/> and <see cref="Using"/> properties
    /// to indicate how to find the elements. This attribute can be used to decorate fields and properties
    /// in your Page Object classes. The <see cref="Type"/> of the field or property must be subtypes of
    /// <see cref="ElementDescription"/>. Any other type will throw an
    /// <see cref="ArgumentException"/> when a <see cref="PageModel"/> is initialized.
    /// </para>
    /// <para>
    /// Compared to original FindsByAttributes implemented in Selenium Support 3.x package, we have removed `Priority`, `CustomFinderType` which is not used in the logic of element locating.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class FindsByAttribute : Attribute
    {

        /// <summary>
        /// Initializing an instance of <see cref="FindsByAttribute"/>
        /// </summary>
        public FindsByAttribute() { }

        /// <summary>
        /// Initializing an instance of <see cref="FindsByAttribute"/> with parameters.
        /// </summary>

        public FindsByAttribute(How how, string @using)
        {
            How = how;
            Using = @using;
        }

        /// <summary>
        /// Gets or sets the method used to look up the element
        /// </summary>
        [DefaultValue(How.Id)]
        public How How { get; set; }

        /// <summary>
        /// Gets or sets the value to lookup by (i.e. for How.Name, the actual name to look up)
        /// </summary>
        public string Using { get; set; }
    }
}
