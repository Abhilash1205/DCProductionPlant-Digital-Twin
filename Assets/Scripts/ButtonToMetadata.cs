using UnityEngine;
using UnityEngine.UI;

public class ButtonToMetadata : MonoBehaviour
{
    public string componentId; 

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
    }

    void OnClick()
    {
        Debug.Log("Clicked button with ID: " + componentId);
        if (!string.IsNullOrEmpty(componentId) && ComponentInfoPanel.Instance != null)
        {
            ComponentInfoPanel.Instance.ShowInfo(componentId);
        }
        else
        {
            Debug.LogWarning($"[ButtonToMetadata] Missing componentId or ComponentInfoPanel not initialized: {gameObject.name}");
        }
        
    }
}