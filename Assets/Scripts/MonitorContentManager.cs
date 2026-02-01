using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonitorContentManager : MonoBehaviour
{
[System.Serializable] public class FileItem { public string title; public string filename; }
[System.Serializable] public class Category { public string name; public string folder; public List<FileItem> files; }
[System.Serializable] public class MonitorIndex { public List<Category> categories; }
public Button btnOpenMonitor;
public Button btnCloseMonitor;
public Button btnResearch;
public Button btnECAD;

public GameObject fileButtonPrefab;
public Transform contentParent;
public GameObject monitorPanelRoot;

private MonitorIndex monitorData;

void Start()
{
    monitorPanelRoot.SetActive(false);
    btnOpenMonitor.gameObject.SetActive(true);

    LoadMonitorData();

    btnOpenMonitor.onClick.AddListener(OpenMonitor);
    btnCloseMonitor.onClick.AddListener(CloseMonitor);

    btnResearch.onClick.AddListener(() => ShowCategory("Research Papers"));
    btnECAD.onClick.AddListener(() => ShowCategory("ECAD Files"));

    ShowCategory("Research Papers");
}

void LoadMonitorData()
{
    string path = Path.Combine(Application.streamingAssetsPath, "MonitorContent/monitorIndex.json");
    if (!File.Exists(path))
    {
        Debug.LogError("monitorIndex.json not found!");
        return;
    }

    string json = File.ReadAllText(path);
    monitorData = JsonUtility.FromJson<MonitorIndex>(json);
}

void ShowCategory(string categoryName)
{
    ClearPreviousFiles();

    Category category = monitorData.categories.Find(c => c.name == categoryName);
    if (category == null)
    {
        Debug.LogWarning("Category not found: " + categoryName);
        return;
    }

    foreach (var file in category.files)
    {
        GameObject btnObj = Instantiate(fileButtonPrefab, contentParent);
        btnObj.GetComponentInChildren<TMP_Text>().text = file.title;

        Button btn = btnObj.GetComponent<Button>();
        string filePath = Path.Combine(Application.streamingAssetsPath, "MonitorContent", category.folder, file.filename);
        btn.onClick.AddListener(() => OnFileClicked(filePath));
    }
}

void OnFileClicked(string path)
{
    Debug.Log("Opening externally: " + path);
    Application.OpenURL("file://" + path);
}

void ClearPreviousFiles()
{
    foreach (Transform child in contentParent)
    {
        Destroy(child.gameObject);
    }
}

public void OpenMonitor()
{
    monitorPanelRoot.SetActive(true);
    btnOpenMonitor.gameObject.SetActive(false);
}

public void CloseMonitor()
{
    monitorPanelRoot.SetActive(false);
    btnOpenMonitor.gameObject.SetActive(true);
    ClearPreviousFiles();
}
}
