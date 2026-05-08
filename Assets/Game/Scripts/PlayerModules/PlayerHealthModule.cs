using System.Collections;
using UnityEngine;

namespace Game.Scripts.PlayerModules
{
    public class PlayerHealthModule : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;

        private Color _originalColor;
        private Coroutine _flashCoroutine;

        private void Awake()
        {
            _originalColor = _renderer.material.color;
        }

        public void TakeDamage(int damage)
        {
            if (_flashCoroutine != null) return;
            _flashCoroutine = StartCoroutine(FlashRed());
        }

        private IEnumerator FlashRed()
        {
            _renderer.material.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            _renderer.material.color = _originalColor;
            _flashCoroutine = null;
        }
    }
}