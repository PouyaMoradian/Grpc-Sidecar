using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Hades.Core
{
    public static class EnumerableExtensions
    {
        public static void Foreach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            _ = collection
                ?? throw new ArgumentNullException(nameof(collection));
            _ = action
                ?? throw new ArgumentNullException(nameof(action));

            foreach (var item in collection)
            {
                action(item);
            }
        }

        public static void Foreach<T>(this IEnumerable<T> collection, Action<int, T> action)
        {
            _ = collection
                   ?? throw new ArgumentNullException(nameof(collection));
            _ = action
                ?? throw new ArgumentNullException(nameof(action));

            var i = 0;
            foreach (var item in collection)
            {
                action(i, item);
                i++;
            }
        }

        public async static IAsyncEnumerable<T> ReadAllAsync<T>(this IAsyncStreamReader<T> streamReader, [EnumeratorCancellation]CancellationToken cancellationToken = default)
        {
            if (streamReader == null)
            {
                throw new System.ArgumentNullException(nameof(streamReader));
            }

            while (await streamReader.MoveNext(cancellationToken))
            {
                yield return streamReader.Current;
            }
        }
    }
}
