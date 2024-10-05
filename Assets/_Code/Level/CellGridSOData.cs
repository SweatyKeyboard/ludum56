using _Code.Level.Blocks;
using UnityEngine;

namespace _Code.Level
{
    [CreateAssetMenu(fileName = "CellGridSOData", menuName = "CellGridSOData")]
    public sealed class CellGridSOData : ScriptableObject
    {
        [field: SerializeField] public int Width { get; private set; }
        [field: SerializeField] public int Height { get; private set; }
        [field: SerializeField] public float BlockSize { get; private set; }
        [field: SerializeField] public Block[] BlocksPrefabs { get; private set; }
    }
}