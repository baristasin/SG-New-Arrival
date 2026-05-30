using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.ZombieModules
{
    public class ZombieHealthBar : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private float _displayDuration = 1.5f;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 2f, 0f);

        private Transform _target;
        private Coroutine _hideCoroutine;

        private bool _isTargetDead;

        // Exposed so the pool can clear its target → bar map when this bar is returned.
        public Transform Target => _target;

        public void Show(Transform target, int remainingHealth, int maxHealth)
        {
            _target = target;
            _fillImage.fillAmount = maxHealth > 0 ? Mathf.Max(0f, (float)remainingHealth / maxHealth) : 0f;
            _isTargetDead = remainingHealth <= 0;
            gameObject.SetActive(true);

            if (_hideCoroutine != null)
                StopCoroutine(_hideCoroutine);
            _hideCoroutine = StartCoroutine(HideAfterDelay());
        }

        private void LateUpdate()
        {
            if (_target == null || !_target.gameObject.activeInHierarchy || _isTargetDead)
            {
                gameObject.SetActive(false);
                ReturnToPool();
                return;
            }

            transform.position = _target.position + _offset;
            transform.forward = Camera.main.transform.forward;
        }

        private IEnumerator HideAfterDelay()
        {
            yield return new WaitForSeconds(_displayDuration);
            gameObject.SetActive(false);
            ReturnToPool();
        }

        // Called by the pool / ZombieController.ZombieDead the moment the zombie dies, so the
        // bar disappears in the same frame instead of lingering for the death animation.
        public void HideImmediate()
        {
            if (_hideCoroutine != null)
            {
                StopCoroutine(_hideCoroutine);
                _hideCoroutine = null;
            }
            gameObject.SetActive(false);
            ReturnToPool();
        }

        private void ReturnToPool()
        {
            if (_hideCoroutine != null)
            {
                StopCoroutine(_hideCoroutine);
                _hideCoroutine = null;
            }
            // Keep _target set — the pool reads it to remove its target → bar mapping. The next
            // Show() call will overwrite it.
            ZombieHealthBarPool.Return(this);
        }
    }
}
