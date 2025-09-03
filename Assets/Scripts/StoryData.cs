using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "StoryData")]
public class StoryData : ScriptableObject
{
    // ストーリーのリスト
    public List<Story> stories = new List<Story>();
    public SoundManager.BGM bgm;
    //public SEffectManager.SFX sfx;
}

[System.Serializable]
public class Story
{
    // 背景画像
    public Sprite Background;
    // キャラクター画像
    public Sprite CharacterImage;
    //キャラ画像2(必要に応じて使用)
    //public Sprite CharaImage2;
    //効果音
    public AudioClip SFX;
    // ストーリーテキスト（複数行対応）
    [TextArea]
    public string StoryText;
    // キャラクター名
    public string CharacterName;
    // 選択肢（未設定=通常の次行に進む）
    public List<ChoiceOption> Choices = new List<ChoiceOption>();
}

// 選択肢1件分のデータ
[System.Serializable]
public class ChoiceOption
{
    // ボタンに表示するテキスト
    public string Text;
    // 遷移先のStoryData配列インデックス（-1なら現在の章のまま）
    public int NextStoryIndex = -1;
    // 遷移先の行インデックス（-1ならデフォルトで次行）
    public int NextTextIndex = -1;
}
