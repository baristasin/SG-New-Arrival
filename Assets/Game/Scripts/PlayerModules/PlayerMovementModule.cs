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

            if (moveDir.sqrMagnitude > 0.0001f)
            {
                _agent.Move(moveDir * _moveSpeed * Time.deltaTime);
                AudioManager.Instance.PlayOneShotNoOverlapAttached("Footstep-Single-2", _footstepEvent, gameObject);
            }

            if (Input.GetMouseButton(1))
            {
                Vector3 aimDir = GetAimDirection();
                aimDir.y = 0f;
                if (aimDir.sqrMagnitude > 0.0001f)
                {
                    Quaternion target = Quaternion.LookRotation(aimDir.normalized);
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation, target, _rotationSpeed * Time.deltaTime);
                }
            }
            else if (moveDir.sqrMagnitude > 0.0001f)
            {
                Quaternion target = Quaternion.LookRotation(moveDir.normalized);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, target, _rotationSpeed * Time.deltaTime);
            }
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