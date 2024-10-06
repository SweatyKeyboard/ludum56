using _Code.Cards;
using _Code.Level;
using _Code.Menues;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace _Code.Characters
{
    public sealed class CharactersManager : MonoBehaviour
    {
        [SerializeField] private CellGridBrain _cellGridBrain;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Character _characterPrefab;
        [SerializeField] private Transform _finishPos;
        [SerializeField] private WinMenu _winMenu;
        [SerializeField] private CardManager _cardManager;
        
        private Character _character;

        public async UniTask StartCharacterMoving()
        {
            var currentColumn = 0;
            var currentRow = 0;
            
            while (currentRow != -1)
            {
                currentRow = _cellGridBrain.GetAvailableBlockInColumn(currentColumn, currentRow, _character.Extra);
                if (currentRow == -1)
                    break;

                var position = currentColumn < _cellGridBrain.Data.Width
                        ? _cellGridBrain.GetCellPosition(currentColumn, currentRow)
                        : _finishPos.position;
                await _character.MoveToPosition(position, new Vector2Int(currentColumn, currentRow));
                currentColumn++;
            }
            
            if (currentColumn >= _cellGridBrain.Data.Width)
            {
                FinishGame();
                return;
            }
            
            StartCharacterActing().Forget();
        }

        private void FinishGame()
        {
            _character.FinishDance();
            _cardManager.Disable();
            _winMenu.Show();
        }

        private async UniTask StartCharacterActing()
        {
            _character.TriedToPerformAction += _cellGridBrain.TryPerformAction;
            _character.PerformAction().Forget();
        }

        public async UniTask SpawnNewCharacter(CardSOData card, ActionSOData[] actions)
        {
            var spawnedCharacter = Instantiate(_characterPrefab, _spawnPoint.position, Quaternion.identity);
            spawnedCharacter.Init(card, actions);
            _character = spawnedCharacter;
            _character.Extra = card;
            StartCharacterMoving().Forget();
            await UniTask.WaitUntil(() => _character.IsDestroyed());
        }
    }
}