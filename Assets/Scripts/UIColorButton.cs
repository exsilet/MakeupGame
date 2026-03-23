using System;
using UnityEngine;
using UnityEngine.UI;

public class UIColorButton : MonoBehaviour
{
    [SerializeField] private Sprite _colorSprite;
    [SerializeField] private Color _tipColor;
    [SerializeField] private Sprite _faceSprite;
    [SerializeField] private RectTransform _pickupSourceRect;
    [SerializeField] private MakeupStateMachine.Tool _toolType;
    public MakeupStateMachine.Tool ToolType => _toolType;
    public RectTransform PickupSourceRect => _pickupSourceRect;
    public RectTransform RectTransform => GetComponent<RectTransform>();
    public Sprite FaceSprite => _faceSprite;
    public Color TipColor => _tipColor;

    public event Action<UIColorButton> OnColorSelected;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
            OnColorSelected?.Invoke(this)
        );
    }
}