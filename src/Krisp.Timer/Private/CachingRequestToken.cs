using System.Collections.Concurrent;

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
            // OnCancel is protected against disposed state
            // Can safely ignore this warning for now
            _cache.TryRemove(this, out Unit _);
        }

        private protected override void OnDispose()
        {
            // OnDispose is protected against disposed state
            // Can safely ignore this warning for now
            _cache.TryRemove(this, out Unit _);
            _cache = null!;
        }

        private protected override void OnSchedule()
        {
            // OnSchedule is protected against disposed state
            // Can safely ignore this warning for now
            _cache.TryAdd(this, Unit.Value);
        }
    }
}