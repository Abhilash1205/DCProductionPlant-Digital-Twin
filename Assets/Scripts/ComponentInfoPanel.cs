using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class ComponentInfoPanel : MonoBehaviour
{
    public static ComponentInfoPanel Instance;

    public TMP_Text titleText;
    public TMP_Text descText;
    public Image previewImage;

    public GameObject ecadSection;
    public Image ecadImage;

    public GameObject panelRoot;

    private ComponentMetadata currentMetadata;

    void Awake()
    {
        Instance = this;
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    public void ShowInfo(string id)
    {
        StartCoroutine(LoadMetadataAndShow(id));
    }

    IEnumerator LoadMetadataAndShow(string id)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "ComponentMetadata/cabinetData.json");

        UnityWebRequest www = UnityWebRequest.Get(path);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load metadata: " + www.error);
            yield break;
        }

        string json = www.downloadHandler.text;
        ComponentMetadataList data = JsonUtility.FromJson<ComponentMetadataList>(json);

        foreach (var item in data.components)
        {
            if (item.id == id)
            {
                currentMetadata = item;
                titleText.text = item.name;
                descText.text = item.description;
                StartCoroutine(LoadImage(item.imagePath, previewImage));

            

                panelRoot.SetActive(true);
                yield break;
            }
        }

        Debug.LogWarning("Component ID not found: " + id);
    }

    IEnumerator LoadImage(string relPath, Image targetImage)
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, "ComponentMetadata", relPath);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(fullPath);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning("Failed to load image: " + fullPath);
            yield break;
        }

        Texture2D tex = DownloadHandlerTexture.GetContent(www);
        targetImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    public void ClosePanel()
    {
        panelRoot.SetActive(false);
    }
}