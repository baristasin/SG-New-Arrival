using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Visa
{
    [Serializable]
    public class PhotoData
    {
        public Sprite Photo;
    }

    public class PhotoDocument : VisaDocumentBase<PhotoData>
    {
        [SerializeField] private Image _photoImage;

        public override void Initialize(PhotoData data)
        {
            base.Initialize(data);
            _photoImage.sprite = data.Photo;
        }
    }
}
