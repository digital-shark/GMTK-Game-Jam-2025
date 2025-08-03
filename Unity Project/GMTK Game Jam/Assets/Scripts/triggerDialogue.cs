using UnityEngine;

public class triggerDialogue : MonoBehaviour
{
    public bool UseExit;
    public int targetLine;

    bool once = true;

    dialogue d;

    private void Start()
    {
        d = FindFirstObjectByType<dialogue>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (once && !UseExit)
        {
            d.nextLine();
            once = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (once && UseExit && targetLine == d.index+1)
        {
            d.nextLine();
            once = false;
        }
    }
}
