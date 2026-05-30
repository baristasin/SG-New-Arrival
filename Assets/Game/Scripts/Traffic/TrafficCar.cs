using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Game.Scripts.Traffic
{
    // Follows a SplineContainer at a fixed speed. TrafficRoute instantiates one per car and
    // calls Setup with the spline, an initial t value (so cars are spaced along the road), and
    // a per-instance speed. Loops by default; on a non-looping route the car self-destructs at
    // the end of the path.
    public class TrafficCar : MonoBehaviour
    {
        private SplineContainer _spline;
        private float _t;
        private float _speed;          // metres per second
        private bool _loop;
        private float _length;         // metres — cached on Setup; spline assumed static

        public void Setup(SplineContainer spline, float startT, float speed, bool loop)
        {
            _spline = spline;
            _t = Mathf.Repeat(startT, 1f);
            _speed = Mathf.Max(0f, speed);
            _loop = loop;
            _length = _spline != null ? _spline.CalculateLength() : 0f;
            UpdatePose();
        }

        private void Update()
        {
            if (_spline == null || _length <= 0f) return;

            _t += _speed * Time.deltaTime / _length;

            if (_t >= 1f)
            {
                if (_loop) _t -= 1f;
                else { Destroy(gameObject); return; }
            }

            UpdatePose();
        }

        private void UpdatePose()
        {
            if (!_spline.Evaluate(_t, out float3 pos, out float3 tangent, out _)) return;

            transform.position = pos;
            Vector3 tan = (Vector3)tangent;
            if (tan.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(tan);
        }
    }
}
