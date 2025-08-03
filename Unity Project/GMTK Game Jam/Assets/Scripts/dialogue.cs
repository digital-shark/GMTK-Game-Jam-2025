using UnityEngine;
using TMPro;
using System.Collections;

public class dialogue : MonoBehaviour
{
    public TextMeshProUGUI text;

    public string[] lines;
    public float textspeed;


    public int index;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text.text = string.Empty;
        startDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void startDialogue() 
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine() 
    {
        foreach (char c in lines[index].ToCharArray()) 
        {
            text.text += c;
            yield return new WaitForSeconds(textspeed);
        }
    }

    public void nextLine() 
    {
        if (index < lines.Length - 1) 
        {
            index++;
            text.text = string.Empty;
            StartCoroutine(TypeLine());
        }
    }

}
