using System.Collections.Concurrent;

namespace Krisp.Timer.Private
{
    internal sealed class CachingRequestToken : RequestToken
    {
        private readonly ConcurrentDictionary<CachingRequestToken, Unit> _cache;

        public CachingRequestToken(ConcurrentDictionary<CachingRequestToken, Unit> cache)
        {
            _cache = cache;
        }

        private protected override void OnCancel()
        {
            _cache.TryRemove(this, out var _);
        }

        private protected override void OnDispose()
        {
            _cache.TryRemove(this, out var _);
        }

        private protected override void OnSchedule()
        {
            _cache.TryAdd(this, Unit.Value);
        }
    }
}