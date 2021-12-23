using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediaVC.Difference;

namespace MediaVC.Enumerators
{
    internal class StringReaderEnumerator : IAsyncEnumerator<string>
    {
        public StringReaderEnumerator(IStringReader reader)
        {
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        private readonly IStringReader reader;
        private long position = -1;

        public string Current { get; }

        public async ValueTask DisposeAsync() => await Task.CompletedTask;

        public async ValueTask<bool> MoveNextAsync()
        {
            
        }
    }
}
