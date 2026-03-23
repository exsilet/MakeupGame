using UnityEngine;
using UnityEngine.UI;

public class HandToolVisual : MonoBehaviour
{
    [Header("Компоненты руки")]
    [SerializeField] private Image toolImage;

    [Header("Кисточки (отдельные объекты)")] 
    [SerializeField] private GameObject blushBrushObject;
    [SerializeField] private GameObject eyeshadowBrushObject;

    [Header("Кончики кисточек")]
    [SerializeField] private Image blushBrushTip;
    [SerializeField] private Image eyeshadowBrushTip;

    [Header("Спрайты инструментов")] 
    [SerializeField] private Sprite creamSprite;

    private MakeupStateMachine.Tool _currentTool;

    public RectTransform BlushBrushRect =>
        blushBrushObject != null ? blushBrushObject.GetComponent<RectTransform>() : null;

    public RectTransform EyeshadowBrushRect =>
        eyeshadowBrushObject != null ? eyeshadowBrushObject.GetComponent<RectTransform>() : null;

    public RectTransform BlushBrushTipRect =>
        blushBrushTip != null ? blushBrushTip.GetComponent<RectTransform>() : null;

    public RectTransform EyeshadowBrushTipRect =>
        eyeshadowBrushTip != null ? eyeshadowBrushTip.GetComponent<RectTransform>() : null;
    
    public void ShowTool(MakeupStateMachine.Tool tool)
    {
        _currentTool = tool;

        HideAllBrushes();
        toolImage.gameObject.SetActive(false);

        switch (tool)
        {
            case MakeupStateMachine.Tool.Cream:
                toolImage.gameObject.SetActive(true);
                toolImage.sprite = creamSprite;
                break;

            case MakeupStateMachine.Tool.Blush:
                if (blushBrushObject != null)
                    blushBrushObject.SetActive(true);
                break;

            case MakeupStateMachine.Tool.Eyeshadow:
                if (eyeshadowBrushObject != null)
                    eyeshadowBrushObject.SetActive(true);
                break;
        }
    }

    public void ShowLipstick(Sprite tubeSprite)
    {
        _currentTool = MakeupStateMachine.Tool.Lipstick;

        HideAllBrushes();

        toolImage.gameObject.SetActive(true);
        toolImage.sprite = tubeSprite;
    }

    public void SetBrushTipColor(Color color)
    {
        switch (_currentTool)
        {
            case MakeupStateMachine.Tool.Blush:
                if (blushBrushTip != null)
                    blushBrushTip.color = color;
                break;
            case MakeupStateMachine.Tool.Eyeshadow:
                if (eyeshadowBrushTip != null)
                    eyeshadowBrushTip.color = color;
                break;
        }
    }

    public Vector2 GetBrushTipOffsetFromHand(MakeupStateMachine.Tool tool, RectTransform handRect)
    {
        RectTransform tip = null;

        switch (tool)
        {
            case MakeupStateMachine.Tool.Blush:
                tip = BlushBrushTipRect;
                break;
            case MakeupStateMachine.Tool.Eyeshadow:
                tip = EyeshadowBrushTipRect;
                break;
        }

        if (tip == null || handRect == null || handRect.parent == null)
            return Vector2.zero;

        RectTransform parentRect = handRect.parent as RectTransform;
        if (parentRect == null)
            return Vector2.zero;

        Vector2 handLocalPoint;
        Vector2 tipLocalPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            RectTransformUtility.WorldToScreenPoint(null, handRect.position),
            null,
            out handLocalPoint
        );

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            RectTransformUtility.WorldToScreenPoint(null, tip.position),
            null,
            out tipLocalPoint
        );

        return tipLocalPoint - handLocalPoint;
    }

    public void HideTool()
    {
        toolImage.gameObject.SetActive(false);
        HideAllBrushes();

        if (blushBrushTip != null) blushBrushTip.color = Color.white;
        if (eyeshadowBrushTip != null) eyeshadowBrushTip.color = Color.white;
    }

    private void HideAllBrushes()
    {
        if (blushBrushObject != null) blushBrushObject.SetActive(false);
        if (eyeshadowBrushObject != null) eyeshadowBrushObject.SetActive(false);
    }
}