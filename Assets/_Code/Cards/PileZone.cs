using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Code.Cards
{
    public sealed class PileZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TMP_Text _text;
        
        private readonly Color _colorFaded = new Color(0.8f, 0.8f, 0.8f, 0.8f); 
        private readonly Color _colorUnfaded = Color.white;
        
        private bool _isCardSelected;
        private bool _isEntered;
        private bool _isUsed;

        public event Action CardPlaced;
        public void SetCardSelectedState(bool isCardSelected)
        {
            _isCardSelected = isCardSelected;

            if (!isCardSelected)
            {
                _text.DOColor(_colorUnfaded, 0.1f).SetEase(Ease.InSine);
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (!_isEntered || _isUsed)
                    return;
            
                CardPlaced?.Invoke();
                _isUsed = true;        
                _isEntered = false;
            }
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_isCardSelected)
                return;
            
            _isEntered = true;
            _text.DOColor(_colorFaded, 0.1f).SetEase(Ease.InSine);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_isCardSelected)
                return;
            
            _isEntered = false;
            _text.DOColor(_colorUnfaded, 0.1f).SetEase(Ease.InSine);
        }

        public void ForgetCard()
        {
            _isUsed = false;
        }
    }
}