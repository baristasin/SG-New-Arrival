using Game.Scripts.AudioModules;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    // Drop this on any Button to make it play the global UI click sound on press. Wires itself
    // up in Awake — no Inspector setup needed beyond adding the component. The sound lives on
    // UIClickSounds (persistent singleton); change it there to change all buttons at once.
    [RequireComponent(typeof(Button))]
    public class ButtonClickSfx : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
                UIClickSounds.Instance?.PlayClick());
        }
    }
}
