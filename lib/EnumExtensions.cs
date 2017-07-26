using System;
using JetBrains.Annotations;

namespace lib
{
    public static class EnumExtensions
    {
        /// <summary>
        ///     Gets an attribute on an enum field value
        /// </summary>
        [CanBeNull]
        public static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString())[0];
            var attributes = memberInfo.GetCustomAttributes(typeof(TAttribute), false);
            return attributes.Length > 0 ? (TAttribute) attributes[0] : null;
        }
    }
}