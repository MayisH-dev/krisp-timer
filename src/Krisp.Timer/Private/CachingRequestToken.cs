using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Krisp.Timer.Private
{
    internal sealed class CachingRequestToken : RequestToken
    {
        private ConcurrentDictionary<CachingRequestToken, Unit>? _cache;

        public CachingRequestToken(ConcurrentDictionary<CachingRequestToken, Unit> cache)
        {
            _cache = cache;
        }

        private protected override void OnCancel()
        {
            _cache.TryRemove(this, out Unit _);
        }

        private protected override void OnDispose()
        {
            _cache.TryRemove(this, out Unit _);
            _cache = null!;
        }

        private protected override void OnSchedule()
        {
            _cache.TryAdd(this, Unit.Value);
        }
    }
}