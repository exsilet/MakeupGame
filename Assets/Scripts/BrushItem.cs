using UnityEngine;

public class BrushItem : MonoBehaviour
{
    public void Hide() => gameObject.SetActive(false);
    public void Show() => gameObject.SetActive(true);
}