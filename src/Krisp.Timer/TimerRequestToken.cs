using System.Threading;

namespace Krisp.Timer
{
    /// <summary>
    /// An opaque type representing an identifier of a scheduled operation
    /// </summary>
    public sealed class TimerRequestToken
    {
        internal TimerRequestToken(CancellationTokenSource cancellationTokenSource)
        {
            CancellationTokenSource = cancellationTokenSource;
        }

        internal CancellationTokenSource CancellationTokenSource { get; }
    }
}