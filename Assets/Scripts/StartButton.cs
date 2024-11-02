using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickExample : MonoBehaviour
{
    public Button myButton;
    public bool GameGo = false;

    void Start()
    {        
        // ボタンにリスナーを追加
        myButton.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        // ボタンが押された時の処理
        GameGo = true;
        Debug.Log("ボタンが押されました！");
    }
}
