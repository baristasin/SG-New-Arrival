using UnityEngine;
using TMPro;

public class GuestVisuals : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject speechBubbleCanvas;
    public GameObject sittingBubbleCanvas;

    public TextMeshProUGUI speechText;
    public TextMeshProUGUI sittingSpeechText;

    void Start()
    {
        speechBubbleCanvas.SetActive(true);
        sittingBubbleCanvas.SetActive(false);
    }
    public void SetupSpeechBubble(GuestData data)
    {
        if (speechText != null && data != null)
        {
            //Get those Names
            string spriteLike1 = GetSpriteAssetName(data.Likes[0]);
            string spriteLike2 = GetSpriteAssetName(data.Likes[1]);
            string spriteDislike = GetSpriteAssetName(data.Dislike);

            // SpeechBubble Text
            speechText.text =           $"Like: <sprite=\"{spriteLike1}\" index=0> & <sprite=\"{spriteLike2}\" index=0>\n" +
                                        $"Hate: <sprite=\"{spriteDislike}\" index=0>";
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
        if (speechBubbleCanvas != null)
        {
            speechBubbleCanvas.SetActive(false);
            sittingBubbleCanvas.SetActive(true);
        }
    }
}