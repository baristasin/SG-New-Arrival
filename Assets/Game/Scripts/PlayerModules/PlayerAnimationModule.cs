using UnityEngine;

namespace Game.Scripts.PlayerModules
{
    public class PlayerAnimationModule : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private string _movingParameter = "IsMoving";

        private int _movingHash;

        private void Awake()
        {
            _movingHash = Animator.StringToHash(_movingParameter);
        }

        public void SetMoving(bool isMoving)
        {
            if (_animator == null) return;
            _animator.SetBool(_movingHash, isMoving);
        }
    }
}
