using System;
using System.Diagnostics.CodeAnalysis;

namespace Krisp.Timer.Private
{
    /// <summary>
    /// Defines operations that guard against common invalid arguments
    /// </summary>
    internal static class Throw
    {
        internal static void WhenDisposed([DoesNotReturnIf(true)] bool isDisposed, string objectName)
        {
            if (isDisposed)
                throw new ObjectDisposedException(objectName);
        }

        internal static void WhenNegative(int number,  string argumentName)
        {
            if(number < 0)
                throw new ArgumentOutOfRangeException(argumentName);
        }

        internal static void WhenNull(object? value, string argumentName)
        {
            if(value is null)
                throw new ArgumentNullException(argumentName);
        }
    }
}