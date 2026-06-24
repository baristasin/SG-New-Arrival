using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Screens
{
    public class LoadingScreenUI : UIScreen
    {
        [SerializeField] private Image _progressFill;

        [SerializeField] private Button _complainButton;
        [SerializeField] private Sprite _emojiSprite;
        [SerializeField] private RectTransform _emojiSpawnParent;
        [SerializeField] private Vector2 _emojiSize = new Vector2(80f, 80f);
        [SerializeField] private float _emojiHorizontalJitter = 40f;
        [SerializeField] private float _emojiRiseDistance = 400f;
        [SerializeField] private float _emojiDuration = 1.5f;

        public override Tween Show()
        {
            if (IsShown) return null;
            IsShown = true;
            _canvasGroup.DOKill();
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;

            SetProgress(0f);
            if (_complainButton != null)
            {
                _complainButton.onClick.RemoveListener(SpawnEmoji);
                _complainButton.onClick.AddListener(SpawnEmoji);
                _complainButton.interactable = true;
            }
            return null;
        }

        public override Tween Hide()
        {
            if (!IsShown) return null;
            IsShown = false;
            _canvasGroup.DOKill();
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;

            if (_complainButton != null)
                _complainButton.onClick.RemoveListener(SpawnEmoji);
            return null;
        }


        public IEnumerator FillBar(float duration)
        {
            duration = Mathf.Max(0.01f, duration);
            float t = 0f;
            SetProgress(0f);
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                SetProgress(t / duration);
                yield return null;
            }
            SetProgress(1f);
        }

        public void SetProgress(float p)
        {
            p = Mathf.Clamp01(p);
            if (_progressFill != null) _progressFill.fillAmount = p;
        }
        
        private void SpawnEmoji()
        {
            if (_emojiSprite == null || _emojiSpawnParent == null) return;

            var go = new GameObject("ComplainEmoji", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
            var rt = (RectTransform)go.transform;
            rt.SetParent(_emojiSpawnParent, false);
            rt.sizeDelta = _emojiSize;
            rt.anchoredPosition = new Vector2(Random.Range(-_emojiHorizontalJitter, _emojiHorizontalJitter), 0f);

            var img = go.GetComponent<Image>();
            img.sprite = _emojiSprite;
            img.raycastTarget = false;

            var cg = go.GetComponent<CanvasGroup>();
            cg.alpha = 1f;

            rt.DOAnchorPosY(_emojiRiseDistance, _emojiDuration).SetEase(Ease.OutQuad);
            cg.DOFade(0f, _emojiDuration).SetEase(Ease.InQuad)
                .OnComplete(() => Destroy(go));
        }
    }
}
