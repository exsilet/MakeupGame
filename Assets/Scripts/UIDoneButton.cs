using UnityEngine;
using UnityEngine.UI;

public class UIDoneButton : MonoBehaviour
{
    [SerializeField] private Image  buttonImage;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;

    private void Awake()
    {
        SetInactive();
    }

    public void SetActive()
    {
        buttonImage.sprite = activeSprite;
    }

    public void SetInactive()
    {
        buttonImage.sprite = inactiveSprite;
    }
}