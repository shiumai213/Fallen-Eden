using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputText : MonoBehaviour
{
    // Start is called before the first frame update
    public Text text;

    public void TextAppare()
    {
        //InputFieldに入力された文字を取得
        Text FieldText = GameObject.Find("InputField/Text").GetComponent<Text>();

        //InputFieldに入力された文字をテキストエリアに表示
        text.text = FieldText.text;

        //InputFieldに表示された文字を消す
        InputField column = GameObject.Find("InputField").GetComponent<InputField>();
        column.text = "";
    }
}
