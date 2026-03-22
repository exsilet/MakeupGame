using System;
using UnityEngine;
using UnityEngine.UI;

public class UIColorButton : MonoBehaviour
{
    [SerializeField] private Sprite colorSprite;
    [SerializeField] private Color tipColor;

    public RectTransform RectTransform => GetComponent<RectTransform>();
    public Sprite FaceSprite => colorSprite;
    public Color TipColor => tipColor;

    public event Action<UIColorButton> OnColorSelected;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
            OnColorSelected?.Invoke(this)
        );
    }
}