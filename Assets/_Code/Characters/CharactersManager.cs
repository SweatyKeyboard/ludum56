using System;
using _Code.Level;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Code.Characters
{
    public sealed class CharactersManager : MonoBehaviour
    {
        [SerializeField] private CellGridBrain _cellGridBrain;
        [SerializeField] private Character _character;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                StartCharacterMoving().Forget();
        }

        public async UniTask StartCharacterMoving()
        {
            var currentColumn = 0;
            var currentRow = 0;
            
            while (currentRow != -1)
            {
                Debug.Log(currentColumn);
                currentRow = _cellGridBrain.GetAvailableBlockInColumn(currentColumn, currentRow);
                if (currentRow == -1)
                    return;
                
                await _character.MoveToPosition(_cellGridBrain.GetCellPosition(currentColumn, currentRow));
                currentColumn++;
            }
        }
    }
}