using UnityEngine;

/// <summary>
/// Отвечает ТОЛЬКО за связи между скриптами через события.
/// </summary>
public class MakeupCoordinator : MonoBehaviour
{
    [Header("Скрипты")]
    [SerializeField] private MakeupStateMachine stateMachine;
    [SerializeField] private HandAnimator        handAnimator;
    [SerializeField] private HandDragHandler     dragHandler;
    [SerializeField] private HandToolVisual      toolVisual;
    [SerializeField] private FaceZoneChecker     faceZoneChecker;
    [SerializeField] private MakeupApplier       makeupApplier;
    [SerializeField] private MakeupTabSwitcher   tabSwitcher;
    [SerializeField] private UIDoneButton        doneButton;

    [Header("Кнопки на полке")]
    [SerializeField] private CreamButton           creamButton;
    [SerializeField] private UnityEngine.UI.Button loofahButton;

    [Header("Кисточки в сцене (скрываются когда рука берёт)")]
    [SerializeField] private BrushItem eyeshadowBrushItem;
    [SerializeField] private BrushItem blushBrushItem;

    [Header("Кнопки теней и румян")]
    [SerializeField] private UIColorButton[] allColorButtons;

    [Header("Кнопки помады")]
    [SerializeField] private LipstickButton[] allLipstickButtons;

    // Данные текущего действия
    private Sprite _selectedFaceSprite;
    private Color  _selectedTipColor;

    // Позиция возврата (null = дефолт)
    private Vector2? _returnPosition;

    // Скрытые предметы
    private CreamButton    _hiddenCreamButton;
    private LipstickButton _hiddenLipstickButton;
    private BrushItem      _hiddenBrushItem;

    // =====================
    // ИНИЦИАЛИЗАЦИЯ
    // =====================
    private void Awake()
    {
        creamButton.OnCreamSelected += OnCreamTapped;
        loofahButton.onClick.AddListener(OnLoofahTapped);

        foreach (var btn in allColorButtons)
            btn.OnColorSelected += OnColorButtonTapped;

        foreach (var btn in allLipstickButtons)
            btn.OnLipstickSelected += OnLipstickButtonTapped;

        handAnimator.OnAnimationFinished += OnAnimationFinished;
        handAnimator.OnDipCompleted      += OnDipCompleted;

        dragHandler.OnDragEnd += OnDragEnded;

        stateMachine.OnStateChanged += OnStateChanged;
    }

    // =====================
    // КРЕМ
    // =====================
    private void OnCreamTapped()
    {
        if (!stateMachine.CanInteract()) return;

        _hiddenCreamButton = creamButton;
        _returnPosition    = creamButton.RectTransform.anchoredPosition;

        creamButton.Hide();

        stateMachine.SetTool(MakeupStateMachine.Tool.Cream);
        stateMachine.SetState(MakeupStateMachine.State.PickingUp);

        toolVisual.ShowTool(MakeupStateMachine.Tool.Cream);
        handAnimator.PlayPickUp(_returnPosition.Value);
    }

    // =====================
    // СПОНЖИК
    // =====================
    private void OnLoofahTapped()
    {
        if (!stateMachine.CanInteract()) return;

        makeupApplier.ResetAll();
        doneButton.SetInactive();
    }

    // =====================
    // ТЕНИ И РУМЯНА
    // =====================
    private void OnColorButtonTapped(UIColorButton button)
    {
        if (!stateMachine.CanInteract()) return;

        MakeupStateMachine.Tool tool = tabSwitcher.ActiveTool;
        if (tool != MakeupStateMachine.Tool.Eyeshadow &&
            tool != MakeupStateMachine.Tool.Blush) return;

        _selectedFaceSprite = button.FaceSprite;
        _selectedTipColor   = button.TipColor;
        _returnPosition     = null; // кисть уходит в дефолт

        // Скрываем нужную кисточку из сцены
        _hiddenBrushItem = tool == MakeupStateMachine.Tool.Eyeshadow
            ? eyeshadowBrushItem
            : blushBrushItem;
        _hiddenBrushItem?.Hide();

        stateMachine.SetTool(tool);
        stateMachine.SetState(MakeupStateMachine.State.PickingUp);

        toolVisual.ShowTool(tool);
        handAnimator.PlayDipIntoPalette(button.RectTransform.anchoredPosition);
    }

    // =====================
    // ПОМАДА
    // =====================
    private void OnLipstickButtonTapped(LipstickButton button)
    {
        if (!stateMachine.CanInteract()) return;

        _hiddenLipstickButton = button;
        _selectedFaceSprite   = button.FaceSprite;
        _returnPosition       = button.RectTransform.anchoredPosition;

        button.Hide();

        stateMachine.SetTool(MakeupStateMachine.Tool.Lipstick);
        stateMachine.SetState(MakeupStateMachine.State.PickingUp);

        toolVisual.ShowLipstick(button.TubeSprite);
        handAnimator.PlayPickUp(_returnPosition.Value);
    }

    // =====================
    // КИСТЬ ОКУНУЛИ
    // =====================
    private void OnDipCompleted()
    {
        toolVisual.SetBrushTipColor(_selectedTipColor);
    }

    // =====================
    // DRAG ЗАВЕРШЁН
    // =====================
    private void OnDragEnded(Vector2 screenPoint)
    {
        if (!faceZoneChecker.IsInsideFaceZone(screenPoint))
        {
            // Игрок отпустил вне зоны лица — отмена, возвращаем всё
            Cancel();
            return;
        }

        // Переводим screenPoint в anchoredPosition
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            handAnimator.HandRect.parent as RectTransform,
            screenPoint,
            null,
            out Vector2 localPoint
        );

        stateMachine.SetState(MakeupStateMachine.State.Applying);

        switch (stateMachine.CurrentTool)
        {
            case MakeupStateMachine.Tool.Cream:
                handAnimator.PlayApplyCream(localPoint);
                break;
            case MakeupStateMachine.Tool.Eyeshadow:
                handAnimator.PlayApplyEyeshadow(localPoint);
                break;
            case MakeupStateMachine.Tool.Blush:
                handAnimator.PlayApplyBlush(localPoint);
                break;
            case MakeupStateMachine.Tool.Lipstick:
                handAnimator.PlayApplyLipstick(localPoint);
                break;
        }
    }

    // =====================
    // ОТМЕНА (отпустил вне зоны лица)
    // =====================
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

    // =====================
    // АВТОАНИМАЦИЯ ЗАВЕРШЕНА
    // =====================
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
                    handAnimator.PlayReturn(_returnPosition.Value);
                else
                    handAnimator.PlayReturnToDefault();
                break;

            case MakeupStateMachine.State.Returning:
                ShowHiddenItems();
                toolVisual.HideTool();
                stateMachine.Reset();
                break;
        }
    }

    // =====================
    // СМЕНА СОСТОЯНИЯ
    // =====================
    private void OnStateChanged(MakeupStateMachine.State state)
    {
        if (state == MakeupStateMachine.State.Carrying)
            dragHandler.EnableDrag();
        else
            dragHandler.DisableDrag();
    }

    // =====================
    // ПРИМЕНЕНИЕ МАКИЯЖА
    // =====================
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

    // =====================
    // ПОКАЗАТЬ СКРЫТЫЕ ПРЕДМЕТЫ
    // =====================
    private void ShowHiddenItems()
    {
        _hiddenCreamButton?.Show();
        _hiddenLipstickButton?.Show();
        _hiddenBrushItem?.Show();

        _hiddenCreamButton    = null;
        _hiddenLipstickButton = null;
        _hiddenBrushItem      = null;
    }
}