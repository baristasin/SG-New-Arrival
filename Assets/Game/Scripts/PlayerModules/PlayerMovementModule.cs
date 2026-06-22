using FMODUnity;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts.PlayerModules
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerMovementModule : MonoBehaviour
    {
        [SerializeField] private Camera _cam;
        [SerializeField] private float _moveSpeed = 4f;
        [SerializeField] private float _rotationSpeed = 720f;
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private PlayerAnimationModule _animationModule;

        [Tooltip("On = character always faces the mouse cursor (combat/aim mode). " +
                 "Off = character faces its movement direction (exploration mode).")]
        [SerializeField] private bool _faceMouseDirection = true;

        [SerializeField] private EventReference _footstepEvent;

        private void Awake()
        {
            _agent.updateRotation = false;
            _agent.speed = _moveSpeed;
            _agent.acceleration = 999f;
            _agent.angularSpeed = 0f;
            // if (_cam == null) Debug.Log("No cam assigned");
        }

        private void Update()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Vector3 camForward = _cam.transform.forward;
            Vector3 camRight = _cam.transform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = camForward * v + camRight * h;
            if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();
            bool isMoving = moveDir.sqrMagnitude > 0.0001f;

            if (isMoving)
            {
                _agent.Move(moveDir * _moveSpeed * Time.deltaTime);
                // AudioManager.Instance.PlayOneShotNoOverlapAttached("Footstep-Single-2", _footstepEvent, gameObject);
            }

            // _faceMouseDirection toggles which way the body looks. On = combat/aim feel
            // (NightCity). Off = third-person exploration feel (DayCity).
            Vector3 facing = Vector3.zero;
            if (_faceMouseDirection)
            {
                Vector3 aimDir = GetAimDirection();
                aimDir.y = 0f;
                if (aimDir.sqrMagnitude > 0.0001f) facing = aimDir;
            }
            else if (isMoving)
            {
                facing = moveDir;
            }

            if (facing.sqrMagnitude > 0.0001f)
            {
                Quaternion target = Quaternion.LookRotation(facing.normalized);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, target, _rotationSpeed * Time.deltaTime);
            }

            if (_animationModule != null) _animationModule.SetMoving(isMoving);
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