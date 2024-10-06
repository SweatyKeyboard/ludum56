using UnityEngine;

namespace _Code.Level.Blocks
{
    public sealed class SolidBlock : Block
    {
        [SerializeField] private Sprite _solidState;
        [SerializeField] private Sprite _usualState;

        private int _hp = 2;

        public override int HP
        {
            get => _hp;
            set
            {
                _hp = value;
                if (_hp == 1)
                    _spriteRenderer.sprite = _usualState;
            }
        }
    }
}