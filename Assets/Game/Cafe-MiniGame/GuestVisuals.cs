using UnityEngine;
using TMPro;

public class GuestVisuals : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject speechBubbleCanvas;
    public TextMeshProUGUI speechText;

    public void SetupSpeechBubble(GuestData data)
    {
        if (speechText != null && data != null)
        {
            speechText.text = $"I like {data.Likes[0]} & {data.Likes[1]}\nI hate {data.Dislike}";
        }
    }

    public void HideSpeechBubble()
    {
        if (speechBubbleCanvas != null)
        {
            speechBubbleCanvas.SetActive(false);
        }
    }
}