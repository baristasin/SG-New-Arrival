using UnityEngine;

namespace Game.Scripts.ApartmentHunting
{
    public abstract class HuntingPaperBase<TData> : MonoBehaviour
    {
        public RectTransform RectTransform => _rectTransform;

        [SerializeField] private RectTransform _rectTransform;

        public TData Data { get; private set; }

        public virtual void Initialize(TData data)
        {
            Data = data;
        }
    }
}
