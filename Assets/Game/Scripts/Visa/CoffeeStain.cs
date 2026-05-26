using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Visa
{
    // One pooled coffee-stain overlay. The corruption handler places it on a paper.
    public class CoffeeStain : MonoBehaviour
    {
        [SerializeField] private Image _image;

        public void Show(Sprite sprite, RectTransform parent, Vector2 anchoredPos, float rotationZ, float scale)
        {
            _image.sprite = sprite;

            var rt = (RectTransform)transform;
            rt.SetParent(parent, false);
            rt.anchoredPosition = anchoredPos;
            rt.localRotation = Quaternion.Euler(0f, 0f, rotationZ);
            rt.localScale = Vector3.one * scale;
        }
    }
}
