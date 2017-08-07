using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace lib
{
    public static class MoreExtensions
    {

        public static string ToShortUpperLetters(this string s)
        {
            return string.Join("", s.Where(char.IsUpper).ToArray());
        }
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items, Random random)
        {
            var copy = items.ToList();
            for (var i = 0; i < copy.Count; i++)
            {
                var nextIndex = random.Next(i, copy.Count);
                yield return copy[nextIndex];
                copy[nextIndex] = copy[i];
            }
        }

        public static void ShuffleInPlace<T>(this IList<T> items, Random random)
        {
            for (var i = 0; i < items.Count; i++)
            {
                var nextIndex = random.Next(i, items.Count);
                items[nextIndex] = items[i];
            }
        }

        public static int IndexOf<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            var i = 0;
            foreach (var item in items)
            {
                if (predicate(item)) return i;
                i++;
            }
            return -1;
        }

        public static int BoundTo(this int v, int left, int right)
        {
            if (v < left) return left;
            if (v > right) return right;
            return v;
        }

        public static bool InRange(this int v, int min, int max)
        {
            return v >= min && v <= max;
        }

        public static int IndexOf<T>(this IReadOnlyList<T> readOnlyList, T value)
        {
            var count = readOnlyList.Count;
            var equalityComparer = EqualityComparer<T>.Default;
            for (var i = 0; i < count; i++)
            {
                var current = readOnlyList[i];
                if (equalityComparer.Equals(current, value)) return i;
            }
            return -1;
        }

        public static TV GetOrCreate<TK, TV>(this IDictionary<TK, TV> d, TK key, Func<TK, TV> create)
        {
            return d.TryGetValue(key, out TV v)
                ? v
                : (d[key] = create(key));
        }

        public static TV GetOrDefault<TK, TV>(this IDictionary<TK, TV> d, TK key, TV defaultValue)
        {
            return d.TryGetValue(key, out TV v)
                ? v
                : (d[key] = defaultValue);
        }

        public static TV GetOrDefaultNoSideEffects<TK, TV>(this IDictionary<TK, TV> d, TK key, TV defaultValue)
        {
            return d.TryGetValue(key, out TV v)
                ? v
                : defaultValue;
        }

        public static void Replace<TKey, TValue>(
            this Dictionary<TKey, TValue> d, TKey key, Func<TValue, TValue> replacer)
            where TValue : new()
        {
            if (!d.ContainsKey(key)) d[key] = new TValue();
            d[key] = replacer(d[key]);
        }

        public static void Replace<TKey, TValue>(
            this Dictionary<TKey, TValue> d, TKey key, Func<TValue, TValue> replacer, TValue defaultValue)
        {
            if (!d.ContainsKey(key)) d[key] = defaultValue;
            d[key] = replacer(d[key]);
        }

        public static IList<T> MaxListBy<T>(this IEnumerable<T> items, Func<T, double> getKey)
        {
            IList<T> result = null;
            var bestKey = double.MinValue;
            foreach (var item in items)
            {
                var itemKey = getKey(item);
                if (result == null || bestKey < itemKey)
                {
                    result = new List<T> { item };
                    bestKey = itemKey;
                }
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                else if (bestKey == itemKey)
                {
                    result.Add(item);
                }
            }
            return result ?? new T[0];
        }


        public static int ElementwiseHashcode<T>(this IEnumerable<T> items)
        {
            unchecked
            {
                return items.Select(t => t.GetHashCode()).Aggregate((res, next) => (res * 379) ^ next);
            }
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> en)
        {
            var set = new HashSet<T>();
            foreach (var e in en)
                set.Add(e);
            return set;
        }

        public static string CalculateMd5(this string arg)
        {
            byte[] hash;
            using (var md5 = MD5.Create())
            {
                hash = md5.ComputeHash(Encoding.Default.GetBytes(arg));
            }

            var sb = new StringBuilder();
            foreach (var b in hash)
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }

        public static string ReadAllText(this Stream inputStream)
        {
            var codeBytes = new MemoryStream();
            inputStream.CopyTo(codeBytes);
            return Encoding.UTF8.GetString(codeBytes.ToArray());
        }

        public static bool IsOneOf<T>(this T o, params T[] validValues)
        {
            return validValues.Contains(o);
        }

        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue v;
            if (dictionary.TryGetValue(key, out v)) return v;
            throw new KeyNotFoundException("Key " + key + " not found in dictionary");
        }
    }
}