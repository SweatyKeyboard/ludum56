using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace _Code.Characters
{
    public sealed class Character : MonoBehaviour
    {
        private static readonly int OnJump = Animator.StringToHash("OnJump");
        private static readonly int OnMove = Animator.StringToHash("OnMove");
        
        [SerializeField] private float _blockMoveTime = 0.8f;
        [SerializeField] private Animator _animator;
        
        private ECharacterAnimation _selectedAnim = 0;
        
        public async UniTask MoveToPosition(Vector3 position)
        {

            if (!Mathf.Approximately(position.y, transform.position.y))
            {
                if (_selectedAnim != ECharacterAnimation.Jump)
                {
                    _selectedAnim = ECharacterAnimation.Jump;
                    _animator.SetTrigger(OnJump);
                }
                await transform.DOJump(position, 0.5f, 1, _blockMoveTime).SetEase(Ease.Linear);
            }
            else
            {
                if (_selectedAnim != ECharacterAnimation.Move)
                {
                    _selectedAnim = ECharacterAnimation.Move;
                    _animator.SetTrigger(OnMove);
                }
                await transform.DOMove(position, _blockMoveTime).SetEase(Ease.Linear);
            }
        }

        public async UniTask PerformAction()
        {
            
        }
        
        
        private enum ECharacterAnimation
        {
            Idle,
            Move,
            Jump
        }
    }
}