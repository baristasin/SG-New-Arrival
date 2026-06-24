using UnityEngine;

namespace Game.Scripts.Night
{
    // Simple manual top-down camera follow. Sits at _target.position + _offset, smoothly damps
    // its way there each frame, and (optionally) keeps looking at the target. Use this on the
    // Main Camera in place of Cinemachine for a predictable fixed-angle view.
    public class TopDownCameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offset = new Vector3(-7f, 12f, -11f);
        [SerializeField] private float _positionDamping = 8f;
        [SerializeField] private bool _lookAtTarget = true;

        private void OnEnable()
        {
            if (_target != null) transform.position = _target.position + _offset;
            if (_target != null && _lookAtTarget) transform.LookAt(_target);
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            Vector3 desired = _target.position + _offset;
            transform.position = Vector3.Lerp(
                transform.position, desired, _positionDamping * Time.deltaTime);

            if (_lookAtTarget) transform.LookAt(_target);
        }
    }
}
