using UnityEngine;

namespace Game.Scripts.CharacterMovement
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMotor : MonoBehaviour
    {
        public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
        public Vector3 Velocity => _moveInput * _moveSpeed;

        [SerializeField] private float _moveSpeed = 4f;
        [SerializeField] private float _rotationSpeed = 720f;

        private Rigidbody _rb;
        private Vector3 _moveInput;
        private Vector3 _faceDir;        

        // Default Settings
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.isKinematic = true;
            _rb.useGravity = false;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            _rb.constraints = RigidbodyConstraints.FreezeRotation
                            | RigidbodyConstraints.FreezePositionY;
            _faceDir = transform.forward;
        }

        public void SetMoveInput(Vector3 direction)
        {
            direction.y = 0f;
            if (direction.sqrMagnitude > 1f) direction.Normalize();
            _moveInput = direction;
        }
        
        public void SetFaceDirection(Vector3 direction)
        {
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.0001f) _faceDir = direction;
        }

        private void FixedUpdate()
        {
            if (_moveInput.sqrMagnitude > 0.0001f)
            {
                Vector3 delta = _moveInput * _moveSpeed * Time.fixedDeltaTime;
                _rb.MovePosition(_rb.position + delta);
            }

            if (_faceDir.sqrMagnitude > 0.0001f)
            {
                Quaternion target = Quaternion.LookRotation(_faceDir);
                Quaternion next = Quaternion.RotateTowards(
                    _rb.rotation, target, _rotationSpeed * Time.fixedDeltaTime);
                _rb.MoveRotation(next);
            }
        }
    }
}