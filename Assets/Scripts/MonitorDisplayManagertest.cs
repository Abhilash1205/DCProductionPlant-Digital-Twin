using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using System.Linq; 


public class MonitorDisplayManagertest : MonoBehaviour
{
    
    [Header("UI Control Buttons")]
    public Button btnOpenMonitor;
    public Button btnCloseMonitor;
    public Button btnResearch;
    public Button btnECAD;
    public Button btnNextPage;
    public Button btnPreviousPage;
    public Button btnBackToFileList;

    [Header("UI Display Elements")]
    public GameObject fileButtonPrefab;
    public Transform contentParent;
    public GameObject monitorPanelRoot;
    public GameObject fileListPanel;
    public RawImage monitorScreenDisplay;
    public TMP_Text pageNumberText;

    [Tooltip("The AspectRatioFitter component on the MonitorScreenDisplay RawImage.")]
    public AspectRatioFitter monitorAspectRatioFitter;

    private MonitorIndex monitorData;
    private List<Texture2D> currentDocumentPages;
    private int currentPageIndex = 0;
    private string currentCategoryName = "Research Papers";

    private float lastClickTime = 0f;
    private float clickCooldown = 0.2f;

    private RectTransform _monitorScreenRectTransform;
    private Vector2 _originalAnchorMin;
    private Vector2 _originalAnchorMax;
    private Vector2 _originalOffsetMin;
    private Vector2 _originalOffsetMax;

    [Tooltip("The resource path for the specific landscape PPT document. Needs to match the folder name.")]
    public string landscapePptResourcePath = "MonitorContent/ResearchPapers/Oeffentlichkeitsdarstellung";

    [Header("Aspect Ratios")]
    [Tooltip("Aspect ratio (width/height) for landscape PPT pages (e.g., 16f / 9f).")]
    public float landscapePptAspectRatio = 16f / 9f; 
    [Tooltip("Aspect ratio (width/height) for portrait research paper pages (e.g., 1654f / 2339f).")]
    public float portraitPaperAspectRatio = 1654f / 2339f; 

    [System.Serializable]
    public class FileItem { public string title; public string resourcePath; }
    [System.Serializable]
    public class Category { public string name; public List<FileItem> files; }
    [System.Serializable]
    public class MonitorIndex { public List<Category> categories; }


    void Awake()
    {
        _monitorScreenRectTransform = monitorScreenDisplay?.GetComponent<RectTransform>();
        if (_monitorScreenRectTransform != null)
        {
            _originalAnchorMin = _monitorScreenRectTransform.anchorMin;
            _originalAnchorMax = _monitorScreenRectTransform.anchorMax;
            _originalOffsetMin = _monitorScreenRectTransform.offsetMin;
            _originalOffsetMax = _monitorScreenRectTransform.offsetMax;

            Debug.Log("MonitorDisplayManager: Stored original monitor screen RectTransform settings in Awake.");
            Debug.Log($"Original AnchorMin: {_originalAnchorMin}, AnchorMax: {_originalAnchorMax}");
            Debug.Log($"Original OffsetMin: {_originalOffsetMin}, OffsetMax: {_originalOffsetMax}");
        }
        else
        {
            Debug.LogError("MonitorDisplayManager: monitorScreenDisplay or its RectTransform is null in Awake! Please assign monitorScreenDisplay in the Inspector.", this);
        }

        if (monitorAspectRatioFitter == null && monitorScreenDisplay != null)
        {
            monitorAspectRatioFitter = monitorScreenDisplay.GetComponent<AspectRatioFitter>();
            if (monitorAspectRatioFitter == null)
            {
                Debug.LogWarning("MonitorDisplayManager: AspectRatioFitter not found on monitorScreenDisplay. Dynamic aspect ratio will not work correctly. Please add an AspectRatioFitter component to MonitorScreenDisplay.", this);
            }
        }
    }


    void Start()
    {
        if (monitorPanelRoot != null) monitorPanelRoot.SetActive(false);
        LoadMonitorData();

        if (btnOpenMonitor != null) { btnOpenMonitor.onClick.RemoveAllListeners(); btnOpenMonitor.onClick.AddListener(OpenMonitor); }
        if (btnCloseMonitor != null) { btnCloseMonitor.onClick.RemoveAllListeners(); btnCloseMonitor.onClick.AddListener(CloseMonitor); }
        if (btnResearch != null) { btnResearch.onClick.RemoveAllListeners(); btnResearch.onClick.AddListener(() => ShowCategory("Research Papers")); }
        if (btnECAD != null) { btnECAD.onClick.RemoveAllListeners(); btnECAD.onClick.AddListener(() => ShowCategory("ECAD Files")); }
        if (btnNextPage != null) { btnNextPage.onClick.RemoveAllListeners(); btnNextPage.onClick.AddListener(ShowNextPage); }
        if (btnPreviousPage != null) { btnPreviousPage.onClick.RemoveAllListeners(); btnPreviousPage.onClick.AddListener(ShowPreviousPage); }
        if (btnBackToFileList != null) { btnBackToFileList.onClick.RemoveAllListeners(); btnBackToFileList.onClick.AddListener(ReturnToFileList); }
        if (monitorScreenDisplay != null) monitorScreenDisplay.gameObject.SetActive(false);
        if (btnNextPage != null) btnNextPage.gameObject.SetActive(false);
        if (btnPreviousPage != null) btnPreviousPage.gameObject.SetActive(false);
        if (pageNumberText != null) pageNumberText.gameObject.SetActive(false);
        if (fileListPanel != null) fileListPanel.gameObject.SetActive(false);
        if (btnBackToFileList != null) btnBackToFileList.gameObject.SetActive(false);

        if (monitorAspectRatioFitter != null)
        {
            monitorAspectRatioFitter.enabled = false;
        }
    }

    void LoadMonitorData()
    {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>("MonitorContent/monitorIndex");
        if (jsonTextAsset == null) { Debug.LogError("MonitorDisplayManager: monitorIndex.json TextAsset not found.", this); return; }
        monitorData = JsonUtility.FromJson<MonitorIndex>(jsonTextAsset.text);
        Debug.Log("Monitor data loaded successfully.");
    }

    
    public void ShowCategory(string categoryName)
    {
        ClearPreviousFiles();
        currentCategoryName = categoryName;
        Category category = monitorData.categories.Find(c => c.name == categoryName);
        if (category == null) { Debug.LogWarning("MonitorDisplayManager: Category not found: " + categoryName, this); return; }

        if (fileListPanel != null) fileListPanel.gameObject.SetActive(true);
        if (monitorScreenDisplay != null) monitorScreenDisplay.gameObject.SetActive(false);
        if (btnBackToFileList != null) btnBackToFileList.gameObject.SetActive(false);
        if (btnNextPage != null) btnNextPage.gameObject.SetActive(false);
        if (btnPreviousPage != null) btnPreviousPage.gameObject.SetActive(false);
        if (pageNumberText != null) pageNumberText.gameObject.SetActive(false);

        foreach (var file in category.files)
        {
            if (fileButtonPrefab == null || contentParent == null) { Debug.LogError("Prefab or content parent is null.", this); continue; }
            GameObject btnObj = Instantiate(fileButtonPrefab, contentParent);
            TMP_Text buttonText = btnObj.GetComponentInChildren<TMP_Text>();
            if (buttonText != null) { buttonText.text = file.title; } else { Debug.LogError($"Button Prefab '{fileButtonPrefab.name}' missing TMP_Text!", btnObj); }
            Button btn = btnObj.GetComponent<Button>();
            if (btn != null) { string filePath = file.resourcePath; string fileTitle = file.title; btn.onClick.AddListener(() => OnFileClicked(filePath, fileTitle)); } else { Debug.LogError($"Button Prefab '{fileButtonPrefab.name}' missing Button component!", btnObj); }
        }
        Debug.Log($"Displaying category: {categoryName}");
    }

    void ClearPreviousFiles()
    {
        if (contentParent == null) return;
        foreach (Transform child in contentParent) { Destroy(child.gameObject); }
    }

    public void OnFileClicked(string resourcePath, string title)
    {
        Debug.Log("Clicked document: " + title + " from path: " + resourcePath);

        ClearCurrentDocument(); 

        if (fileListPanel != null) fileListPanel.gameObject.SetActive(false);
        if (monitorScreenDisplay != null) monitorScreenDisplay.gameObject.SetActive(true);
        if (btnBackToFileList != null) btnBackToFileList.gameObject.SetActive(true);

        if (_monitorScreenRectTransform != null) 
        {
            if (resourcePath == landscapePptResourcePath)
            {
                Debug.Log("MonitorDisplayManager: Loading Landscape PPT. Adjusting monitor screen to full size.");
                _monitorScreenRectTransform.anchorMin = new Vector2(0, 0); 
                _monitorScreenRectTransform.anchorMax = new Vector2(1, 1);
                _monitorScreenRectTransform.offsetMin = Vector2.zero; 
                _monitorScreenRectTransform.offsetMax = Vector2.zero; 

                if (monitorAspectRatioFitter != null)
                {
                    monitorAspectRatioFitter.enabled = true;
                    monitorAspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent; 
                    monitorAspectRatioFitter.aspectRatio = landscapePptAspectRatio; 
                }
            }
            else 
            {
                Debug.Log("MonitorDisplayManager: Loading Portrait Research Paper. Restoring original monitor screen size.");
                _monitorScreenRectTransform.anchorMin = _originalAnchorMin; 
                _monitorScreenRectTransform.anchorMax = _originalAnchorMax;
                _monitorScreenRectTransform.offsetMin = _originalOffsetMin; 
                _monitorScreenRectTransform.offsetMax = _originalOffsetMax;

                if (monitorAspectRatioFitter != null)
                {
                    monitorAspectRatioFitter.enabled = true;
                    monitorAspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent; 
                    monitorAspectRatioFitter.aspectRatio = portraitPaperAspectRatio; 
                }
            }
        }

        currentDocumentPages = new List<Texture2D>();
        int pageNum = 1;
        Texture2D pageTexture;
        while ((pageTexture = Resources.Load<Texture2D>($"{resourcePath}/page_{pageNum:000}")) != null)
        {
            currentDocumentPages.Add(pageTexture);
            pageNum++;
        }

        if (currentDocumentPages.Count == 0) { Debug.LogError($"No pages found for: {resourcePath}.", this); ReturnToFileList(); return; }
        currentPageIndex = 0;
        ShowCurrentPage();
    }

    void ClearCurrentDocument()
    {
        if (monitorScreenDisplay != null)
        {
            monitorScreenDisplay.texture = null;
            monitorScreenDisplay.gameObject.SetActive(false);

            if (_monitorScreenRectTransform != null)
            {
                
                _monitorScreenRectTransform.anchorMin = _originalAnchorMin;
                _monitorScreenRectTransform.anchorMax = _originalAnchorMax;
                _monitorScreenRectTransform.offsetMin = _originalOffsetMin; 
                _monitorScreenRectTransform.offsetMax = _originalOffsetMax;
                Debug.Log("MonitorDisplayManager: Monitor screen RectTransform reset to original portrait settings.");
            }

            if (monitorAspectRatioFitter != null) { monitorAspectRatioFitter.enabled = false; } 
        }
        if (currentDocumentPages != null) currentDocumentPages.Clear();
        currentPageIndex = 0;
        if (btnNextPage != null) btnNextPage.gameObject.SetActive(false);
        if (btnPreviousPage != null) btnPreviousPage.gameObject.SetActive(false);
        if (pageNumberText != null) pageNumberText.gameObject.SetActive(false);
    }

    void ShowCurrentPage()
    {
        if (monitorScreenDisplay == null || currentDocumentPages == null || currentDocumentPages.Count == 0) { Debug.LogWarning("No pages to show.", this); ClearCurrentDocument(); return; }
        monitorScreenDisplay.texture = currentDocumentPages[currentPageIndex];
        if (pageNumberText != null) { pageNumberText.gameObject.SetActive(true); pageNumberText.text = $"{currentPageIndex + 1} / {currentDocumentPages.Count}"; }
        if (btnPreviousPage != null) btnPreviousPage.gameObject.SetActive(currentPageIndex > 0);
        if (btnNextPage != null) btnNextPage.gameObject.SetActive(currentPageIndex < currentDocumentPages.Count - 1);
    }

    public void ShowNextPage()
    {
        if (Time.time < lastClickTime + clickCooldown) return;
        lastClickTime = Time.time;
        if (currentDocumentPages != null && currentPageIndex < currentDocumentPages.Count - 1) { currentPageIndex++; ShowCurrentPage(); } else { Debug.Log("At end of document."); }
    }

    public void ShowPreviousPage()
    {
        if (Time.time < lastClickTime + clickCooldown) return;
        lastClickTime = Time.time;
        if (currentDocumentPages != null && currentPageIndex > 0) { currentPageIndex--; ShowCurrentPage(); } else { Debug.Log("At beginning of document."); }
    }

    public void OpenMonitor()
    {
        if (btnOpenMonitor != null && !btnOpenMonitor.interactable) { Debug.Log("Cannot open monitor. Button not interactable.", this); return; }
        if (monitorPanelRoot != null) monitorPanelRoot.SetActive(true);
        if (btnOpenMonitor != null) btnOpenMonitor.gameObject.SetActive(false);
        ShowCategory("Research Papers");
    }

    public void CloseMonitor()
    {
        if (monitorPanelRoot != null) monitorPanelRoot.SetActive(false);
        if (btnOpenMonitor != null) btnOpenMonitor.gameObject.SetActive(true);
        ClearPreviousFiles();
        ClearCurrentDocument();
        if (fileListPanel != null) fileListPanel.gameObject.SetActive(false);
    }

    public void ReturnToFileList()
    {
        ClearCurrentDocument();
        ShowCategory(currentCategoryName);
    }
}