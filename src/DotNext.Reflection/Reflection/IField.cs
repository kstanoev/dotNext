﻿using System.Reflection;

namespace DotNext.Reflection
{
    /// <summary>
    /// Represents reflected field.
    /// </summary>
    public interface IField : IMember<FieldInfo>
    {
        /// <summary>
        /// Indicates that field is read-only.
        /// </summary>
        bool IsReadOnly { get; }
    }

    /// <summary>
    /// Represents static field.
    /// </summary>
    /// <typeparam name="F">Type of field .</typeparam>
    public interface IField<F> : IField
    {
        /// <summary>
        /// Gets or sets field value.
        /// </summary>
        F Value { get; set; }
    }

    /// <summary>
    /// Represents instance field.
    /// </summary>
    /// <typeparam name="T">Field declaring type.</typeparam>
    /// <typeparam name="F">Type of field.</typeparam>
    public interface IField<T, F> : IField
    {
        /// <summary>
        /// Gets or sets instance field value.
        /// </summary>
        /// <param name="this"><c>this</c> parameter.</param>
		F this[in T @this] { get; set; }
    }
}
