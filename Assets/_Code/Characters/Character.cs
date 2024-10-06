using System;
using _Code.Cards;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _Code.Characters
{
    public sealed class Character : MonoBehaviour
    {
        private static readonly int OnJump = Animator.StringToHash("OnJump");
        private static readonly int OnMove = Animator.StringToHash("OnMove");
        private static readonly int OnAct = Animator.StringToHash("OnAct");
        private static readonly int OnDeath = Animator.StringToHash("OnDeath");
        private static readonly int OnDance = Animator.StringToHash("OnDance");
        
        [SerializeField] private float _blockMoveTime = 2f / 3f;
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _pupil;
        [SerializeField] private SpriteRenderer _hat;
        
        private ECharacterAnimation _selectedAnim = 0;
        public event Func<CharacterPerformActionData, bool> TriedToPerformAction;
        private ActionSOData[] _actions;
        
        private Vector2Int _gridPosition;
        public async UniTask MoveToPosition(Vector3 position, Vector2Int gridPosition)
        {
            if (!Mathf.Approximately(position.y, transform.position.y))
            {
                if (_selectedAnim != ECharacterAnimation.Jump)
                {
                    _selectedAnim = ECharacterAnimation.Jump;
                    _animator.SetTrigger(OnJump);
                }
                await transform.DOJump(position, 0.33f, 1, _blockMoveTime).SetEase(Ease.Linear);
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
            _gridPosition = gridPosition;
        }

        public async UniTask PerformAction()
        {
            _animator.SetTrigger(OnAct);
            for (var i = 0; i < 3; i++)
            {
                var isSuccess = TriedToPerformAction?.Invoke(new CharacterPerformActionData(
                        _actions[i].Action,
                        _gridPosition + new Vector2Int(1, -i + 1)
                )) ?? false;

                if (isSuccess)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(_blockMoveTime));
                }
            }

            Die().Forget();
        }

        private async UniTask Die()
        {
            _animator.SetTrigger(OnDeath);
            await UniTask.Delay(TimeSpan.FromSeconds(_blockMoveTime));
            Destroy(gameObject);
        }


        private enum ECharacterAnimation
        {
            Idle,
            Move,
            Jump
        }

        public void Init(CardSOData data, ActionSOData[] actions)
        {
            _pupil.color = data.PupilColor;
            _hat.sprite = data.Icon;
            _actions = actions;
        }

        public void FinishDance()
        {
            _animator.SetTrigger(OnDance);
        }
    }
}