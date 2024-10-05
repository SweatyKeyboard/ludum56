using System.Collections.Generic;
using _Code.Level.Blocks;
using UnityEngine;

namespace _Code.Level
{
    public sealed class CellGridBrain : MonoBehaviour
    {
        [SerializeField] private LevelSOData _levelSOData;
        [SerializeField] private CellGridSOData _cellGridSOData;
        
        private int[,] _cells;
        private readonly List<Vector2Int> _blocksList = new();

        private void Start()
        {
            LoadFromLevel(_levelSOData);
            Initialize();
        }

        public void Initialize()
        {
            RedrawGrid();
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
                        if (!_blocksList.Contains(new Vector2Int(i, j)))
                            SpawnBlock(i, j, _cellGridSOData.BlocksPrefabs[blockId - 1]);
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
                    Debug.Log(j + " + " + i + " * " + levelData.Height);
                    _cells[j, i] = levelData.Cells[j + i * levelData.Width];
                }
            }
        }

        private void SpawnBlock(int x, int y, Block blocksPrefab)
        {
            var spawnedBlock = Instantiate(blocksPrefab,
                    transform.position + new Vector3(x, y, 0) * _cellGridSOData.BlockSize,
                    Quaternion.identity);
            
            spawnedBlock.transform.localScale *= _cellGridSOData.BlockSize;
            
            _blocksList.Add(new Vector2Int(x, y));
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
    }
}