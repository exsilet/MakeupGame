using UnityEngine;
using UnityEngine.UI;

public class MakeupCoordinator : MonoBehaviour
{
    [Header("Скрипты")] 
    [SerializeField] private MakeupStateMachine stateMachine;
    [SerializeField] private HandAnimator handAnimator;
    [SerializeField] private HandDragHandler dragHandler;
    [SerializeField] private HandToolVisual toolVisual;
    [SerializeField] private FaceZoneChecker faceZoneChecker;
    [SerializeField] private MakeupApplier makeupApplier;
    [SerializeField] private MakeupTabSwitcher tabSwitcher;
    [SerializeField] private UIDoneButton doneButton;

    [Header("Кнопки на полке")]
    [SerializeField] private CreamButton creamButton;
    [SerializeField] private Button loofahButton;

    [Header("Кисточки в сцене (скрываются когда рука берёт)")] 
    [SerializeField] private BrushItem eyeshadowBrushItem;
    [SerializeField] private BrushItem blushBrushItem;

    [Header("Кнопки теней и румян")]
    [SerializeField] private UIColorButton[] allColorButtons;

    [Header("Кнопки помады")]
    [SerializeField] private LipstickButton[] allLipstickButtons;

    [Header("Фиксированные точки нанесения")]
    [SerializeField] private Vector2 creamApplyPoint;
    [SerializeField] private Vector2 blushApplyPoint;
    [SerializeField] private Vector2 eyeshadowApplyPoint;
    [SerializeField] private Vector2 lipstickApplyPoint;

    private Sprite _selectedFaceSprite;
    private Color _selectedTipColor;

    private Vector2? _returnPosition;

    private CreamButton _hiddenCreamButton;
    private LipstickButton _hiddenLipstickButton;
    private BrushItem _hiddenBrushItem;
    private UIColorButton _hiddenColorButton;
    private BrushItem _pendingBrushItem;

    private void Awake()
    {
        creamButton.OnCreamSelected += OnCreamTapped;
        loofahButton.onClick.AddListener(OnLoofahTapped);

        foreach (var btn in allColorButtons)
            btn.OnColorSelected += OnColorButtonTapped;

        foreach (var btn in allLipstickButtons)
            btn.OnLipstickSelected += OnLipstickButtonTapped;

        handAnimator.OnAnimationFinished += OnAnimationFinished;
        handAnimator.OnDipCompleted += OnDipCompleted;
        handAnimator.OnPickupReached += OnPickupReached;

        dragHandler.OnDragEnd += OnDragEnded;

        stateMachine.OnStateChanged += OnStateChanged;
    }

    private void OnCreamTapped()
    {
        if (!stateMachine.CanInteract()) return;

        _hiddenCreamButton = creamButton;
        _returnPosition = GetPickupPointInHandSpace(creamButton.PickupSourceRect);

        stateMachine.SetTool(MakeupStateMachine.Tool.Cream);
        stateMachine.SetState(MakeupStateMachine.State.PickingUp);
        
        handAnimator.PlayPickUp(_returnPosition.Value);
    }

    private void OnLoofahTapped()
    {
        if (!stateMachine.CanInteract()) return;

        makeupApplier.ResetAll();
        doneButton.SetInactive();
    }

    private void OnColorButtonTapped(UIColorButton button)
    {
        if (!stateMachine.CanInteract()) return;

        _selectedFaceSprite = button.FaceSprite;
        _selectedTipColor = button.TipColor;

        MakeupStateMachine.Tool tool = button.ToolType;

        _pendingBrushItem = tool == MakeupStateMachine.Tool.Blush
            ? blushBrushItem
            : eyeshadowBrushItem;

        _hiddenBrushItem = _pendingBrushItem;
        _returnPosition = GetPickupPointInHandSpace(_pendingBrushItem.PickupSourceRect);

        stateMachine.SetTool(tool);
        stateMachine.SetState(MakeupStateMachine.State.PickingUp);

        Vector2 brushPickupPoint = GetPickupPointInHandSpace(_pendingBrushItem.PickupSourceRect);

        handAnimator.HandRect.anchoredPosition = brushPickupPoint;
        toolVisual.ShowTool(tool);

        Canvas.ForceUpdateCanvases();

        Vector2 dipPoint = GetPickupPointInHandSpace(button.BrushDipPoint);
        Vector2 tipOffset = toolVisual.GetBrushTipOffsetFromHand(tool, handAnimator.HandRect);
        Vector2 handTargetForDip = dipPoint - tipOffset;

        toolVisual.HideTool();

        handAnimator.PlayPickBrushAndDip(brushPickupPoint, handTargetForDip);
    }

    private void OnLipstickButtonTapped(LipstickButton button)
    {
        if (!stateMachine.CanInteract()) return;

        _hiddenLipstickButton = button;
        _selectedFaceSprite = button.FaceSprite;
        _returnPosition = GetPickupPointInHandSpace(button.PickupSourceRect);

        stateMachine.SetTool(MakeupStateMachine.Tool.Lipstick);
        stateMachine.SetState(MakeupStateMachine.State.PickingUp);
        
        handAnimator.PlayPickUp(_returnPosition.Value);
    }

    private void OnDipCompleted()
    {
        toolVisual.SetBrushTipColor(_selectedTipColor);
    }

    private void OnDragEnded(Vector2 screenPoint)
    {
        RectTransform faceRect = faceZoneChecker.FaceZone;

        Vector2 handLocalPoint = handAnimator.HandRect.anchoredPosition;

        bool inside =
            handLocalPoint.x >= faceRect.anchoredPosition.x - faceRect.rect.width * 0.5f &&
            handLocalPoint.x <= faceRect.anchoredPosition.x + faceRect.rect.width * 0.5f &&
            handLocalPoint.y >= faceRect.anchoredPosition.y - faceRect.rect.height * 0.5f &&
            handLocalPoint.y <= faceRect.anchoredPosition.y + faceRect.rect.height * 0.5f;

        if (!inside)
        {
            Cancel();
            return;
        }

        Vector2 applyPoint = handLocalPoint;

        switch (stateMachine.CurrentTool)
        {
            case MakeupStateMachine.Tool.Cream:
                applyPoint = creamApplyPoint;
                break;

            case MakeupStateMachine.Tool.Blush:
                applyPoint = blushApplyPoint;
                break;

            case MakeupStateMachine.Tool.Eyeshadow:
                applyPoint = eyeshadowApplyPoint;
                break;

            case MakeupStateMachine.Tool.Lipstick:
                applyPoint = lipstickApplyPoint;
                break;
        }

        stateMachine.SetState(MakeupStateMachine.State.Applying);

        switch (stateMachine.CurrentTool)
        {
            case MakeupStateMachine.Tool.Cream:
                handAnimator.PlayApplyCream(applyPoint);
                break;

            case MakeupStateMachine.Tool.Eyeshadow:
                handAnimator.PlayApplyEyeshadow(applyPoint);
                break;

            case MakeupStateMachine.Tool.Blush:
                handAnimator.PlayApplyBlush(applyPoint);
                break;

            case MakeupStateMachine.Tool.Lipstick:
                handAnimator.PlayApplyLipstick(applyPoint);
                break;
        }
    }
    
    private void OnPickupReached()
    {
        switch (stateMachine.CurrentTool)
        {
            case MakeupStateMachine.Tool.Cream:
                _hiddenCreamButton?.Hide();
                toolVisual.ShowTool(MakeupStateMachine.Tool.Cream);
                break;

            case MakeupStateMachine.Tool.Lipstick:
                _hiddenLipstickButton?.Hide();
                if (_hiddenLipstickButton != null)
                    toolVisual.ShowLipstick(_hiddenLipstickButton.TubeSprite);
                break;

            case MakeupStateMachine.Tool.Blush:
                _pendingBrushItem?.Hide();
                toolVisual.ShowTool(MakeupStateMachine.Tool.Blush);
                break;

            case MakeupStateMachine.Tool.Eyeshadow:
                _pendingBrushItem?.Hide();
                toolVisual.ShowTool(MakeupStateMachine.Tool.Eyeshadow);
                break;
        }
    }

    private void Cancel()
    {
        stateMachine.SetState(MakeupStateMachine.State.Returning);

        Vector2 returnTo = _returnPosition ?? handAnimator.HandRect.anchoredPosition;

        handAnimator.PlayCancelToDefault(returnTo, () =>
        {
            ShowHiddenItems();
            toolVisual.HideTool();
            stateMachine.Reset();
        });
    }

    private void OnAnimationFinished()
    {
        switch (stateMachine.CurrentState)
        {
            case MakeupStateMachine.State.PickingUp:
                stateMachine.SetState(MakeupStateMachine.State.Carrying);
                break;

            case MakeupStateMachine.State.Applying:
                ApplyCurrentMakeup();
                stateMachine.SetState(MakeupStateMachine.State.Returning);

                if (_returnPosition.HasValue)
                {
                    handAnimator.PlayReturn(_returnPosition.Value, () =>
                    {
                        ShowHiddenItems();
                        toolVisual.HideTool();
                    });
                }
                else
                {
                    handAnimator.PlayReturnToDefault();
                }
                break;

            case MakeupStateMachine.State.Returning:
                ShowHiddenItems();
                toolVisual.HideTool();
                stateMachine.Reset();
                break;
        }
    }

    private void OnStateChanged(MakeupStateMachine.State state)
    {
        if (state == MakeupStateMachine.State.Carrying)
            dragHandler.EnableDrag();
        else
            dragHandler.DisableDrag();
    }

    private void ApplyCurrentMakeup()
    {
        switch (stateMachine.CurrentTool)
        {
            case MakeupStateMachine.Tool.Cream:
                makeupApplier.ApplyCream();
                break;
            case MakeupStateMachine.Tool.Eyeshadow:
                makeupApplier.ApplyEyeshadow(_selectedFaceSprite);
                break;
            case MakeupStateMachine.Tool.Lipstick:
                makeupApplier.ApplyLipstick(_selectedFaceSprite);
                break;
            case MakeupStateMachine.Tool.Blush:
                makeupApplier.ApplyBlush(_selectedFaceSprite);
                break;
        }

        if (makeupApplier.HasAnyMakeup())
            doneButton.SetActive();
    }
    
    private Vector2 GetPickupPointInHandSpace(RectTransform sourceRect)
    {
        RectTransform handParent = handAnimator.HandRect.parent as RectTransform;

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, sourceRect.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            handParent,
            screenPoint,
            null,
            out Vector2 localPoint
        );

        return localPoint;
    }

    private void ShowHiddenItems()
    {
        _hiddenCreamButton?.Show();
        _hiddenLipstickButton?.Show();
        _hiddenBrushItem?.Show();

        _hiddenCreamButton = null;
        _hiddenLipstickButton = null;
        _hiddenBrushItem = null;
    }
}