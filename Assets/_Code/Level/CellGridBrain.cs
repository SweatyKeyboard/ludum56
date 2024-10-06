using System.Collections.Generic;
using System.Linq;
using _Code.Characters;
using _Code.Level.Blocks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _Code.Level
{
    public sealed class CellGridBrain : MonoBehaviour
    {
        [SerializeField] private LevelSOData _levelSOData;
        [SerializeField] private CellGridSOData _cellGridSOData;
        
        private int[,] _cells;
        private readonly Dictionary<Vector2Int, Block> _blocks = new();
        private readonly Vector3 _adjustPositionVector = new (0, -0.04f, 0);

        private void Start()
        {
            LoadFromLevel(_levelSOData);
            Initialize();
        }

        public void Initialize()
        {
            RedrawGrid();
            var lastCell = 0;
            
            for (var i = 0; i < _levelSOData.Width; i++)
            {
                lastCell = GetAvailableBlockInColumn(i, lastCell);
                if (lastCell == -1)
                    return;
                
            }
        }

        public void RedrawGrid()
        {
            for (var i = 0; i < _cellGridSOData.Width; i++)
            {
                for (var j = 0; j < _cellGridSOData.Height; j++)
                {
                    var blockId = _cells[i, j];
                    if (blockId != 0)
                    {
                        if (!_blocks.ContainsKey(new Vector2Int(i, j)))
                            SpawnBlock(i, j, _cellGridSOData.BlocksPrefabs[blockId - 1], true);
                    }
                }
            }
        }

        private void LoadFromLevel(LevelSOData levelData)
        {
            _cells = new int[levelData.Width, levelData.Height];
            for (var i = 0; i < levelData.Height; i++)
            {
                for (var j = 0; j < levelData.Width; j++)
                {
                    _cells[j, i] = levelData.Cells[j + i * levelData.Width];
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (_cellGridSOData == null)
                return;
            
            for (var i = 0; i < _cellGridSOData.Width; i++)
            {
                for (var j = 0; j < _cellGridSOData.Height; j++)
                {
                    Gizmos.DrawWireCube(transform.position + new Vector3(i, j, 0) * _cellGridSOData.BlockSize, Vector3.one * _cellGridSOData.BlockSize);
                }
            }
        }

        public int GetAvailableBlockInColumn(int columnIndex, int lastColumnAvailableBlock)
        {
            for (var i = Mathf.Clamp(lastColumnAvailableBlock - 2, 0, _cellGridSOData.Height - 1); i <= Mathf.Clamp(lastColumnAvailableBlock, 0, _cellGridSOData.Height - 1); i++)
            {
                if (_cells[columnIndex, i] != 0 && _cells[columnIndex, i + 1] == 0)
                {
                    if (i >= lastColumnAvailableBlock && columnIndex > 0 && _cells[Mathf.Clamp(columnIndex - 1, 0, _cellGridSOData.Width - 1), i + 1] != 0)
                        continue;
                    
                    return i + 1;
                }
            } 
            return -1;
        }

        public Vector3 GetCellPosition(int currentColumn, int currentRow)
        {
            return transform.position + new Vector3(currentColumn, currentRow, 0) * _cellGridSOData.BlockSize + _adjustPositionVector;
        }

        public bool TryPerformAction(CharacterPerformActionData data)
        {
            bool actionPerformed = false;
            switch (data.Action)
            {
                case ECharacterBuildAction.Build:
                    if (_cells[data.Position.x, data.Position.y] == 0)
                    {
                        SpawnBlock(data.Position.x, data.Position.y, _cellGridSOData.BlocksPrefabs[0]);
                        _cells[data.Position.x, data.Position.y] = 1;
                        actionPerformed = true;
                    }
                    break;
                
                case ECharacterBuildAction.Destroy:
                    if (_cells[data.Position.x, data.Position.y] != 0)
                    {
                        DestroyBlock(data.Position.x, data.Position.y).Forget();
                        _cells[data.Position.x, data.Position.y] = 0;
                        actionPerformed = true;
                    }
                    break;
            }
            
            return actionPerformed;
        }

        private void SpawnBlock(int x, int y, Block blocksPrefab, bool isSkipAnimation = false)
        {
            var spawnedBlock = Instantiate(blocksPrefab,
                    transform.position + new Vector3(x, y, 0) * _cellGridSOData.BlockSize,
                    Quaternion.identity);

            if (isSkipAnimation)
            {
                spawnedBlock.transform.localScale = Vector3.one * _cellGridSOData.BlockSize;
            }
            else
            {
                spawnedBlock.transform.localScale = Vector3.zero; 
                spawnedBlock.transform.DOScale(Vector3.one * _cellGridSOData.BlockSize, 2f / 3f).SetEase(Ease.InCubic);
            
            }
            
            _blocks.Add(new Vector2Int(x, y), spawnedBlock);
        }

        private async UniTask DestroyBlock(int positionX, int positionY)
        {
            var block = _blocks.FirstOrDefault( x=> x.Key.x == positionX && x.Key.y == positionY);
            await block.Value.transform.DOScale(Vector3.zero, 2f / 3f).SetEase(Ease.OutQuint);
            _blocks.Remove(new Vector2Int(positionX, positionY));
            Destroy(block.Value.gameObject);
        }
    }
}