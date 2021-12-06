using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaVC.Enumerators
{
    internal class InputSourceEnumerator : IAsyncEnumerator<byte>
    {
        public byte Current { get; }

        public ValueTask DisposeAsync() => throw new NotImplementedException();
        public ValueTask<bool> MoveNextAsync() => throw new NotImplementedException();
    }
}
