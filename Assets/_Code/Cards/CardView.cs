using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

namespace _Code.Cards
{
    public sealed class CardView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private TMP_Text _header;
        [SerializeField] private Image _mainIcon;
        [SerializeField] private Image[] _actionIcons;
        [SerializeField] private TMP_Text _description;

        [SerializeField] private Image _shadowTop;
        [SerializeField] private Image _shadowBottom;
        [SerializeField] private Image _shadowLeft;
        [SerializeField] private Image _shadowRight;

        public bool IsInMyDeck { get; private set; }
        public int CardPlaceInMyDeck { get; private set; }

        private bool _isWatchingCard;


        public event Action StartedLooking;
        public event Action StoppedLooking;
        private void Update()
        {
            if (_isWatchingCard)
            {
                var mousePos = Input.mousePosition;
                mousePos = new Vector3(mousePos.x / Screen.width - 0.5f, mousePos.y / Screen.height - 0.5f, 0);
                transform.eulerAngles = new Vector3(-mousePos.y, mousePos.x, 0f) * 30f;

                var colorTop = (1 - (mousePos.y + 0.5f)) / 2f;
                var colorBottom = (mousePos.y + 0.5f) / 2f;
                var colorLeft = (mousePos.x + 0.5f) / 2f;
                var colorRight = (1 - (mousePos.x + 0.5f)) / 2f;
                _shadowTop.color = new Color(colorTop, colorTop, colorTop, 0.5f);
                _shadowBottom.color = new Color(colorBottom, colorBottom, colorBottom,0.5f );
                _shadowLeft.color = new Color(colorLeft,colorLeft, colorLeft, 0.5f);
                _shadowRight.color = new Color(colorRight, colorRight, colorRight,0.5f );
            }
        }

        public void Init(CardSOData cardData)
        {
            _header.text = cardData.Header;
            _description.text = cardData.Description;
            _mainIcon.sprite = cardData.Icon;

            for (var i = 0; i < _actionIcons.Length; i++)
            {
                _actionIcons[i].sprite = cardData.Data.Sprites[i];
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isWatchingCard = true;
            transform.DOMove(Vector3.zero, 0.2f).SetEase(Ease.InCubic);
            transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InCubic);
            StartedLooking?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isWatchingCard = false;
            transform.DOScale(Vector3.one * 0.5f, 0.2f).SetEase(Ease.InCubic);
            transform.DORotate(Vector3.zero, 0.2f).SetEase(Ease.InCubic);
            _shadowTop.color = Color.clear;
            _shadowBottom.color = Color.clear;
            _shadowLeft.color = Color.clear;
            _shadowRight.color = Color.clear;
            StoppedLooking?.Invoke();
        }

        public void SetIsInMyDeckState(bool isInDeck)
        {
            IsInMyDeck = isInDeck;
        }

        public void SetCardPlaceInMyDeck(int place)
        {
            CardPlaceInMyDeck = place;
        }
    }
}