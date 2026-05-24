using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Visa
{
    [Serializable]
    public class PhotoData
    {
        public Sprite Photo;
        public int PhotoCount;
    }

    public class PhotoDocument : VisaDocumentBase<PhotoData>
    {
        [SerializeField] private List<Image> _photoImages;

        public override void Initialize(PhotoData data)
        {
            base.Initialize(data);
            for (int i = 0; i < _photoImages.Count; i++)
            {
                bool show = i < data.PhotoCount;
                _photoImages[i].gameObject.SetActive(show);
                if (show && data.Photo != null)
                    _photoImages[i].sprite = data.Photo;
            }
        }
    }
}
