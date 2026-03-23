using System;
using UnityEngine;
using UnityEngine.UI;

public class CreamButton : MonoBehaviour
{
    [SerializeField] private RectTransform pickupSourceRect;
    public RectTransform PickupSourceRect => pickupSourceRect;
    public RectTransform RectTransform => GetComponent<RectTransform>();
 
    public event Action OnCreamSelected;
 
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
            OnCreamSelected?.Invoke()
        );
    }
    
    public void Hide() => gameObject.SetActive(false);
    public void Show() => gameObject.SetActive(true);
}