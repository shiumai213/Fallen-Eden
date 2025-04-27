using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    [SerializeField] private StoryData[] storyDatas; // ストーリーデータの配列

    [SerializeField] private Image background; // 背景画像
    [SerializeField] private Image characterImage; // キャラクター画像
    //[SerializeField] private Image characterImage2; //キャラ画像2
    //[SerializeField] private AudioClip SoundEffect; //効果音
    [SerializeField] private TextMeshProUGUI storyText; // ストーリーテキスト
    [SerializeField] private TextMeshProUGUI characterName; // キャラクター名
    [SerializeField] private SoundManager soundManager; //bgm
    public int storyIndex { get; private set; } // 現在のストーリーインデックス
    public int textIndex { get; private set; } // 現在のテキストインデックス
    private Coroutine typingCoroutine;

    private bool finishText = false;

    private void Start()
    {
        storyText.text = "";
        characterName.text = "";
        SetStoryElement(storyIndex, textIndex);
    }

    private void Update()
    {
        {
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
                    textIndex++;
                    storyText.text = "";
                    finishText = false;
                    ProgressionStory(storyIndex);
                }
            }
        }
    }
    
    private void SetStoryElement(int _storyIndex, int _textIndex)
        {
            soundManager.PlayBGM(storyDatas[_storyIndex].bgm);
            var storyElement = storyDatas[_storyIndex].stories[_textIndex];

            background.sprite = storyElement.Background;
            characterImage.sprite = storyElement.CharacterImage;
            characterName.text = storyElement.CharacterName;
            
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
}