using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MediaVC
{
    internal sealed class TrackingFilesList : IList<string>, IDisposable
    {
        public TrackingFilesList(Stream listMemory)
        {
            ArgumentNullException.ThrowIfNull(listMemory);

            if(!listMemory.CanWrite || !listMemory.CanRead)
                throw new ArgumentException("Current stream is not usable.");

            memory = listMemory;
            reader = new StreamReader(listMemory);
            writer = new StreamWriter(listMemory);
        }

        private readonly Stream memory;
        private readonly StreamReader reader;
        private readonly StreamWriter writer;
        private readonly object locker = new();

        public string this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int Count { get; }

        public bool IsReadOnly { get; }

        public void Add(string item) => throw new System.NotImplementedException();

        public void Clear() => throw new System.NotImplementedException();

        public bool Contains(string item) => throw new System.NotImplementedException();

        public void CopyTo(string[] array, int arrayIndex) => throw new System.NotImplementedException();
        
        public IEnumerator<string> GetEnumerator() => throw new System.NotImplementedException();

        public int IndexOf(string item) => throw new System.NotImplementedException();

        public void Insert(int index, string item) => throw new System.NotImplementedException();

        public bool Remove(string item) => throw new System.NotImplementedException();

        public void RemoveAt(int index) => throw new System.NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator() => throw new System.NotImplementedException();

        public void Dispose()
        {
            this.reader.Dispose();
            this.writer.Dispose();
        }
    }
}
