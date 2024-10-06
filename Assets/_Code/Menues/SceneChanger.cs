using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Code.Menues
{
    public sealed class SceneChanger : MonoBehaviour
    {
        public void GoToMainMenu()
        {
            SceneManager.LoadScene(0);
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void NextLevel()
        {
            var currentScene = SceneManager.GetActiveScene().buildIndex;
            
            var sceneIndex = currentScene + 1; 
            if (SceneUtility.GetScenePathByBuildIndex(sceneIndex).Length > 0)
                SceneManager.LoadScene(sceneIndex);
            else
                SceneManager.LoadScene(0);
        }
    }
}