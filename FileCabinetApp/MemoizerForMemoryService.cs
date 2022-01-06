using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Class that holds memoized method for find purpuses in memory service.
    /// </summary>
    internal static class MemoizerForMemoryService
    {
        /// <summary>
        /// Holds function results.
        /// </summary>
        private static ConcurrentDictionary<string, object> cache = new ();

        /// <summary>
        /// Memoize function results.
        /// </summary>
        /// <typeparam name="T1">Parameter to search.</typeparam>
        /// <typeparam name="TResult">Function result.</typeparam>
        /// <param name="context">Caller's context.</param>
        /// <param name="arg">Key value in dictionary.</param>
        /// <param name="f">Function which result should be stored in dictionary.</param>
        /// <param name="cacheKey">Key value for kepping in dictionary by caller name.</param>
        /// <returns>Expected output.</returns>
        /// <exception cref="ArgumentNullException">Throws when contex or caller's name is null.</exception>
        internal static TResult Memoized<T1, TResult>(
            this object context,
            T1 arg,
            Func<T1, TResult> f,
            [CallerMemberName] string? cacheKey = null)
            where T1 : notnull
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (cacheKey == null)
            {
                throw new ArgumentNullException(nameof(cacheKey));
            }

            var methodCache = (ConcurrentDictionary<T1, TResult>)cache.GetOrAdd(cacheKey, _ => new ConcurrentDictionary<T1, TResult>());

            return methodCache.GetOrAdd(arg, f);
        }

        /// <summary>
        /// Clears cache.
        /// </summary>
        internal static void RefreshMemoizer()
        {
            cache.Clear();
            Console.WriteLine("Memoizer has been refreshed.");
        }
    }
}
