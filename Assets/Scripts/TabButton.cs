using UnityEngine;
using UnityEngine.UI;

public class TabButton : Button
{
    private UiManager uiManager;

    protected override void Start()
    {
        base.Start();
        uiManager =GetComponentInParent<UiManager>();
    }

    public void OnClickTabBtn()
    {
        uiManager.OnTabButtonClick(this);
    }
}