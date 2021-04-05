using UnityEngine;
using UnityEngine.UI;

public class TabButton : Button
{
    private UiManager uiManager;
    private Image btnImg;

    protected override void Start()
    {
        base.Start();
        uiManager = GetComponentInParent<UiManager>();
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
}