using _Code.Cards;
using UnityEngine;

namespace _Code.Characters
{
    [CreateAssetMenu(fileName = "CharacterSOData", menuName = "CharacterSOData")]
    public class CharacterSOData : ScriptableObject
    {
        [SerializeField] private ActionSOData _topBlockAction;
        [SerializeField] private ActionSOData _middleBlockAction;
        [SerializeField] private ActionSOData _bottomBlockAction;
        
        public ECharacterBuildAction[] Actions => new [] { _topBlockAction.Action, _middleBlockAction.Action, _bottomBlockAction.Action };
        public Sprite[] Sprites => new [] { _topBlockAction.Icon, _middleBlockAction.Icon, _bottomBlockAction.Icon };
    }
}