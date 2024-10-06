using _Code.Characters;
using UnityEngine;

namespace _Code.Cards
{
    [CreateAssetMenu(fileName = "CardSOData", menuName = "CardSOData")]
    public class CardSOData : ScriptableObject
    {
        [field: SerializeField] public string Header { get; private set; }
        [field: TextArea] [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public CharacterSOData Data { get; private set; }
        [field: SerializeField] public ECardSpecialEffect Special { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public Color PupilColor { get; private set; }
    }
}