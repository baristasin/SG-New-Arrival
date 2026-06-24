using System;
using System.Collections.Generic;
using Game.Scripts.StudentData;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Visa
{
    [Serializable]
    public class PhotoData
    {
        public Sprite Photo;
        public int PhotoCount;
        public PhotoQuality Quality;
    }

    public class PhotoDocument : VisaDocumentBase<PhotoData>
    {
        [SerializeField] private List<Image> _photoImages;

        [SerializeField] private GameObject _sunglassesPrefab;
        [SerializeField] private GameObject _blurPrefab;

        private readonly List<GameObject> _activeOverlays = new();

        public override void Initialize(PhotoData data)
        {
            base.Initialize(data);
            ClearOverlays();

            var overlayPrefab = GetOverlayForQuality(data.Quality);

            for (int i = 0; i < _photoImages.Count; i++)
            {
                bool show = i < data.PhotoCount;
                _photoImages[i].gameObject.SetActive(show);

                if (!show) continue;
                if (data.Photo != null) _photoImages[i].sprite = data.Photo;

                if (overlayPrefab != null)
                {
                    var overlay = Instantiate(overlayPrefab, _photoImages[i].rectTransform, false);
                    _activeOverlays.Add(overlay);
                }
            }
        }

        private GameObject GetOverlayForQuality(PhotoQuality quality)
        {
            switch (quality)
            {
                case PhotoQuality.Sunglasses: return _sunglassesPrefab;
                case PhotoQuality.Blurred:    return _blurPrefab;
                default:                       return null;
            }
        }

        private void ClearOverlays()
        {
            foreach (var o in _activeOverlays)
                if (o != null) Destroy(o);
            _activeOverlays.Clear();
        }
    }
}
