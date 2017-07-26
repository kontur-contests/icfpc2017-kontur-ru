using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace lib
{
    public struct V : IEquatable<V>, IFormattable
    {
        public readonly int X, Y;

        public V(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static V Parse(string s)
        {
            var parts = s.Split(',');
            if (parts.Length != 2) throw new FormatException(s);
            return new V(int.Parse(parts[0]), int.Parse(parts[1]));
        }

        #region value semantics

        [Pure]
        public bool Equals(V other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is V && Equals((V) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"{X.ToString(format, formatProvider)},{Y.ToString(format, formatProvider)}";
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }

        #endregion

        public static implicit operator V(string s) => Parse(s);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static V operator -(V a, V b) => new V(a.X - b.X, a.Y - b.Y);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static V operator -(V a) => new V(-a.X, -a.Y);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static V operator +(V a, V b) => new V(a.X + b.X, a.Y + b.Y);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static V operator *(V a, int k) => new V(a.X * k, a.Y * k);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static V operator /(V a, int k) => new V(a.X / k, a.Y / k);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static V operator *(int k, V a) => new V(a.X * k, a.Y * k);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ScalarProd(V p2) => X * p2.X + Y * p2.Y;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int VectorProdLength(V p2) => X * p2.Y - p2.X * Y;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public V Translate(int shiftX, int shiftY) => new V(X + shiftX, Y + shiftY);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public V Rotate90CW() => new V(Y, -X);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public V Rotate90CCW() => new V(-Y, X);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Length() => Math.Sqrt(X * X + Y * Y);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long LengthSquared() => X * X + Y * Y;
    }
}