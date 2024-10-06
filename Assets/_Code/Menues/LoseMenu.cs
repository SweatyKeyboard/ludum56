using DG.Tweening;
using UnityEngine;

namespace _Code.Menues
{
    public sealed class LoseMenu : MonoBehaviour
    {
        [SerializeField] private Transform _mainTransform;

        public void Show()
        {
            _mainTransform.gameObject.SetActive(true);
            _mainTransform.localScale = Vector3.zero;
            _mainTransform.DOScale(Vector3.one, 2f / 3f).SetEase(Ease.InCubic);
        }
    }
}