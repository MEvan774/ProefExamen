using System;
using UnityEngine;
using UnityEngine.Events;

    public abstract class CompletableBehaviour : MonoBehaviour
    {
        private bool _isCompleted;

        public UnityEvent onComplete = new UnityEvent();

        private void OnDestroy()
        {
            onComplete?.RemoveAllListeners();
        }

        public virtual void Complete()
        {
            _isCompleted = true;
            onComplete?.Invoke();
        }

        public bool IsCompleted => _isCompleted;
    }