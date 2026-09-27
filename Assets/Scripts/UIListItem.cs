using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIListItem : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TMP_Text text;

    public void Set(Sprite sprite, string txt)
    {
        image.sprite = sprite;
        text.text = txt;
    }
}
