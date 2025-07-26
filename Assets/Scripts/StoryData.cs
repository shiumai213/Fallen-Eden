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
    
}
