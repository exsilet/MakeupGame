using UnityEngine;

public class BrushItem : MonoBehaviour
{
    [SerializeField] private RectTransform pickupSourceRect;
    public RectTransform PickupSourceRect => pickupSourceRect;
    public void Hide() => gameObject.SetActive(false);
    public void Show() => gameObject.SetActive(true);
}