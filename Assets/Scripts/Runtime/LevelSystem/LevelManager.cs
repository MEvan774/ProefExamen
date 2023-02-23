using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.LevelSystem
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private List<int> levelBuildIndexes;
        [SerializeField] private GameObject[] nonDestoyableObjects;

        private int _currentLevel = -1;
        private CompletableBehaviour[] _completableBehaviours;

        private void Awake()
        {
            CurrentLevel = 0;

            for (int i = 0; i < nonDestoyableObjects.Length; i++)
            {
                DontDestroyOnLoad(nonDestoyableObjects[i]);
            }
        }

        public void ForceNextLevel()
        {
            NextLevel();
        }

        private void NextLevel()
        {
            CurrentLevel += 1;
        }

        public int CurrentLevel
        {
            get => _currentLevel;
            set
            {
                if (_currentLevel != -1)
                {
                    AsyncOperation operation = SceneManager.UnloadSceneAsync(levelBuildIndexes[_currentLevel]);
                    var newValue = value;
                    operation.completed += (_) => LoadNextLevel(newValue);
                }
                else
                {
                    LoadNextLevel(value);
                }
            }
        }

        private void LoadNextLevel(int newLevelIndex)
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(levelBuildIndexes[newLevelIndex], LoadSceneMode.Additive);
            async.completed += OnLevelLoaded;
            _currentLevel = newLevelIndex;
        }

        private void OnLevelLoaded(AsyncOperation operation)
        {
            _completableBehaviours = FindObjectsOfType<CompletableBehaviour>(true);

            foreach (var completableBehaviour in _completableBehaviours)
            {
                completableBehaviour.onComplete?.AddListener(HandleCompletableComplete);
            }
        }

        private void HandleCompletableComplete()
        {
            if (_completableBehaviours.Any(behaviour => !behaviour.IsCompleted)) return;

            NextLevel();
        }
    }
}
