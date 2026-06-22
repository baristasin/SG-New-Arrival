using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GuestVisuals : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject sittingBubbleCanvas;
    public TextMeshProUGUI sittingSpeechText;

    void Start()
    {
        sittingBubbleCanvas.SetActive(false);
    }
    public void SetupSpeechBubble(GuestData data)
    {
        if (sittingBubbleCanvas != null && data != null)
        {
            //Get those Names
            string spriteLike1 = GetSpriteAssetName(data.Likes[0]);
            string spriteLike2 = GetSpriteAssetName(data.Likes[1]);
            string spriteDislike = GetSpriteAssetName(data.Dislike);

            sittingSpeechText.text =    $"<sprite=\"{spriteLike1}\" index=0> & <sprite=\"{spriteLike2}\" index=0>\n" + $"<sprite=\"{spriteDislike}\" index=0>";
        }
    }

    //Helping Method to set the strings
    private string GetSpriteAssetName(GuestData.InterestType interest)
    {
        switch (interest)
        {
            case GuestData.InterestType.Books:      return "icons8-offenes-buch-64";
            case GuestData.InterestType.Boardgames: return "icons8-joystick-64";
            case GuestData.InterestType.Music:      return "icons8-musiknoten-64";
            case GuestData.InterestType.Beer:       return "icons8-bier-64";
            default:                                return "";
        }
    }
    public void HideSpeechBubble()
    {
        sittingBubbleCanvas.SetActive(true);
    }
}