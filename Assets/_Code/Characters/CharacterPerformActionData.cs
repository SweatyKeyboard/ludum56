using UnityEngine;

namespace _Code.Characters
{
    public sealed class CharacterPerformActionData
    {
        public ECharacterBuildAction Action { get; }
        public Vector2Int Position { get; }

        public CharacterPerformActionData(ECharacterBuildAction action, Vector2Int position)
        {
            Action = action;
            Position = position;
        }
    }
}