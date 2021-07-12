using System;

namespace Krisp.Timer.Private
{
    /// <summary>
    /// A type with a single possible value
    /// </summary>
    /// <remarks>Used as a workaround for using ConcurrentDictionary as a Set</remarks>
    internal sealed record Unit : IComparable<Unit>
    {
        private Unit() {}
        internal static Unit Value = new();

        public int CompareTo(Unit? other)
        {
            Throw.WhenNull(other, nameof(other));
            return 0;
        }
    }
}