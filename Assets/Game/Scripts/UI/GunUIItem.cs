using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class GunUIItem : MonoBehaviour
    {
        [SerializeField] private Image _selectedImage;

        public void Select()
        {
            _selectedImage.gameObject.SetActive(true);
        }

        public void UnSelect()
        {
            _selectedImage.gameObject.SetActive(false);
        }
    }
}