using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _Code.Cards
{
    public sealed class CardManager : MonoBehaviour
    {
        [SerializeField] private CardView[] _cards;
        [SerializeField] private CardSOData[] _cardsData;
        
        [SerializeField] private CardPlace[] _cardPlaces;
        [SerializeField] private Transform _myDeckParentTransform;
        [SerializeField] private Transform _pileParentTransform;
        [SerializeField] private PileZone _pileZone;
        [SerializeField] private RectTransform _pileLayout;
        
        private CardView _selectedCard;
        private bool _isPuttingCard;

        public async UniTask Init()
        {
            var i = 0;
            foreach (var card in _cards)
            {
                var cached = i;
                card.gameObject.SetActive(true);
                card.Init(_cardsData[Random.Range(0, _cardsData.Length)]);
                card.StoppedLooking += () =>
                {
                    foreach (var cardPlace in _cardPlaces)
                    {
                        cardPlace.SetCardSelectedState(false);
                    }
                    _pileZone.SetCardSelectedState(false);

                    if (!card.IsInMyDeck)
                    {
                        MoveCardToMyDeck(card).Forget();
                    }
                    else
                    {
                        card.transform.DOMove(_cardPlaces[card.CardPlaceInMyDeck].transform.position, 0.2f).SetEase(Ease.InCubic);
                    }
                    
                    TrySetSelectedToNull().Forget();
                };
                card.StartedLooking += () =>
                {
                    if (card.IsInMyDeck)
                    {
                        _pileZone.SetCardSelectedState(true);
                    }
                    foreach (var cardPlace in _cardPlaces)
                    {
                        cardPlace.SetCardSelectedState(true);
                    }

                    _selectedCard = card;
                };
                i++;
            }

            var index = 0;
            foreach (var cardPlace in _cardPlaces)
            {
                var cached = index;
                cardPlace.CardPlaced += async () =>
                {
                    _isPuttingCard = true;
                    _selectedCard.transform.DOMove(cardPlace.transform.position, 0.2f).SetEase(Ease.InCubic);
                    await _selectedCard.transform.DOScale(_selectedCard.transform.localScale * 0.5f, 0.2f).SetEase(Ease.InCubic);
                    _selectedCard.transform.SetParent(_myDeckParentTransform);
                    _selectedCard.SetIsInMyDeckState(true);
                    _pileZone.ForgetCard();
                    _cardPlaces[_selectedCard.CardPlaceInMyDeck].ForgetCard();
                    _selectedCard.SetCardPlaceInMyDeck(cached);
                    _selectedCard = null;
                    _isPuttingCard = false;
                };
                index++;
            }

            _pileZone.CardPlaced += async () =>
            {
                Debug.Log("!!!");
                _isPuttingCard = true;
                MoveCardToMyDeck(_selectedCard).Forget();
                await _selectedCard.transform.DOScale(_selectedCard.transform.localScale * 0.5f, 0.19f).SetEase(Ease.InCubic);
                _selectedCard.transform.SetParent(_pileParentTransform);
                _selectedCard.SetIsInMyDeckState(false);
                _pileZone.ForgetCard();
                _cardPlaces[_selectedCard.CardPlaceInMyDeck].ForgetCard();
                _selectedCard = null;
                _isPuttingCard = false;
            };
        }

        private async UniTask MoveCardToMyDeck(CardView card)
        {
            await card.transform.DOMove(_pileParentTransform.position, 0.2f).SetEase(Ease.InCubic);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_pileLayout);
        }

        private async UniTask TrySetSelectedToNull()
        {
            await UniTask.DelayFrame(3);
            if (_isPuttingCard)
                return;
            _selectedCard = null; 
        }

        private void Start()
        {
            Init().Forget();
        }
    }
}