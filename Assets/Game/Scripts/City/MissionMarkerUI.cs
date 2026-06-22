using UnityEngine;

namespace Game.Scripts.City
{
    // Mission objective marker. Finds the MissionTrigger that matches the day's planned minigame
    // and parks an arrow on screen:
    //   • Target on-screen  → arrow sits over the target (marker mode).
    //   • Target off-screen → arrow clamped to the screen edge, rotated toward the target.
    // Hides itself outside CityRoaming. Lives in the DayCity scene on a Screen Space Overlay
    // Canvas — the arrow's Image is a child of that canvas.
    public class MissionMarkerUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _arrow;
        [SerializeField] private Camera _camera;
        [Tooltip("Distance kept from each screen edge when the arrow is clamped to the border.")]
        [SerializeField] private float _edgePadding = 80f;
        [Tooltip("Rotate the arrow toward the target even when it's already on-screen. Off = " +
                 "arrow sits flat over the target like a marker; only rotates while off-screen.")]
        [SerializeField] private bool _rotateOnScreen = false;

        private Transform _target;

        private void OnEnable()
        {
            if (_camera == null) _camera = Camera.main;

            var gm = GameManager.Instance;
            if (gm != null) gm.OnStateChanged += HandleStateChanged;

            FindTarget();
            UpdateVisibility();
        }

        private void OnDisable()
        {
            var gm = GameManager.Instance;
            if (gm != null) gm.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState prev, GameState next)
        {
            if (next == GameState.CityRoaming) FindTarget();
            UpdateVisibility();
        }

        private void FindTarget()
        {
            var gm = GameManager.Instance;
            var today = gm != null ? gm.GetTodayMinigame() : null;
            if (today == null) { _target = null; return; }

            var triggers = Object.FindObjectsByType<MissionTrigger>(FindObjectsSortMode.None);
            for (int i = 0; i < triggers.Length; i++)
            {
                if (triggers[i].Id == today.Value)
                {
                    _target = triggers[i].transform;
                    return;
                }
            }
            _target = null;
        }

        private void UpdateVisibility()
        {
            var gm = GameManager.Instance;
            bool show = gm != null && gm.State == GameState.CityRoaming && _target != null && _arrow != null;
            if (_arrow != null) _arrow.gameObject.SetActive(show);
        }

        private void LateUpdate()
        {
            if (_target == null || _arrow == null || _camera == null) return;
            if (!_arrow.gameObject.activeSelf) return;

            Vector3 screenPos = _camera.WorldToScreenPoint(_target.position);
            bool isBehind = screenPos.z < 0f;
            if (isBehind) { screenPos.x = -screenPos.x; screenPos.y = -screenPos.y; }

            Vector2 center = new Vector2(Screen.width, Screen.height) * 0.5f;
            float halfW = Screen.width * 0.5f - _edgePadding;
            float halfH = Screen.height * 0.5f - _edgePadding;

            bool onScreen = !isBehind &&
                            screenPos.x >= _edgePadding && screenPos.x <= Screen.width - _edgePadding &&
                            screenPos.y >= _edgePadding && screenPos.y <= Screen.height - _edgePadding;

            if (onScreen)
            {
                _arrow.position = screenPos;
                _arrow.localRotation = _rotateOnScreen
                    ? RotationTowards((Vector2)screenPos - center)
                    : Quaternion.identity;
                return;
            }

            // Off-screen: clamp the screen-space position to the inner rectangle, point toward target.
            Vector2 dir = ((Vector2)screenPos - center).normalized;
            float tx = Mathf.Abs(dir.x) > 0.001f ? halfW / Mathf.Abs(dir.x) : float.MaxValue;
            float ty = Mathf.Abs(dir.y) > 0.001f ? halfH / Mathf.Abs(dir.y) : float.MaxValue;
            float t = Mathf.Min(tx, ty);

            _arrow.position = center + dir * t;
            _arrow.localRotation = RotationTowards(dir);
        }

        private static Quaternion RotationTowards(Vector2 dir)
        {
            // Arrow sprite assumed to point UP at zero rotation. Atan2 gives angle from +X CCW,
            // so subtract 90° to align the sprite's UP with the direction vector.
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            return Quaternion.Euler(0f, 0f, angle);
        }
    }
}
