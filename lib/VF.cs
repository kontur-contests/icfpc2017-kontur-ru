using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace lib
{
    public struct VF : IEquatable<VF>, IFormattable
    {
        private const double eps = 1e-7;
        public readonly double X, Y;

        public VF(double x, double y)
        {
            X = x;
            Y = y;
        }

        public VF(PointF point) : this(point.X, point.Y)
        {
        }

        public static VF Parse(string s)
        {
            var parts = s.Split(',');
            if (parts.Length != 2) throw new FormatException(s);
            return new VF(double.Parse(parts[0]), double.Parse(parts[1]));
        }

        #region value semantics

        [Pure]
        public bool Equals(VF other)
        {
            return Math.Abs(X - other.X) < eps && Math.Abs(Y - other.Y) < eps;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is VF && Equals((V) obj);
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

        public static implicit operator VF(string s) => Parse(s);

        public PointF ToPointF => new PointF((float) X, (float) Y);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VF operator -(VF a, VF b) => new VF(a.X - b.X, a.Y - b.Y);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VF operator -(VF a) => new VF(-a.X, -a.Y);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VF operator +(VF a, VF b) => new VF(a.X + b.X, a.Y + b.Y);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VF operator *(VF a, double k) => new VF(a.X * k, a.Y * k);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VF operator /(VF a, double k) => new VF(a.X / k, a.Y / k);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VF operator *(double k, VF a) => new VF(a.X * k, a.Y * k);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ScalarProd(VF p2) => X * p2.X + Y * p2.Y;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double VectorProdLength(VF p2) => X * p2.Y - p2.X * Y;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VF Translate(double shiftX, double shiftY) => new VF(X + shiftX, Y + shiftY);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VF Rotate90CW() => new VF(Y, -X);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VF Rotate90CCW() => new VF(-Y, X);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Length() => Math.Sqrt(X * X + Y * Y);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double LengthSquared() => X * X + Y * Y;
    }
}