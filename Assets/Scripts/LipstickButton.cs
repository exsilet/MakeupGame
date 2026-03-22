using System;
using UnityEngine;
using UnityEngine.UI;

public class LipstickButton  : MonoBehaviour
{
    [SerializeField] private Sprite tubeSprite;
    [SerializeField] private Sprite faceSprite;

    public RectTransform RectTransform => GetComponent<RectTransform>();
    public Sprite TubeSprite => tubeSprite;
    public Sprite FaceSprite => faceSprite;

    public event Action<LipstickButton> OnLipstickSelected;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
            OnLipstickSelected?.Invoke(this)
        );
    }
    
    public void Hide() => gameObject.SetActive(false);
    public void Show() => gameObject.SetActive(true);
}