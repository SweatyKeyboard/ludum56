using _Code.Characters;
using UnityEngine;

namespace _Code.Cards
{
    [CreateAssetMenu(fileName = "ActionSOData", menuName = "ActionSOData")]
    public sealed class ActionSOData : ScriptableObject
    {
        [field: SerializeField] public ECharacterBuildAction Action { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
    }
}