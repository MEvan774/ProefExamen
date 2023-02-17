using UnityEngine;

namespace Runtime.LevelSystem
{
    /// <summary>
    /// Can be used if you want to use 1 completable for multiple states.
    /// requires to be completed the set amount of times before actually completing.
    /// </summary>
    public class MultiCompletable : SimpleCompletable
    {
        [SerializeField] private int requiredCompletionCount = 1;

        private int _timesCompleted = 0;

        public override void Complete()
        {
            _timesCompleted += 1;
            if (_timesCompleted < requiredCompletionCount) return;
            base.Complete();
        }
    }
}