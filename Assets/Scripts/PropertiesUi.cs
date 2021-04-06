using UnityEngine;
using UnityEngine.UI;

public class PropertiesUi : MonoBehaviour
{
    [SerializeField] private Toggle attachToEntity;
    public void SetAttachToEntityToggleCallback(JobjectContainer jobjectContainer)
    {
        attachToEntity.onValueChanged.AddListener(toggle => { jobjectContainer.attachToEntity = toggle; });
    }
}