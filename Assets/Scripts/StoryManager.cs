using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    [SerializeField] public StoryData[] storyDatas; // ストーリーデータの配列

    [SerializeField] private Image background; // 背景画像
    [SerializeField] private Image characterImage; // キャラクター画像
    //[SerializeField] private Image characterImage2; //キャラ画像2
    [SerializeField] private AudioClip SoundEffect; //効果音
    [SerializeField] private TextMeshProUGUI storyText; // ストーリーテキスト
    [SerializeField] private TextMeshProUGUI characterName; // キャラクター名
    [SerializeField] private SoundManager soundManager; //bgm
    
    // 選択肢UI
    [Header("Choices UI (optional)")]
    [SerializeField] private RectTransform choicesContainer; // 未指定ならランタイムで生成
    [SerializeField] private Sprite choiceButtonSprite; // 任意（未指定でも可）
    
    // DispMenuManagerへの参照
    private DispMenuManager dispMenuManager;
    
    public int storyIndex { get; private set; } // 現在のストーリーインデックス
    public int textIndex { get; private set; } // 現在のテキストインデックス
    private Coroutine typingCoroutine;

    private bool finishText = false;
    private bool isChoosing = false;
    private readonly List<GameObject> spawnedChoiceButtons = new List<GameObject>();

    private void Start()
    {
        // DispMenuManagerを取得
        dispMenuManager = FindObjectOfType<DispMenuManager>();
        
        storyText.text = "";
        characterName.text = "";
        SetStoryElement(storyIndex, textIndex);
    }

    private void Update()
    {
        if (isChoosing) return; // 選択中はEnterで進めない

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!finishText)
            {
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                }
                var currentStoryText = storyDatas[storyIndex].stories[textIndex].StoryText;
                storyText.text = currentStoryText;
                finishText = true;
            }
            else
            {
                // 現在のストーリーをバックログに追加
                AddCurrentStoryToBacklog();

                // 分岐がある場合は選択肢を表示して待機
                var currentStory = storyDatas[storyIndex].stories[textIndex];
                if (currentStory.Choices != null && currentStory.Choices.Count > 0)
                {
                    ShowChoices(currentStory.Choices);
                    return;
                }

                // 通常進行
                textIndex++;
                storyText.text = "";
                finishText = false;
                ProgressionStory(storyIndex);
            }
        }
    }
    
    // 現在のストーリーをバックログに追加
    private void AddCurrentStoryToBacklog()
    {
        if (dispMenuManager != null && 
            storyDatas != null && 
            storyIndex < storyDatas.Length && 
            textIndex < storyDatas[storyIndex].stories.Count)
        {
            var currentStory = storyDatas[storyIndex].stories[textIndex];
            dispMenuManager.AddToBacklog(currentStory.StoryText, currentStory.CharacterName);
        }
    }
    
    private void SetStoryElement(int _storyIndex, int _textIndex)
        {
            soundManager.PlayBGM(storyDatas[_storyIndex].bgm);
            var storyElement = storyDatas[_storyIndex].stories[_textIndex];

            background.sprite = storyElement.Background;
            characterImage.sprite = storyElement.CharacterImage;
            characterName.text = storyElement.CharacterName;
            
            // 効果音が設定されていれば再生
            if (storyElement.SFX != null)
            {
                soundManager.PlaySE(storyElement.SFX);
            }

            if(typingCoroutine !=null)
            {
                StopCoroutine(typingCoroutine);
            }
            
            //storyText.text = storyElement.StoryText;
            typingCoroutine = StartCoroutine(TypeSentence(_storyIndex, _textIndex));
        }

    private void ProgressionStory(int _storyIndex)
    {
        if (textIndex < storyDatas[_storyIndex].stories.Count)
        {
            SetStoryElement(storyIndex, textIndex);
        }
        else
        {
            //シーンチェンジ，選択肢出す、別のScriptableObjectを呼ぶ
            textIndex = 0;
            ChangeStoryElement();
            return;
        }
    }

    private void ChangeStoryElement()
    {
        textIndex = 0;
        storyIndex++;
        SetStoryElement(storyIndex, textIndex);
    }

    private IEnumerator TypeSentence(int _storyIndex, int _textIndex)
    {
        storyText.text = "";
        finishText = false;
        foreach (var letter in storyDatas[_storyIndex].stories[_textIndex].StoryText.ToCharArray())
        {
            storyText.text+= letter;
            yield return new WaitForSeconds(0.1f);
        }

        finishText = true;
    }

    // ===== 分岐処理 =====
    private void ShowChoices(List<ChoiceOption> choices)
    {
        isChoosing = true;

        // 親コンテナが未指定ならCanvas直下に簡易パネルを生成
        if (choicesContainer == null)
        {
            var canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("Canvas not found for choices UI");
                return;
            }

            var panelGO = new GameObject("ChoicesPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panelGO.transform.SetParent(canvas.transform, false);
            var panelRect = (RectTransform)panelGO.transform;
            panelRect.anchorMin = new Vector2(0.5f, 0f);
            panelRect.anchorMax = new Vector2(0.5f, 0f);
            panelRect.pivot = new Vector2(0.5f, 0f);
            panelRect.anchoredPosition = new Vector2(0f, 40f);
            panelRect.sizeDelta = new Vector2(800f, 0f);
            var panelImg = panelGO.GetComponent<Image>();
            panelImg.color = new Color(0f, 0f, 0f, 0.5f);

            // レイアウト
            var layout = panelGO.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
            layout.childControlHeight = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;
            layout.spacing = 8f;

            var fitter = panelGO.AddComponent<UnityEngine.UI.ContentSizeFitter>();
            fitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;

            choicesContainer = panelRect;
        }

        // 既存のボタンを掃除
        ClearChoicesInternal();

        for (int i = 0; i < choices.Count; i++)
        {
            var opt = choices[i];
            var btnGO = new GameObject($"Choice_{i}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btnGO.transform.SetParent(choicesContainer, false);
            var img = btnGO.GetComponent<Image>();
            if (choiceButtonSprite != null) img.sprite = choiceButtonSprite; else img.color = new Color(1f, 1f, 1f, 0.2f);

            var btn = btnGO.GetComponent<Button>();
            int capturedIndex = i;
            btn.onClick.AddListener(() => OnChoiceSelected(choices[capturedIndex]));

            // ラベル(TMP)
            var labelGO = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            labelGO.transform.SetParent(btnGO.transform, false);
            var labelRect = (RectTransform)labelGO.transform;
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(16f, 8f);
            labelRect.offsetMax = new Vector2(-16f, -8f);
            var label = labelGO.GetComponent<TextMeshProUGUI>();
            label.text = string.IsNullOrEmpty(opt.Text) ? "選択肢" : opt.Text;
            label.fontSize = 28f;
            label.color = Color.white;
            label.enableWordWrapping = true;
            label.alignment = TextAlignmentOptions.MidlineLeft;

            // 高さの目安
            var le = btnGO.AddComponent<UnityEngine.UI.LayoutElement>();
            le.minHeight = 64f;

            spawnedChoiceButtons.Add(btnGO);
        }
    }

    private void OnChoiceSelected(ChoiceOption option)
    {
        // UI片付け
        ClearChoicesInternal();
        isChoosing = false;

        // インデックス計算
        int nextStory = storyIndex;
        int nextText = textIndex + 1;

        if (option != null)
        {
            if (option.NextStoryIndex >= 0) nextStory = option.NextStoryIndex;
            if (option.NextTextIndex >= 0) nextText = option.NextTextIndex;
        }

        storyText.text = string.Empty;
        finishText = false;

        // 範囲内に収まるよう調整しつつ進行
        storyIndex = Mathf.Clamp(nextStory, 0, storyDatas.Length - 1);
        textIndex = Mathf.Clamp(nextText, 0, storyDatas[storyIndex].stories.Count);

        if (textIndex >= storyDatas[storyIndex].stories.Count)
        {
            // 章末に飛んだ場合は次章へ
            textIndex = 0;
            ChangeStoryElement();
        }
        else
        {
            SetStoryElement(storyIndex, textIndex);
        }
    }

    private void ClearChoicesInternal()
    {
        foreach (var go in spawnedChoiceButtons)
        {
            if (go != null) Destroy(go);
        }
        spawnedChoiceButtons.Clear();
    }
}