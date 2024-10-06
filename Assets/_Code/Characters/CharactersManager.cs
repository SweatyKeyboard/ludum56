using _Code.Cards;
using _Code.Level;
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
        
        private Character _character;

        public async UniTask StartCharacterMoving()
        {
            var currentColumn = 0;
            var currentRow = 0;
            
            while (currentRow != -1)
            {
                currentRow = _cellGridBrain.GetAvailableBlockInColumn(currentColumn, currentRow);
                if (currentRow == -1)
                    break;
                
                await _character.MoveToPosition(_cellGridBrain.GetCellPosition(currentColumn, currentRow), new Vector2Int(currentColumn, currentRow));
                currentColumn++;
            }
            
            StartCharacterActing().Forget();
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
            StartCharacterMoving().Forget();
            await UniTask.WaitUntil(() => _character.IsDestroyed());
        }
    }
}