using System;

namespace MediaVC
{
    /// <summary>
    /// Provides access to reference stored non-class value.
    /// </summary>
    /// <typeparam name="T">Non-class value type.</typeparam>
    public interface IRef<T> where T : struct
    {
        IObservable<T> Changed { get; }

        /// <summary>
        /// Provides access to the stored value
        /// </summary>
        T Value { get; set; }
    }
}