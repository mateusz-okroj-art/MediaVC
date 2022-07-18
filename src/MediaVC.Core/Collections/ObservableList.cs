using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace MediaVC
{
    /// <summary>
    /// Thread-safe, observable list
    /// </summary>
    /// <typeparam name="T">Items type</typeparam>
    public class ObservableList<T> : IObservableEnumerable<T>, ICollection<T>
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

        public int Count => this.list.Count;

        public bool IsReadOnly => false;

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

        public bool Remove(T item)
        {
            bool result;

            lock(Locker)
            {
                result = this.list.Remove(item);

                this.removed.OnNext(item);
            }

            return result;
        }

        /// <summary>
        /// Removes item in selected position.
        /// </summary>
        /// <param name="index"></param>
        /// <returns><see langword="true"/>, if is removed successfully.</returns>
        public bool RemoveAt(int index)
        {
            bool result;

            lock(Locker)
            {
                var item = this.list[index];
                result = this.list.Remove(item);

                this.removed.OnNext(item);
            }

            return result;
        }

        public bool Contains(T item) => this.list.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => this.list.CopyTo(array, arrayIndex);

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
