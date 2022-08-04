using System;
using System.Reactive.Subjects;

namespace MediaVC
{
    /// <summary>
    /// Stores non-class value into reference type.
    /// </summary>
    /// <typeparam name="T">Non-class value type.</typeparam>
    public sealed class Ref<T> : IRef<T>
        where T : struct
    {
        public Ref(T? initValue = default)
        {
            value = initValue;
            this.changed.OnNext(value);
        }

        private T? value;

        private readonly ISubject<T?> changed = new Subject<T?>();

        public static implicit operator T?(Ref<T> source) => source.value;

        public T? Value
        {
            get => value;
            set
            {
                this.value = value;
                this.changed.OnNext(value);
            }
        }

        public IObservable<T?> Changed => this.changed;
    }
}
