using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using System.Linq; 

public class MonitorDisplayManager : MonoBehaviour
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

    private MonitorIndex monitorData;
    private List<Texture2D> currentDocumentPages;
    private int currentPageIndex = 0;
    private string currentCategoryName = "Research Papers"; 

    private float lastClickTime = 0f;
    private float clickCooldown = 0.2f; 

    [System.Serializable]
    public class FileItem
    {
        public string title;
        public string resourcePath;
    }

    [System.Serializable]
    public class Category
    {
        public string name;
        public List<FileItem> files;
    }

    [System.Serializable]
    public class MonitorIndex
    {
        public List<Category> categories;
    }


    void OnValidate()
    {
    }


    void Start()
    {
        if (monitorPanelRoot != null) monitorPanelRoot.SetActive(false);

        LoadMonitorData();
        
        if (btnOpenMonitor != null) btnOpenMonitor.onClick.RemoveAllListeners();
        if (btnOpenMonitor != null) btnOpenMonitor.onClick.AddListener(OpenMonitor);

        if (btnCloseMonitor != null) btnCloseMonitor.onClick.RemoveAllListeners();
        if (btnCloseMonitor != null) btnCloseMonitor.onClick.AddListener(CloseMonitor);

        if (btnResearch != null) btnResearch.onClick.RemoveAllListeners();
        if (btnResearch != null) btnResearch.onClick.AddListener(() => ShowCategory("Research Papers"));

        if (btnECAD != null) btnECAD.onClick.RemoveAllListeners();
        if (btnECAD != null) btnECAD.onClick.AddListener(() => ShowCategory("ECAD Files"));

        if (btnNextPage != null) btnNextPage.onClick.RemoveAllListeners();
        if (btnNextPage != null) btnNextPage.onClick.AddListener(ShowNextPage);

        if (btnPreviousPage != null) btnPreviousPage.onClick.RemoveAllListeners();
        if (btnPreviousPage != null) btnPreviousPage.onClick.AddListener(ShowPreviousPage);

        if (btnBackToFileList != null) btnBackToFileList.onClick.RemoveAllListeners();
        if (btnBackToFileList != null) btnBackToFileList.onClick.AddListener(ReturnToFileList);

        if (monitorScreenDisplay != null) monitorScreenDisplay.gameObject.SetActive(false);
        if (btnNextPage != null) btnNextPage.gameObject.SetActive(false);
        if (btnPreviousPage != null) btnPreviousPage.gameObject.SetActive(false);
        if (pageNumberText != null) pageNumberText.gameObject.SetActive(false);
        if (fileListPanel != null) fileListPanel.gameObject.SetActive(false); 
        if (btnBackToFileList != null) btnBackToFileList.gameObject.SetActive(false); 
    }


    void LoadMonitorData()
    {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>("MonitorContent/monitorIndex");
        if (jsonTextAsset == null)
        {
            Debug.LogError("MonitorDisplayManager: monitorIndex.json TextAsset not found in Resources/MonitorContent/! Please ensure the file is there.", this);
            return;
        }

        monitorData = JsonUtility.FromJson<MonitorIndex>(jsonTextAsset.text);
        Debug.Log("Monitor data loaded successfully.");
    }

    public void ShowCategory(string categoryName)
    {
        ClearPreviousFiles();
        
        currentCategoryName = categoryName;

        Category category = monitorData.categories.Find(c => c.name == categoryName);
        if (category == null)
        {
            Debug.LogWarning("MonitorDisplayManager: Category not found: " + categoryName, this);
            return;
        }

        if (fileListPanel != null) fileListPanel.gameObject.SetActive(true);
        if (monitorScreenDisplay != null) monitorScreenDisplay.gameObject.SetActive(false);
        if (btnBackToFileList != null) btnBackToFileList.gameObject.SetActive(false);
        if (btnNextPage != null) btnNextPage.gameObject.SetActive(false);
        if (btnPreviousPage != null) btnPreviousPage.gameObject.SetActive(false);
        if (pageNumberText != null) pageNumberText.gameObject.SetActive(false);


        foreach (var file in category.files)
        {
            if (fileButtonPrefab == null)
            {
                Debug.LogError("MonitorDisplayManager: fileButtonPrefab is null! Cannot instantiate file buttons.", this);
                continue;
            }
            if (contentParent == null)
            {
                Debug.LogError("MonitorDisplayManager: contentParent is null! Cannot instantiate file buttons.", this);
                continue;
            }

            GameObject btnObj = Instantiate(fileButtonPrefab, contentParent);
            TMP_Text buttonText = btnObj.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = file.title;
            }
            else
            {
                Debug.LogError($"MonitorDisplayManager: Button Prefab '{fileButtonPrefab.name}' is missing a TMP_Text component in its children!", btnObj);
            }

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                string filePath = file.resourcePath;
                string fileTitle = file.title;
                btn.onClick.AddListener(() => OnFileClicked(filePath, fileTitle));
            }
            else
            {
                Debug.LogError($"MonitorDisplayManager: Button Prefab '{fileButtonPrefab.name}' is missing a Button component!", btnObj);
            }
        }
        Debug.Log($"Displaying category: {categoryName}");
    }


    void ClearPreviousFiles()
    {
        if (contentParent == null) return;
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void OnFileClicked(string resourcePath, string title)
    {
        Debug.Log("Clicked document: " + title + " from path: " + resourcePath);

        ClearCurrentDocument();

        
        if (fileListPanel != null) fileListPanel.gameObject.SetActive(false);
        if (monitorScreenDisplay != null) monitorScreenDisplay.gameObject.SetActive(true);
        if (btnBackToFileList != null) btnBackToFileList.gameObject.SetActive(true);

        currentDocumentPages = new List<Texture2D>();
        int pageNum = 1;
        Texture2D pageTexture;
        while ((pageTexture = Resources.Load<Texture2D>($"{resourcePath}/page_{pageNum:000}")) != null)
        {
            Debug.Log($"MonitorDisplayManager: Successfully loaded page: {resourcePath}/page_{pageNum:000}");
            currentDocumentPages.Add(pageTexture);
            pageNum++;
        }

        if (currentDocumentPages.Count == 0)
        {
            Debug.LogError($"MonitorDisplayManager: No pages found for document: {resourcePath}. Ensure images are in Resources and named page_001.png, page_002.png, etc.", this);
            ReturnToFileList(); 
            return;
        }

        currentPageIndex = 0; 
        Debug.Log($"MonitorDisplayManager: Document loaded with {currentDocumentPages.Count} pages. Initial page index: {currentPageIndex}");
        ShowCurrentPage();
    }


    void ClearCurrentDocument()
    {
        if (monitorScreenDisplay != null)
        {
            monitorScreenDisplay.texture = null;
            monitorScreenDisplay.gameObject.SetActive(false);
        }
        if (currentDocumentPages != null)
        {
            currentDocumentPages.Clear();
        }
        currentPageIndex = 0; 
        if (btnNextPage != null) btnNextPage.gameObject.SetActive(false);
        if (btnPreviousPage != null) btnPreviousPage.gameObject.SetActive(false);
        if (pageNumberText != null) pageNumberText.gameObject.SetActive(false);
    }

    
    void ShowCurrentPage()
    {
        if (monitorScreenDisplay == null)
        {
            Debug.LogError("MonitorScreenDisplay RawImage is null!", this);
            return;
        }
        if (currentDocumentPages == null || currentDocumentPages.Count == 0)
        {
            Debug.LogWarning("MonitorDisplayManager: Attempted to show page, but currentDocumentPages is empty or null.", this);
            ClearCurrentDocument();
            return;
        }

        Debug.Log($"MonitorDisplayManager: Displaying page {currentPageIndex + 1}/{currentDocumentPages.Count}. Texture: {currentDocumentPages[currentPageIndex]?.name ?? "NULL"}");
        monitorScreenDisplay.texture = currentDocumentPages[currentPageIndex];


        if (pageNumberText != null)
        {
            pageNumberText.gameObject.SetActive(true);
            pageNumberText.text = $"{currentPageIndex + 1} / {currentDocumentPages.Count}";
        }

        if (btnPreviousPage != null) btnPreviousPage.gameObject.SetActive(currentPageIndex > 0);
        if (btnNextPage != null) btnNextPage.gameObject.SetActive(currentPageIndex < currentDocumentPages.Count - 1);
    }

    public void ShowNextPage()
    {
        if (Time.time < lastClickTime + clickCooldown)
        {
            Debug.Log("MonitorDisplayManager: ShowNextPage: On cooldown, ignoring click.");
            return;
        }
        lastClickTime = Time.time;

        Debug.Log($"MonitorDisplayManager: ShowNextPage called. Current index: {currentPageIndex}. Total pages: {(currentDocumentPages != null ? currentDocumentPages.Count.ToString() : "0")}");
        if (currentDocumentPages != null && currentPageIndex < currentDocumentPages.Count - 1)
        {
            currentPageIndex++;
            Debug.Log($"MonitorDisplayManager: Index incremented to {currentPageIndex}. Calling ShowCurrentPage.");
            ShowCurrentPage();
        }
        else
        {
            Debug.Log("MonitorDisplayManager: Cannot go to next page (at end of document or no pages loaded).");
        }
    }


    public void ShowPreviousPage()
    {
        if (Time.time < lastClickTime + clickCooldown)
        {
            Debug.Log("MonitorDisplayManager: ShowPreviousPage: On cooldown, ignoring click.");
            return;
        }
        lastClickTime = Time.time; 

        Debug.Log($"MonitorDisplayManager: ShowPreviousPage called. Current index: {currentPageIndex}.");
        if (currentDocumentPages != null && currentPageIndex > 0)
        {
            currentPageIndex--;
            Debug.Log($"MonitorDisplayManager: Index decremented to {currentPageIndex}. Calling ShowCurrentPage.");
            ShowCurrentPage();
        }
        else
        {
            Debug.Log("MonitorDisplayManager: Cannot go to previous page (at beginning of document).");
        }
    }


    public void OpenMonitor()
    {
        
        if (btnOpenMonitor != null && !btnOpenMonitor.interactable)
        {
            Debug.Log("MonitorDisplayManager: Cannot open monitor. Button is not interactable (player not in range or facing).");
            return;
        }

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