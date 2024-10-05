using UnityEngine;

namespace _Code.Characters
{
    [CreateAssetMenu(fileName = "CharacterSOData", menuName = "CharacterSOData")]
    public class CharacterSOData : ScriptableObject
    {
        [SerializeField] private ECharacterBuildAction _topBlockAction;
        [SerializeField] private ECharacterBuildAction _middleBlockAction;
        [SerializeField] private ECharacterBuildAction _bottomBlockAction;
        
        public ECharacterBuildAction[] Actions => new ECharacterBuildAction[] { _topBlockAction, _middleBlockAction, _bottomBlockAction };
    }
}