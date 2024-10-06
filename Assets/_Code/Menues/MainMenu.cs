using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Code.Menues
{
    public sealed class MainMenu : MonoBehaviour
    {
        [SerializeField] private RectTransform _levelSelector;

        public void ShowLevelSelector()
        {
            _levelSelector.DOAnchorPos(new Vector3(0f, -22f, 0f), 1f / 3f).SetEase(Ease.InSine);
        }

        public void HideLevelSelector()
        {
            _levelSelector.DOAnchorPos(new Vector3(0f, -622f, 0f), 1f / 3f).SetEase(Ease.OutSine);
        }

        public void GoToLevel(int level)
        {
            SceneManager.LoadScene("Level"+level);
        }
    }
}