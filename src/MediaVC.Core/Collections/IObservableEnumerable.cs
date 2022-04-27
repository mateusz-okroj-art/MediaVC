using System;
using System.Collections.Generic;
using System.Reactive;

namespace MediaVC
{
    public interface IObservableEnumerable<out T> : IEnumerable<T>
    {
        IObservable<T> Added { get; }

        IObservable<T> Removed { get; }

        IObservable<Unit> Cleared { get; }
    }
}
