using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MediaVC
{
    /// <summary>
    /// Returns <see cref="IAsyncEnumerator{T}"/> that is specified in constructor.
    /// </summary>
    public sealed class AsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        /// <summary>
        /// Initializes new instance of <see cref="AsyncEnumerable{T}"/>.
        /// </summary>
        /// <param name="asyncEnumerator"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public AsyncEnumerable(IAsyncEnumerator<T> asyncEnumerator) =>
            this.enumerator = asyncEnumerator ?? throw new ArgumentNullException(nameof(asyncEnumerator));

        private readonly IAsyncEnumerator<T> enumerator;

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => enumerator.WithCancellation(cancellationToken);
    }
}
