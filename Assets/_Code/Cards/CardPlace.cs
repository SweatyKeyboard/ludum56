using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Code.Cards
{
    public sealed class CardPlace : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _text;
        
        private readonly float _deselectedAlpha = 0.5f;
        private readonly float _selectedAlpha = 1f;
        
        private readonly Color _colorFaded = new Color(0.8f, 0.8f, 0.8f, 0.8f); 
        private readonly Color _colorUnfaded = Color.white;
        
        private bool _isCardSelected;
        private bool _isEntered;
        public bool IsUsed { get; private set; }

        public event Action CardPlaced;
        public void SetCardSelectedState(bool isCardSelected)
        {
            _isCardSelected = isCardSelected;

            if (!isCardSelected)
            {
                _image.DOFade(_deselectedAlpha, 0.1f).SetEase(Ease.InSine);
                _text.DOColor(_colorUnfaded, 0.1f).SetEase(Ease.InSine);
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (!_isEntered || IsUsed)
                    return;
            
                CardPlaced?.Invoke();
                IsUsed = true;         
                _isEntered = false;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_isCardSelected || IsUsed)
                return;
            
            _isEntered = true;
            _image.DOFade(_selectedAlpha, 0.1f).SetEase(Ease.InSine);
            _text.DOColor(_colorFaded, 0.1f).SetEase(Ease.InSine);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_isCardSelected || IsUsed)
                return;
            
            _isEntered = false;
            _image.DOFade(_deselectedAlpha, 0.1f).SetEase(Ease.InSine);
            _text.DOColor(_colorUnfaded, 0.1f).SetEase(Ease.InSine);
        }

        public void ForgetCard()
        {
            IsUsed = false;
        }

        public void Reinit()
        {
            _isCardSelected = false;
            IsUsed = false;
        }
    }
}