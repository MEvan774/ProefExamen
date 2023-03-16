using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Runtime.LevelSystem
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private List<int> levelBuildIndexes;
        [SerializeField] private GameObject[] nonDestroyableObjects;
        [SerializeField] private Image shaderImage;

        private float _timer = 0f;
        private const float _zero = 0f;

        private bool _doTransition;

        private int _currentLevel = -1;

        private CompletableBehaviour[] _completableBehaviours;

        private void Awake()
        {
            shaderImage.material.shader = Shader.Find("Custom/DiamondMask");
            shaderImage.material.SetFloat("_Progress", _zero);

            CurrentLevel = 0;

            for (int i = 0; i < nonDestroyableObjects.Length; i++)
            {
                DontDestroyOnLoad(nonDestroyableObjects[i]);
            }
        }

        private void Update()
        {
            if (_doTransition)
            {
                _timer += Time.deltaTime;
                float progress = Mathf.PingPong(_timer, 1f);
                shaderImage.material.SetFloat("_Progress", progress);
            }
        }

        IEnumerator PlayTransition()
        {
            _doTransition = true;

            yield return new WaitForSeconds(1f);

            //NextLevel();

            yield return new WaitForSeconds(1f);

            _doTransition = false;
            _timer = 0f;
            shaderImage.material.SetFloat("_Progress", _zero);
        }

        public void ForceNextLevel()
        {
            StartCoroutine(PlayTransition());
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
                    operation.completed += (_) => LoadMinigameLevel(newValue);
                }
                else
                {
                    LoadMinigameLevel(value);
                }
            }
        }

        private void LoadMinigameLevel(int newLevelIndex)
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
            StartCoroutine(PlayTransition());
        }
    }
}