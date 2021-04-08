using UnityEngine;
using UnityEngine.UI;
using static SerializeDataClasses;

public class PropertiesUi : MonoBehaviour
{
    [SerializeField] private Toggle attachToEntity;
    public void SetAttachToEntityToggleCallback(JsonData jsonData)
    {
        attachToEntity.onValueChanged.AddListener(toggle => { jsonData.attachToEntity = toggle; });
    }
}