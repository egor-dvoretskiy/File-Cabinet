using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.ServiceTools
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

        /// <summary>
        /// Forming string for memoizing.
        /// </summary>
        /// <param name="phrase">Init phrase.</param>
        /// <returns>Returns string for memoization.</returns>
        internal static string FormIdentificatorForMemoizing(string phrase)
        {
            string[] phrases = phrase.Split(" or ", StringSplitOptions.TrimEntries);
            StringBuilder[] stringForMemoizer = new StringBuilder[phrases.Length];

            stringForMemoizer.InitBuilderArray();
            stringForMemoizer.InsertParameter("FIRSTNAME", phrases);
            stringForMemoizer.InsertParameter("LASTNAME", phrases);
            stringForMemoizer.InsertParameter("DATEOFBIRTH", phrases);
            stringForMemoizer.InsertParameter("PERSONALRATING", phrases);
            stringForMemoizer.InsertParameter("SALARY", phrases);
            stringForMemoizer.InsertParameter("GENDER", phrases);
            stringForMemoizer.IsValid(phrases);

            string identificator = FillIdentificator(stringForMemoizer);

            return identificator;
        }

        private static void IsValid(this StringBuilder[] stringBuilders, string[] phrases)
        {
            for (int i = 0; i < stringBuilders.Length; i++)
            {
                if (string.IsNullOrEmpty(stringBuilders[i].ToString()))
                {
                    throw new ArgumentException($"Wrong parameter name in '{phrases[i]}'");
                }
            }
        }

        private static void InitBuilderArray(this StringBuilder[] stringBuilders)
        {
            for (int i = 0; i < stringBuilders.Length; i++)
            {
                stringBuilders[i] = new StringBuilder();
            }
        }

        private static void InsertParameter(this StringBuilder[] stringBuilder, string name, params string[] phrases)
        {
            for (int i = 0; i < phrases.Length; i++)
            {
                StringBuilder substringBuilder = new StringBuilder();
                string phrase = phrases[i];
                string[] subphrases = phrase.Split(' ');

                for (int j = 0; j < subphrases.Length; j++)
                {
                    string subphrase = subphrases[j];

                    if (!subphrase.Contains(name, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    int startIndex = subphrase.IndexOf('=') + 1;
                    int endIndex = subphrase.IndexOf(' ') == -1 ? subphrase.Length : subphrase.IndexOf(' ');

                    string parameter = subphrase[startIndex..endIndex].Trim('\'').ToUpper();

                    substringBuilder.Append(name.ToUpper());
                    substringBuilder.Append(parameter);

                    break;
                }

                stringBuilder[i].Append(substringBuilder.ToString());
            }
        }

        private static string FillIdentificator(StringBuilder[] stringBuilders)
        {
            var len = stringBuilders.Length;

            for (var i = 1; i < len; i++)
            {
                for (var j = 0; j < len - i; j++)
                {
                    if (string.Compare(stringBuilders[j].ToString(), stringBuilders[j + 1].ToString()) == 1)
                    {
                        Swap(ref stringBuilders[j], ref stringBuilders[j + 1]);
                    }
                }
            }

            return string.Join("OR", stringBuilders.GetStringArrayFromStingBuilder());
        }

        private static string[] GetStringArrayFromStingBuilder(this StringBuilder[] stringBuilders)
        {
            string[] stringArray = new string[stringBuilders.Length];

            for (int i = 0; i < stringBuilders.Length; i++)
            {
                stringArray[i] = stringBuilders[i].ToString();
            }

            return stringArray;
        }

        private static void Swap(ref StringBuilder s1, ref StringBuilder s2)
        {
            var temp = s1;
            s1 = s2;
            s2 = temp;
        }
    }
}
