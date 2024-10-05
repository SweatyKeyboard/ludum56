using UnityEngine;

namespace _Code.Level
{
    [CreateAssetMenu(fileName = "LevelSOData", menuName = "LevelSOData")]
    public sealed class LevelSOData : ScriptableObject
    {
        public int Width;
        public int Height;
        public int[] Cells;
    }
}