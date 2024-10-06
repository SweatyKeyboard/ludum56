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
            try
            {
                SceneManager.LoadScene(currentScene + 1);
            }
            catch
            {
                SceneManager.LoadScene(0);
            }
        }
    }
}