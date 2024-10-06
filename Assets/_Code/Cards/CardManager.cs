using System;
using System.Linq;
using System.Threading;
using _Code.Characters;
using _Code.Menues;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        [SerializeField] private Button _playButton;
        [SerializeField] private TMP_Text _turnsText;
        [SerializeField] private int _maxTurns = 3;

        [SerializeField] private ActionSOData[] _actionsList;

        [SerializeField] private CharactersManager _charactersManager;
        [SerializeField] private LoseMenu _loseWindow;
        [SerializeField] private Button _turnsButton;
        
        private CardView _selectedCard;
        private bool _isPuttingCard;
        private CardView[] _activeCards;
        private readonly CancellationTokenSource _cancellationToken = new();
        private int _turns = 1;

        public async UniTask Init()
        {
            InitReinitable();

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
                    if (_selectedCard.CardPlaceInMyDeck >= 0)
                        _cardPlaces[_selectedCard.CardPlaceInMyDeck].ForgetCard();
                    _selectedCard.SetCardPlaceInMyDeck(cached);
                    _selectedCard = null;
                    _isPuttingCard = false;
                    CheckMyDeck().Forget();
                };
                index++;
            }

            _pileZone.CardPlaced += async () =>
            {
                _isPuttingCard = true;
                MoveCardToMyDeck(_selectedCard).Forget();
                await _selectedCard.transform.DOScale(_selectedCard.transform.localScale * 0.5f, 0.19f).SetEase(Ease.InCubic);
                _selectedCard.transform.SetParent(_pileParentTransform);
                _selectedCard.SetIsInMyDeckState(false);
                _pileZone.ForgetCard();
                _cardPlaces[_selectedCard.CardPlaceInMyDeck].ForgetCard();
                _selectedCard.SetCardPlaceInMyDeck(-1);
                _selectedCard = null;
                _isPuttingCard = false;
                CheckMyDeck().Forget();
            };
        }

        private void InitReinitable()
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
        }

        private async UniTask CheckMyDeck()
        {
            if (_cardPlaces.All(x => x.IsUsed))
            {
                if (!_playButton.gameObject.activeSelf)
                {
                    _playButton.gameObject.SetActive(true);
                    _playButton.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InCubic);
                }
            }
            else
            {
                if (_playButton.gameObject.activeSelf)
                {
                    await _playButton.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InCubic);
                    _playButton.gameObject.SetActive(false);
                }
            }
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

        public void RunGame()
        {
            foreach (var card in _cards)
            {
                card.CanBeTaken = false;
            }

            UpdateTurnsText();
            RunGameAsync().Forget();
        }

        private async UniTask RunGameAsync()
        {
            await _playButton.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InCubic);
            _playButton.gameObject.SetActive(false);
            
            _activeCards = _cards.Where(x => x.CardPlaceInMyDeck != -1).OrderBy(x => x.CardPlaceInMyDeck).ToArray();
            foreach (var card in _activeCards)
            {
                ApplyCardEffect(card.Special, card.CardPlaceInMyDeck);
                await card.PlayEffect();
                await UniTask.Delay(TimeSpan.FromSeconds(1f/3f));
            }

            await UniTask.Delay(TimeSpan.FromSeconds(2f/3f));
            foreach (var card in _activeCards)
            {
                while (card.CanBeUsed)
                {
                    card.Use().Forget();
                    await _charactersManager.SpawnNewCharacter(card.Data, card.Actions).AttachExternalCancellation(_cancellationToken.Token);
                }
            }

            Reinit();
        }

        private void Reinit()
        {
            _turns++;
            if (_turns > _maxTurns)
            {
                _loseWindow.Show();
                _playButton.gameObject.SetActive(false);
                _turnsButton.gameObject.SetActive(false);
                foreach (var card in _cards)
                {
                    card.Deactivate();
                }
                return;
            }
            
            InitReinitable();
            foreach (var card in _cards)
            {
                card.Reinit();
                card.transform.SetParent(_pileParentTransform);
            }

            foreach (var place in _cardPlaces)
            {
                place.Reinit();
            }

            UpdateTurnsText();
        }

        private void UpdateTurnsText()
        {
            _turnsText.text = $"Turn: {_turns}/{_maxTurns}";
        }

        private void ApplyCardEffect(ECardSpecialEffect cardSpecial, int order)
        {
            switch (cardSpecial)
            {
                case ECardSpecialEffect.Professor:
                    if (order == 1)
                    {
                        var leftMiddle = _activeCards[0].Actions[1];
                        _activeCards[2].ReplaceAction(1, leftMiddle).Forget();
                    }
                    break;
                
                case ECardSpecialEffect.Railroader:
                    if (order < 2)
                    {
                        _activeCards[order + 1].ReplaceAction(1, _actionsList.FirstOrDefault(x => x.Action == ECharacterBuildAction.Destroy)).Forget();
                        _activeCards[order + 1].ReplaceAction(2, _actionsList.FirstOrDefault(x => x.Action == ECharacterBuildAction.Destroy)).Forget();
                    }
                    break;
                
                case ECardSpecialEffect.Doubler:
                    _activeCards[0].AddCopy();
                    break;
                
                case ECardSpecialEffect.Sapper:
                    for (var i = order + 1; i < _activeCards.Length; i++)
                    {
                        _activeCards[i].ReplaceActionsOfType(ECharacterBuildAction.Nothing, _actionsList.FirstOrDefault(x => x.Action == ECharacterBuildAction.BuildSolid)).Forget();
                    }
                    break;
            }
        }

        public void Disable()
        {
            _cancellationToken.Cancel();
            _cancellationToken.Dispose();
        }

        public void SkipTurn()
        {
            Reinit();
        }
    }
}