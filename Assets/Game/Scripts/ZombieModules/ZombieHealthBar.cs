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

        public void Show(Transform target, float healthPercentage)
        {
            _target = target;
            _fillImage.fillAmount = healthPercentage;
            gameObject.SetActive(true);

            if (_hideCoroutine != null)
                StopCoroutine(_hideCoroutine);
            _hideCoroutine = StartCoroutine(HideAfterDelay());
        }

        private void LateUpdate()
        {
            if (_target == null || !_target.gameObject.activeInHierarchy)
            {
                ReturnToPool();
                return;
            }

            transform.position = _target.position + _offset;
            transform.forward = Camera.main.transform.forward;
        }

        private IEnumerator HideAfterDelay()
        {
            yield return new WaitForSeconds(_displayDuration);
            ReturnToPool();
        }

        private void ReturnToPool()
        {
            if (_hideCoroutine != null)
            {
                StopCoroutine(_hideCoroutine);
                _hideCoroutine = null;
            }
            _target = null;
            ZombieHealthBarPool.Return(this);
        }
    }
}
