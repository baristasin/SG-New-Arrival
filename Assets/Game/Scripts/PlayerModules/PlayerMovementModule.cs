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

        private void Awake()
        {
            _agent.updateRotation = false;
            _agent.speed = _moveSpeed;
            _agent.acceleration = 999f;
            _agent.angularSpeed = 0f;
            if (_cam == null) Debug.Log("No cam assigned");
        }

        private void Update()
        {
            Vector3 faceDir = GetAimDirection();
            faceDir.y = 0f;

            if (faceDir.sqrMagnitude < 0.0001f) return;
            faceDir.Normalize();

            // Rotate
            Quaternion target = Quaternion.LookRotation(faceDir);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, target, _rotationSpeed * Time.deltaTime);

            // Move
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Vector3 right = new Vector3(faceDir.z, 0f, -faceDir.x);
            Vector3 dir = faceDir * v + right * h;

            if (dir.sqrMagnitude > 1f) dir.Normalize();
            if (dir.sqrMagnitude > 0.0001f)
                _agent.Move(dir * _moveSpeed * Time.deltaTime);
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