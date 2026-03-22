using System;
using UnityEngine;
using UnityEngine.UI;

public class MakeupTabSwitcher : MonoBehaviour
{
    [Serializable]
    public class Tab
    {
        public Button button;
        public Image icon;
        public Sprite activeSprite;
        public Sprite inactiveSprite;
        public GameObject colorGrid;
        public MakeupStateMachine.Tool tool;
        
        public RectOffset activeOffsets;
        public RectOffset inactiveOffsets;
        
        public GameObject brushPreview;
    }
    
    [SerializeField] private Tab[] tabs;
    
    public event Action<MakeupStateMachine.Tool> OnTabSelected;

    private int _activeIndex = 0;

    public MakeupStateMachine.Tool ActiveTool => tabs[_activeIndex].tool;

    private void Start()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            int index = i;
            tabs[i].button.onClick.AddListener(() => SelectTab(index));
            
            if (tabs[i].brushPreview != null)
            {
                tabs[i].brushPreview.SetActive(false);
            }
        }

        SelectTab(0);
    }

    private void SelectTab(int index)
    {
        if (index < 0 || index >= tabs.Length) return;

        _activeIndex = index;

        for (int i = 0; i < tabs.Length; i++)
        {
            bool isActive = (i == index);
            tabs[i].icon.sprite = isActive
                ? tabs[i].activeSprite
                : tabs[i].inactiveSprite;
            tabs[i].colorGrid.SetActive(isActive);
            
            if (tabs[i].brushPreview != null) 
                tabs[i].brushPreview.SetActive(isActive);
            
            
            RectTransform rectTransform = tabs[i].button.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                if (isActive && tabs[i].activeOffsets != null)
                {
                    rectTransform.offsetMin = new Vector2(
                        tabs[i].activeOffsets.left,
                        tabs[i].activeOffsets.bottom
                    );
                    rectTransform.offsetMax = new Vector2(
                        -tabs[i].activeOffsets.right,
                        -tabs[i].activeOffsets.top
                    );
                }
                else if (!isActive && tabs[i].inactiveOffsets != null)
                {
                    rectTransform.offsetMin = new Vector2(
                        tabs[i].inactiveOffsets.left,
                        tabs[i].inactiveOffsets.bottom
                    );
                    rectTransform.offsetMax = new Vector2(
                        -tabs[i].inactiveOffsets.right,
                        -tabs[i].inactiveOffsets.top
                    );
                }
            }
        }

        OnTabSelected?.Invoke(tabs[_activeIndex].tool);
    }
}