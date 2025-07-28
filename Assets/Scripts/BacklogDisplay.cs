using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BacklogDisplay : MonoBehaviour
{
    //バックログに表示するテキストのリスト
    private List<(int LineNumber, string Name, string Text)> backlogLogTextList;

    //ViewportContentsのプレハブ
    [SerializeField] private GameObject viewportContentsPrefab;

    //ContentのRectTransform
    private RectTransform contentRectTransform;

    //ScrollViewのRectTransform
    private RectTransform scrollViewRectTransform;
    
    // 閉じるボタン
    private Button closeButton;
    
    // DispMenuManagerへの参照
    private DispMenuManager dispMenuManager;

    private void Start()
    {
        Debug.Log("BacklogDisplay Start called");
        
        // DispMenuManagerを取得
        dispMenuManager = FindObjectOfType<DispMenuManager>();
        if (dispMenuManager == null)
        {
            Debug.LogError("DispMenuManager not found!");
            return;
        }
        Debug.Log("DispMenuManager found successfully");
        
        // バックログのテキストリストを取得
        backlogLogTextList = GetBacklogTextList();
        
        // ContentとScrollViewのRectTransformを取得
        SetupRectTransforms();
        
        // 閉じるボタンを取得して設定
        SetupCloseButton();
        
        // バックログの内容を表示
        DisplayBacklogContents();
    }
    
    // バックログのテキストリストを取得
    private List<(int LineNumber, string Name, string Text)> GetBacklogTextList()
    {
        var backlogList = new List<(int LineNumber, string Name, string Text)>();
        
        if (dispMenuManager != null)
        {
            // DispMenuManagerからバックログエントリを取得
            var backlogEntries = dispMenuManager.GetBacklogList();
            
            int lineNumber = 1;
            foreach (var entry in backlogEntries)
            {
                backlogList.Add((lineNumber, entry.CharacterName, entry.StoryText));
                lineNumber++;
            }
            
            Debug.Log($"Retrieved {backlogEntries.Count} backlog entries from DispMenuManager");
        }
        else
        {
            Debug.LogWarning("DispMenuManager not found!");
        }
        
        // バックログが空の場合、現在のストーリーを追加
        if (backlogList.Count == 0)
        {
            StoryManager storyManager = FindObjectOfType<StoryManager>();
            if (storyManager != null && storyManager.storyDatas != null)
            {
                int currentStoryIndex = storyManager.storyIndex;
                int currentTextIndex = storyManager.textIndex;
                
                if (currentStoryIndex < storyManager.storyDatas.Length &&
                    currentTextIndex < storyManager.storyDatas[currentStoryIndex].stories.Count)
                {
                    var currentStory = storyManager.storyDatas[currentStoryIndex].stories[currentTextIndex];
                    backlogList.Add((1, currentStory.CharacterName, currentStory.StoryText));
                    Debug.Log($"Added current story to backlog: {currentStory.CharacterName} - {currentStory.StoryText}");
                }
            }
        }
        
        Debug.Log($"Total backlog entries: {backlogList.Count}");
        return backlogList;
    }
    
    // ContentとScrollViewのRectTransformを設定
    private void SetupRectTransforms()
    {
        Debug.Log("Setting up RectTransforms...");
        
        // Viewportを探す（ScrollRectのViewport）
        Transform viewportTransform = transform.Find("Viewport");
        if (viewportTransform != null)
        {
            scrollViewRectTransform = viewportTransform.GetComponent<RectTransform>();
            Debug.Log("Viewport found and RectTransform assigned");
            
            // Contentを探す
            Transform contentTransform = viewportTransform.Find("Content");
            if (contentTransform != null)
            {
                contentRectTransform = contentTransform.GetComponent<RectTransform>();
                Debug.Log("Content RectTransform found");
            }
            else
            {
                Debug.LogError("Content not found in Viewport!");
            }
        }
        else
        {
            Debug.LogError("Viewport not found!");
        }
    }
    
    // 閉じるボタンを設定
    private void SetupCloseButton()
    {
        // 閉じるボタンを探す
        Transform closeButtonTransform = transform.Find("BacklogClose");
        if (closeButtonTransform != null)
        {
            closeButton = closeButtonTransform.GetComponent<Button>();
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(CloseBacklog);
                Debug.Log("Close button setup completed");
            }
            else
            {
                Debug.LogError("Button component not found on BacklogClose!");
            }
        }
        else
        {
            Debug.LogError("BacklogClose not found!");
        }
    }
    
    // バックログの内容を表示
    private void DisplayBacklogContents()
    {
        if (contentRectTransform == null || viewportContentsPrefab == null)
        {
            Debug.LogError("Content RectTransform or ViewportContents prefab is missing!");
            return;
        }
        
        // 既存の内容をクリア
        foreach (Transform child in contentRectTransform)
        {
            Destroy(child.gameObject);
        }
        
        Debug.Log($"Displaying {backlogLogTextList.Count} backlog entries");
        
        // 各テキストエントリを生成
        for (int i = 0; i < backlogLogTextList.Count; i++)
        {
            var entry = backlogLogTextList[i];
            Debug.Log($"Creating viewport content {i}: {entry.Name} - {entry.Text}");
            CreateViewportContent(entry, i);
        }
        
        // Contentのサイズを調整
        if (contentRectTransform != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRectTransform);
            Debug.Log($"Content size after rebuild: {contentRectTransform.sizeDelta}");
        }
    }
    
    // ViewportContentを生成
    private void CreateViewportContent((int LineNumber, string Name, string Text) entry, int index)
    {
        // ViewportContentsプレハブをインスタンス化
        GameObject viewportContent = Instantiate(viewportContentsPrefab, contentRectTransform);
        Debug.Log($"Created viewport content GameObject: {viewportContent.name}");
        
        // 名前テキストを設定（ViewportName）
        Transform nameTextTransform = viewportContent.transform.Find("ViewportName");
        if (nameTextTransform != null)
        {
            TextMeshProUGUI nameText = nameTextTransform.GetComponent<TextMeshProUGUI>();
            if (nameText != null)
            {
                nameText.text = entry.Name;
                Debug.Log($"Set name text: {entry.Name}");
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found on ViewportName!");
            }
        }
        else
        {
            Debug.LogError("ViewportName not found in viewportContent!");
        }
        
        // ストーリーテキストを設定（ViewportSentence）
        Transform storyTextTransform = viewportContent.transform.Find("ViewportSentence");
        if (storyTextTransform != null)
        {
            TextMeshProUGUI storyText = storyTextTransform.GetComponent<TextMeshProUGUI>();
            if (storyText != null)
            {
                storyText.text = entry.Text;
                Debug.Log($"Set story text: {entry.Text}");
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found on ViewportSentence!");
            }
        }
        else
        {
            Debug.LogError("ViewportSentence not found in viewportContent!");
        }
        
        // 行番号は既存のプレハブには含まれていないので、必要に応じて追加
        // Transform lineNumberTransform = viewportContent.transform.Find("LineNumber");
        // if (lineNumberTransform != null)
        // {
        //     TextMeshProUGUI lineNumberText = lineNumberTransform.GetComponent<TextMeshProUGUI>();
        //     if (lineNumberText != null)
        //     {
        //         lineNumberText.text = entry.LineNumber.ToString();
        //     }
        // }
    }
    
    // バックログを閉じる
    private void CloseBacklog()
    {
        Debug.Log("CloseBacklog called");
        
        // このGameObjectを破棄
        Destroy(gameObject);
        
        // DispMenuManagerのログ状態を更新
        if (dispMenuManager != null)
        {
            dispMenuManager.TurnOffLog();
        }
    }
}
