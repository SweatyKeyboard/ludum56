using UnityEngine;

namespace _Code.Level.Blocks
{
    public class Block : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer _spriteRenderer;

        public virtual int HP { get; set; } = 1;
    }
}