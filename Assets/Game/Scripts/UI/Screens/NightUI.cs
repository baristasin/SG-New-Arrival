using Game.Scripts.UI;
using UnityEngine;

namespace Game.Scripts.UI.Screens
{
    // Night combat HUD. Children: player health bar, building health bar, BottomGunList. The
    // screen itself just wraps Show/Hide; the gameplay HUDs inside drive their own values.
    public class NightUI : UIScreen
    {
        // Optional child references — fill in slice 6 once we know what the night combat layer
        // needs to query (e.g. resetting health bar on a new night).
    }
}
