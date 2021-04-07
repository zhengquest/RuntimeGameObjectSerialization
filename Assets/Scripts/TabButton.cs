using UnityEngine;
using UnityEngine.UI;

public class TabButton : Button
{
    private CreatorUiManager creatorUiManager;
    private Image btnImg;
    private Text btnName;

    protected override void Awake()
    {
        base.Awake();
        creatorUiManager = GetComponentInParent<CreatorUiManager>();
        btnName = GetComponentInChildren<Text>();
        btnImg = GetComponent<Image>();
    }

    public void ToggleHighlight(bool on)
    {
        btnImg.color = on ? Color.cyan : Color.white;
    }
    
    public void OnClickTabBtn()
    {
        creatorUiManager.OnTabButtonClick(this);
    }

    public void ChangeName(string name)
    {
        btnName.text = name;
    }
}