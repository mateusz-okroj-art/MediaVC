using System;
using System.Collections.Generic;
using System.Reactive;

namespace MediaVC
{
    /// <summary>
    /// Connects traits from enumerables and observables.
    /// </summary>
    /// <typeparam name="T">Destination type</typeparam>
    public interface IObservableEnumerable<out T> : IEnumerable<T>
    {
        /// <summary>
        /// Raises, when item is added
        /// </summary>
        IObservable<T> Added { get; }

        /// <summary>
        /// Raises, when item is removed
        /// </summary>
        IObservable<T> Removed { get; }

        /// <summary>
        /// Raises, when collection is cleared
        /// </summary>
        IObservable<Unit> Cleared { get; }
    }
}
