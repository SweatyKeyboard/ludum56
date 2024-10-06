using System;
using _Code.Characters;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Code.Cards
{
    public sealed class CardView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private TMP_Text _copiesCountText;
        [SerializeField] private GameObject _copiesCounter;
        
        [SerializeField] private TMP_Text _header;
        [SerializeField] private Image _mainIcon;
        [SerializeField] private Image[] _actionIcons;
        [SerializeField] private TMP_Text _description;

        [SerializeField] private Image _shadowTop;
        [SerializeField] private Image _shadowBottom;
        [SerializeField] private Image _shadowLeft;
        [SerializeField] private Image _shadowRight;

        public bool IsInMyDeck { get; private set; }
        public int CardPlaceInMyDeck { get; private set; } = -1;
        public bool CanBeTaken { get; set; } = true;
        public ECardSpecialEffect Special => _data.Special;
        public ActionSOData[] Actions { get; private set; }
        public bool CanBeUsed => _copiesCount > 0;
        public object Character => _data.Data;
        public CardSOData Data => _data;

        private bool _isWatchingCard;
        private CardSOData _data;

        private int _copiesCount = 1;

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
            _data = cardData;
            _header.text = cardData.Header;
            _description.text = cardData.Description;
            _mainIcon.sprite = cardData.Icon;

            for (var i = 0; i < _actionIcons.Length; i++)
            {
                _actionIcons[i].sprite = cardData.Data.Sprites[i];
            }

            Actions = new ActionSOData[3];
            for (var i = 0; i < 3; i++)
            {
                Actions[i] = cardData.Data.Data[i];
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!CanBeTaken)
                return;

            _isWatchingCard = true;
            transform.DOMove(Vector3.zero, 0.2f).SetEase(Ease.InCubic);
            transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InCubic);
            StartedLooking?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!CanBeTaken)
                return;
            
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

        public async UniTask PlayEffect()
        {
            transform.DOShakeScale(0.4f, 0.1f, 1).SetEase(Ease.InCubic);
            await transform.DOJump(transform.position, 0.2f, 1, 2f/3f).SetEase(Ease.InCubic);
        }

        public async UniTask ReplaceAction(int index, ActionSOData action)
        {
            var time = 2f / 3f;
            _actionIcons[index].transform.DOScale(Vector3.zero, time / 2f);
            await UniTask.Delay(TimeSpan.FromSeconds(time / 2f));
            _actionIcons[index].sprite = action.Icon;
            _actionIcons[index].transform.DOScale(Vector3.one, time / 2f);
            await UniTask.Delay(TimeSpan.FromSeconds(time / 2f));
            Actions[index] = action;
        }

        public async UniTask SetCopiesCount(int count)
        {
            _copiesCount = count;
            _copiesCountText.text = "x" + _copiesCount;

            if (count > 0)
            {
                _copiesCounter.SetActive(true);
                await _copiesCounter.transform.DOScale(Vector3.one, 2f / 3f).SetEase(Ease.InCubic);
            }
            else
            {
                await _copiesCounter.transform.DOScale(Vector3.zero, 2f / 3f).SetEase(Ease.InCubic);
                _copiesCounter.SetActive(false);
            }
        }

        public void AddCopy()
        {
            SetCopiesCount(_copiesCount + 1).Forget();
        }
        
        public async UniTask Use()
        {
            if (_copiesCount > 1)
                await SetCopiesCount(_copiesCount - 1);
            else
            {
                _copiesCount--;
                await transform.DOScale(Vector3.zero, 2f / 3f).SetEase(Ease.OutCubic);
            }
        } 
    }
}