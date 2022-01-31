using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MediaVC.Readers;

namespace MediaVC.Enumerators
{
    internal class StringReaderEnumerator : IAsyncEnumerator<string>
    {
        public StringReaderEnumerator(StringReader reader)
        {
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));

            if(this.reader.source.Length > 0)
                this.reader.source.Position = 0;
        }

        private readonly StringReader reader;

        public string Current { get; private set; }

        public async ValueTask DisposeAsync() => await Task.CompletedTask;

        public async ValueTask<bool> MoveNextAsync()
        {
            var result = await this.reader.ReadLineAsync();

            Current = result!;

            return result is not null;
        }
    }
}
