using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Services.SceneLoader
{
    public class SceneLoader
    {
        public void Load(string nextScene, Action onLoaded = null)
        {
            LoadSceneAsync(nextScene, onLoaded).Forget();
        }

        private async UniTaskVoid LoadSceneAsync(string name, Action onLoaded)
        {
            await SceneManager.LoadSceneAsync(name);
            
            onLoaded?.Invoke();
        }
    }
}