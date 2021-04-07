using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestModeUiManager : MonoBehaviour
{
    public EntityManager entityManager;
    public TabButton behaviourButtonUi;
    public RectTransform behaviourTab;
    public Canvas canvas;
    public Camera mainCam;
    public Button createModeBtn;
    
    private SavedEntity selectedEntity;
    private List<TabButton> selectObjectsBtn;

    public void CreateUiForCustomEntities(IEnumerable<SavedEntity> savedEntites, Action OnEnterCreateMode)
    {
        selectObjectsBtn = new List<TabButton>();
        createModeBtn.onClick.AddListener(() =>
        {
            OnEnterCreateMode?.Invoke();
            OnLeaveTestMode();
        });
        
        foreach (var savedEntity in savedEntites)
        {
            var newSelectBtn = Instantiate(behaviourButtonUi, behaviourTab);
            newSelectBtn.onClick.AddListener(() =>
            {
                foreach (var button in selectObjectsBtn)
                {
                    button.ToggleHighlight(newSelectBtn == button);
                }
                
                selectedEntity = savedEntity;
            });
            
            newSelectBtn.ChangeName(savedEntity.inGameObject.SubObjectName);
            selectObjectsBtn.Add(newSelectBtn);
        }
    }

    public void OnLeaveTestMode()
    {
        if (selectObjectsBtn != null)
        {
            foreach (var button in selectObjectsBtn)
            {
                Destroy(button.gameObject);
            }
        }
        
        createModeBtn.onClick.RemoveAllListeners();
        ToggleVisible(false);
        selectedEntity = null;
    }

    public void ToggleVisible(bool toggle)
    {
        canvas.enabled = toggle;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canvas.enabled && selectedEntity != null)
        {
            var raycast = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(raycast, out var hit, 100f))
            {
                if (hit.transform.CompareTag("Floor"))
                {
                    var spawnPoint = new Vector3(hit.point.x, hit.point.y + 1f, hit.point.z);
                    entityManager.SpawnEntityAsync(selectedEntity, spawnPoint);
                }
            }
        }
    }
}