using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DispMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _displayMenuObject;//ディスプレイオブジェクト
    [SerializeField] private GameObject LogButtonPrefab;//ログボタンプレハブ
    [SerializeField] private GameObject backlogDisplay;//バックログディスプレイ
    [SerializeField] private GameObject backlogPrefab;//バックログプレハブ
    [SerializeField] private Sprite _logButtonInactiveSprite;
    [SerializeField] private Sprite _logButtonActiveSprite;
    
    private bool _isActiveLog = false;
    private GameObject _logButtonInstance;
    private GameObject _backlogInstance;
    
    // バックログ用のリスト
    private List<BacklogEntry> backlogLogTextList = new List<BacklogEntry>();
    
    // StoryManagerへの参照
    private StoryManager storyManager;

    void Start()
    {
        // StoryManagerを取得
        storyManager = FindObjectOfType<StoryManager>();
        
        // StoryManagerが見つからない場合のエラーハンドリング
        if (storyManager == null)
        {
            Debug.LogError("StoryManager not found in the scene!");
            return;
        }
        
        // ログボタンのインスタンスを作成
        if (LogButtonPrefab != null)
        {
            // backlogDisplayがプレハブの場合は、Canvasを探してそこに生成
            Transform parentTransform = null;
            
            if (backlogDisplay != null)
            {
                // backlogDisplayがプレハブの場合は、シーン内のCanvasを探す
                Canvas canvas = FindObjectOfType<Canvas>();
                if (canvas != null)
                {
                    parentTransform = canvas.transform;
                    Debug.Log("Using Canvas as parent for LogButton");
                }
                else
                {
                    Debug.LogError("Canvas not found in scene!");
                    return;
                }
            }
            else
            {
                Debug.LogError("backlogDisplay is not assigned!");
                return;
            }
            
            // LogButtonを生成
            _logButtonInstance = Instantiate(LogButtonPrefab, parentTransform);
            Debug.Log("LogButton instantiated successfully");

            //ボタンの位置を設定（必要に応じて）

            //ボタンの表示設定を設定
            DisplaySprite();

            //ボタンのクリックイベントを登録
            Button logButtonComponent = _logButtonInstance.GetComponent<Button>();
            if (logButtonComponent != null)
            {
                logButtonComponent.onClick.AddListener(ToggleLog);
                Debug.Log("LogButton click event registered");
            }
            else
            {
                Debug.LogError("Button component not found on LogButtonPrefab!");
            }
        }
        else
        {
            Debug.LogError("LogButtonPrefab is not assigned!");
        }
    }

    // バックログエントリを追加するメソッド
    public void AddToBacklog(string storyText, string characterName)
    {
        BacklogEntry entry = new BacklogEntry
        {
            StoryText = storyText,
            CharacterName = characterName
        };
        backlogLogTextList.Add(entry);
        Debug.Log($"Added to backlog: {characterName} - {storyText}");
    }

    // バックログリストを取得する公開メソッド
    public List<BacklogEntry> GetBacklogList()
    {
        return new List<BacklogEntry>(backlogLogTextList);
    }

    // バックログリストをクリアするメソッド
    public void ClearBacklog()
    {
        backlogLogTextList.Clear();
    }

    // 現在のストーリーからバックログを更新
    public void UpdateBacklogFromCurrentStory()
    {
        if (storyManager != null && storyManager.storyDatas != null)
        {
            // 現在のストーリーインデックスとテキストインデックスを取得
            int currentStoryIndex = storyManager.storyIndex;
            int currentTextIndex = storyManager.textIndex;
            
            // StoryDataから現在のストーリーを取得
            if (currentStoryIndex < storyManager.storyDatas.Length &&
                currentTextIndex < storyManager.storyDatas[currentStoryIndex].stories.Count)
            {
                var currentStory = storyManager.storyDatas[currentStoryIndex].stories[currentTextIndex];
                AddToBacklog(currentStory.StoryText, currentStory.CharacterName);
            }
        }
        else
        {
            Debug.LogWarning("StoryManager or storyDatas is null!");
        }
    }

    // バックログの内容を表示するメソッド
    private void DisplayBacklog()
    {
        if (_backlogInstance != null)
        {
            // バックログの内容を表示するUIを作成
            Transform contentTransform = _backlogInstance.transform.Find("Content");
            if (contentTransform != null)
            {
                // 既存の内容をクリア
                foreach (Transform child in contentTransform)
                {
                    Destroy(child.gameObject);
                }
                
                // バックログの内容を表示
                for (int i = 0; i < backlogLogTextList.Count; i++)
                {
                    var entry = backlogLogTextList[i];
                    CreateBacklogEntryUI(contentTransform, entry, i);
                }
            }
        }
    }

    // バックログエントリのUIを作成
    private void CreateBacklogEntryUI(Transform parent, BacklogEntry entry, int index)
    {
        // エントリ用のGameObjectを作成
        GameObject entryObject = new GameObject($"BacklogEntry_{index}");
        entryObject.transform.SetParent(parent, false);
        
        // レイアウト要素を追加
        var layoutElement = entryObject.AddComponent<LayoutElement>();
        layoutElement.minHeight = 100;
        
        // キャラクター名のテキスト
        GameObject nameObject = new GameObject("CharacterName");
        nameObject.transform.SetParent(entryObject.transform, false);
        var nameText = nameObject.AddComponent<TextMeshProUGUI>();
        nameText.text = entry.CharacterName;
        nameText.fontSize = 18;
        nameText.color = Color.white;
        
        // ストーリーテキスト
        GameObject textObject = new GameObject("StoryText");
        textObject.transform.SetParent(entryObject.transform, false);
        var storyText = textObject.AddComponent<TextMeshProUGUI>();
        storyText.text = entry.StoryText;
        storyText.fontSize = 16;
        storyText.color = Color.white;
    }

    public bool IsLogActive()
    {
        return _isActiveLog;
    }

    void DisplaySprite()
    {
        // LogButtonの表示を切り替え
        if (_logButtonInstance != null)
        {
            Image logButtonImage = _logButtonInstance.GetComponent<Image>();
            if (logButtonImage != null)
            {
                logButtonImage.sprite = _isActiveLog ? _logButtonActiveSprite : _logButtonInactiveSprite;
            }
            else
            {
                Debug.LogWarning("Image component not found on _logButtonInstance!");
            }
        }
        else
        {
            Debug.LogWarning("_logButtonInstance is null!");
        }
    }

    public void ToggleLog()
    {
        Debug.Log("ToggleLog called. Current state: " + _isActiveLog);
        
        if (!_isActiveLog)
        {
            // ログが非アクティブの場合、バックログを生成
            if (backlogPrefab != null)
            {
                // Canvasを探してバックログを生成
                Canvas canvas = FindObjectOfType<Canvas>();
                if (canvas != null)
                {
                    _backlogInstance = Instantiate(backlogPrefab, canvas.transform);
                    Debug.Log("Backlog instance created successfully");
                    
                    // BacklogDisplayコンポーネントを取得して確認
                    BacklogDisplay backlogDisplayComponent = _backlogInstance.GetComponent<BacklogDisplay>();
                    if (backlogDisplayComponent != null)
                    {
                        Debug.Log("BacklogDisplay component found on instantiated object");
                    }
                    else
                    {
                        Debug.LogError("BacklogDisplay component not found on instantiated object!");
                    }
                }
                else
                {
                    Debug.LogError("Canvas not found for backlog creation!");
                }
            }
            else
            {
                Debug.LogError("backlogPrefab is null!");
            }
        }
        else
        {
            // ログがアクティブの場合、バックログを削除
            if (_backlogInstance != null)
            {
                Destroy(_backlogInstance);
                _backlogInstance = null;
                Debug.Log("Backlog instance destroyed");
            }
        }
        
        _isActiveLog = !_isActiveLog; // 状態を切り替える
        DisplaySprite(); // ボタンの表示を更新
    }

    public void TurnOffLog()
    {
        _isActiveLog = false; // 状態を切り替える
        
        // バックログを削除
        if (_backlogInstance != null)
        {
            Destroy(_backlogInstance);
            _backlogInstance = null;
        }
        
        DisplaySprite(); // ボタンの表示を更新
    }
}

// バックログエントリのデータ構造
[System.Serializable]
public class BacklogEntry
{
    public string StoryText;
    public string CharacterName;
}