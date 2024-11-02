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
    [SerializeField] private Image characterImage2; //キャラ画像2
    //[SerializeField] private AudioClip SoundEffect; //効果音
    [SerializeField] private TextMeshProUGUI storyText; // ストーリーテキスト
    [SerializeField] private TextMeshProUGUI characterName; // キャラクター名
    public int storyIndex { get; private set; } // 現在のストーリーインデックス
    public int textIndex { get; private set; } // 現在のテキストインデックス

    private bool finishText = false; // テキストが全て表示されたかどうか
    private Coroutine typingCoroutine; // タイピングコルーチンの参照

    [SerializeField] private SoundManager soundManager;
    //[SerializeField] private SoundManager seffectManager;

    private void Start()
    {
        storyText.text = ""; // 初期化
        characterName.text = ""; // 初期化
        SetStoryElement(storyIndex, textIndex); // 最初のストーリー要素を設定
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // マウスの左ボタンが押されたとき
        {
            if (typingCoroutine != null) // タイピングコルーチンが実行中の場合
            {
                StopCoroutine(typingCoroutine); // コルーチンを停止
                ShowFullText(storyIndex, textIndex); // 残りのテキストを全て表示
            }
            else if (finishText) // テキストが全て表示されている場合
            {
                textIndex++; // 次のテキストへ
                storyText.text = ""; // テキストをクリア
                ProgressionStory(storyIndex); // ストーリーを進行
                finishText = false; // テキスト表示完了フラグをリセット
            }
        }
    }

    private void SetStoryElement(int _storyIndex, int _textIndex)
    {
        if (_storyIndex >= storyDatas.Length) // ストーリーインデックスが範囲外の場合
        {
            Debug.Log("全てのストーリーが終了しました"); // 終了メッセージを表示
            return;
        }

        //soundManager.PlayBGM(storyDatas[storyIndex].bgm);
        
        var storyElement = storyDatas[_storyIndex].stories[_textIndex]; // 現在のストーリー要素を取得

        if (storyElement.SFX != null)
        {
            // 効果音を再生
            AudioSource.PlayClipAtPoint(storyElement.SFX, transform.position); // ストーリー要素の効果音を再生
        }
        background.sprite = storyElement.Background; // 背景画像を設定
        characterImage.sprite = storyElement.CharaImage; // キャラクター画像を設定 
        characterImage2.sprite = storyElement.CharaImage2;　//キャラ画像2を設定
        characterName.text = storyElement.CharaName; // キャラクター名を設定
        typingCoroutine = StartCoroutine(TypeSentence(_storyIndex, _textIndex)); // タイピングコルーチンを開始
    }

    private void ProgressionStory(int _storyIndex)
    {
        if (_storyIndex >= storyDatas.Length) // ストーリーインデックスが範囲外の場合
        {
            Debug.Log("全てのストーリーが終了しました"); // 終了メッセージを表示
            return;
        }

        if (textIndex < storyDatas[_storyIndex].stories.Count) // テキストインデックスが範囲内の場合
        {
            SetStoryElement(storyIndex, textIndex); // 次のストーリー要素を設定
        }
        else
        {
            ChangeStoryElement(); // ストーリー要素を変更
        }
    }

    private void ChangeStoryElement()
    {
        textIndex = 0; // テキストインデックスをリセット
        storyIndex++; // ストーリーインデックスを進める
        if (storyIndex < storyDatas.Length) // ストーリーインデックスが範囲内の場合
        {
            SetStoryElement(storyIndex, textIndex); // 次のストーリー要素を設定
        }
        else
        {
            Debug.Log("全てのストーリーが終了しました"); // 終了メッセージを表示
        }
    }

    private IEnumerator TypeSentence(int _storyIndex, int _textIndex)
    {
        storyText.text = ""; // テキストをクリア
        foreach (var letter in storyDatas[_storyIndex].stories[_textIndex].StoryText.ToCharArray()) // 文字を一つずつ表示
        {
            storyText.text += letter; // 文字を追加
            yield return new WaitForSeconds(0.05f); // 0.05秒待機
        }
        finishText = true; // テキスト表示完了フラグを設定
        typingCoroutine = null; // コルーチンの参照をクリア
    }

    private void ShowFullText(int _storyIndex, int _textIndex)
    {
        storyText.text = storyDatas[_storyIndex].stories[_textIndex].StoryText; // 全てのテキストを表示
        finishText = true; // テキスト表示完了フラグを設定
        typingCoroutine = null; // コルーチンの参照をクリア
    }


}