using UnityEngine;

namespace DG.Tweening
{
    public readonly struct TweenStreamingData
    {
        public readonly Object target;
        public readonly int tweenId;

        private static int _currTweenId = 1;
        private static object tweenIdLock = new object();
        private static int GetNextStreamingId()
        {
            lock (tweenIdLock)
            {
                return _currTweenId++;
            }
        }

        public TweenStreamingData(Object target)
        {
            // if (!target) throw new System.ArgumentNullException(nameof(target));
            this.target = target;
            tweenId = GetNextStreamingId();
        }

        public bool HasValue => target;
    }
}