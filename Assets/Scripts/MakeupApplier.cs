using UnityEngine;
using UnityEngine.UI;

public class MakeupApplier : MonoBehaviour
{
    [Header("Overlay слои персонажа")]
    [SerializeField] private GameObject acneOverlay;
    [SerializeField] private Image eyeshadowOverlay;
    [SerializeField] private Image lipsOverlay;
    [SerializeField] private Image blushOverlay;

    public void ApplyCream()
    {
        if (acneOverlay != null)
            acneOverlay.SetActive(false);
    }

    public void ApplyEyeshadow(Sprite colorSprite)
    {
        if (eyeshadowOverlay == null || colorSprite == null) return;
        eyeshadowOverlay.sprite = colorSprite;
        eyeshadowOverlay.gameObject.SetActive(true);
    }

    public void ApplyLipstick(Sprite colorSprite)
    {
        if (lipsOverlay == null || colorSprite == null) return;
        lipsOverlay.sprite = colorSprite;
        lipsOverlay.gameObject.SetActive(true);
    }

    public void ApplyBlush(Sprite colorSprite)
    {
        if (blushOverlay == null || colorSprite == null) return;
        blushOverlay.sprite = colorSprite;
        blushOverlay.gameObject.SetActive(true);
    }

    public void ResetAll()
    {
        if (acneOverlay != null) acneOverlay.SetActive(true);
        if (eyeshadowOverlay != null) eyeshadowOverlay.gameObject.SetActive(false);
        if (lipsOverlay != null) lipsOverlay.gameObject.SetActive(false);
        if (blushOverlay != null) blushOverlay.gameObject.SetActive(false);
    }

    public bool HasAnyMakeup()
    {
        bool eyeOn = eyeshadowOverlay != null && eyeshadowOverlay.gameObject.activeSelf;
        bool lipsOn = lipsOverlay != null && lipsOverlay.gameObject.activeSelf;
        bool blushOn = blushOverlay != null && blushOverlay.gameObject.activeSelf;
        bool acneOff = acneOverlay != null && !acneOverlay.activeSelf;
        return eyeOn || lipsOn || blushOn || acneOff;
    }
}