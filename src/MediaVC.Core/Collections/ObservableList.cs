using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace MediaVC
{
    public class ObservableList<T> : IObservableEnumerable<T>
    {
        #region Fields

        private readonly IList<T> list = new List<T>();

        private readonly ISubject<T> added = new Subject<T>();
        private readonly ISubject<T> removed = new Subject<T>();

        private readonly ISubject<Unit> cleared = new Subject<Unit>();

        #endregion

        #region Properties

        public IObservable<T> Added => this.added.AsObservable();

        public IObservable<T> Removed => this.removed.AsObservable();

        public IObservable<Unit> Cleared => this.cleared.AsObservable();

        public object Locker { get; set; } = new object();

        public T? this[int index]
        {
            get => this.list[index];
            set => this.list[index] = value!;
        }

        #endregion

        #region Methods

        public void Add(T? item)
        {
            lock(Locker)
            {
                this.list.Add(item!);

                this.added.OnNext(item!);
            }
        }

        public void Remove(T item)
        {
            lock(Locker)
            {
                this.list.Remove(item);

                this.removed.OnNext(item);
            }
        }

        public void RemoveAt(int index)
        {
            lock(Locker)
            {
                var item = this.list[index];
                this.list.Remove(item);

                this.removed.OnNext(item);
            }
        }

        public void Clear()
        {
            lock(Locker)
            {
                this.list.Clear();

                this.cleared.OnNext(Unit.Default);
            }
        }

        public IEnumerator<T> GetEnumerator() => this.list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.list.GetEnumerator();

        #endregion
    }
}
