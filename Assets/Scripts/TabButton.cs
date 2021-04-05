using UnityEngine;
using UnityEngine.UI;

public class TabButton : Button
{
    private UiManager uiManager;
    private Image btnImg;
    private Text btnName;

    protected override void Awake()
    {
        base.Awake();
        uiManager = GetComponentInParent<UiManager>();
        btnName = GetComponentInChildren<Text>();
        btnImg = GetComponent<Image>();
    }

    public void ToggleHighlight(bool on)
    {
        btnImg.color = on ? Color.cyan : Color.white;
    }
    
    public void OnClickTabBtn()
    {
        uiManager.OnTabButtonClick(this);
    }

    public void ChangeName(string name)
    {
        btnName.text = name;
    }
}