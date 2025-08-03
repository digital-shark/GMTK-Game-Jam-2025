using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HoverButton : MonoBehaviour
{
    public Image cursor;
    public Image ebutton;
    public TextMeshProUGUI info;

    public void activate(string text)
    {
        ebutton.gameObject.SetActive(true);
        cursor.gameObject.SetActive(false);
        info.text = text;
    }

    public void deactivate()
    {
        ebutton.gameObject.SetActive(false);
        cursor.gameObject.SetActive(true);
    }
}
