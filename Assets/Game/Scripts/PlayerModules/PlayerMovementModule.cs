using Game.Scripts.CharacterMovement;
using UnityEngine;

namespace Game.Scripts.PlayerModules
{
    [RequireComponent(typeof(CharacterMotor))]
    public class PlayerMovementModule : MonoBehaviour
    {
        [SerializeField] private Camera _cam;
        [SerializeField] private bool _isAiming;

        private CharacterMotor _motor;

        public bool IsAiming { get => _isAiming; set => _isAiming = value; }

        private void Awake()
        {
            _motor = GetComponent<CharacterMotor>();
            if (_cam == null) Debug.Log("No cam assigned");
        }

        private void Update()
        {
            Vector3 faceDir = GetAimDirection();
            faceDir.y = 0f;

            if (faceDir.sqrMagnitude < 0.0001f)
            {
                _motor.SetMoveInput(Vector3.zero);
                return;
            }

            faceDir.Normalize();
            _motor.SetFaceDirection(faceDir);

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
        
            Vector3 right = new Vector3(faceDir.z, 0f, -faceDir.x);

            _motor.SetMoveInput(faceDir * v + right * h);
        }

        private Vector3 GetAimDirection()
        {
            Plane plane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float dist))
                return ray.GetPoint(dist) - transform.position;
            return transform.forward;
        }
    }
}