using UnityEngine;

public class Interactable : MonoBehaviour
{

    public string hint;

    public enum STATE 
    {
        UP,
        HELD,
        DOWN,
    }

    public STATE buttonState = STATE.UP;
    STATE oldState = STATE.UP;

    public float holdTime;
    public float timer;

    public bool interacting;
    bool toggle = true;

    public bool skipHeldPosition;

    public Vector3 Up;
    public Vector3 Held;
    public Vector3 Down;

    public bool usePosition;

    public Vector3 UpPos;
    public Vector3 HeldPos;
    public Vector3 DownPos;

    public delegate void timerDone();
    public event timerDone buttonTimerFinished;

    bool once = true;


    public void Interact() 
    {
        interacting = true;
    }

    public void stopInteract() 
    {
        interacting = false;
    }

    private void FixedUpdate()
    {
        switchTargetRotation(buttonState);
        if (usePosition)
            switchTargetPosition(buttonState);

        if (interacting)
        {
            if (toggle)
            {
                toggle = false;
                oldState = buttonState;
                buttonState = STATE.HELD;
                timer = holdTime;
            }

            timer -= Time.deltaTime;
            if (timer < 0 && once) 
            {
                once = false;
                buttonTimerFinished?.Invoke();
            }
        }
        else 
        {
            if (!toggle)
            {
                toggle = true;
                once = true;
                switch (oldState)
                {
                    case STATE.UP:
                        if (timer < 0)
                            buttonState = STATE.DOWN;
                        else
                            buttonState = STATE.UP;
                        break;
                    case STATE.HELD:
                    case STATE.DOWN:
                        buttonState = STATE.UP;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void switchTargetRotation(STATE state) 
    {
        switch (state)
        {
            case STATE.UP:
                transform.localRotation = Quaternion.Euler(Up);
                break;
            case STATE.HELD:
                transform.localRotation = Quaternion.Euler(skipHeldPosition ? Up : Held);
                break;
            case STATE.DOWN:
                transform.localRotation = Quaternion.Euler(Down);
                break;
            default:
                break;
        }
    }

    void switchTargetPosition(STATE state)
    {
        switch (state)
        {
            case STATE.UP:
                transform.localPosition = UpPos;
                break;
            case STATE.HELD:
                transform.localPosition = skipHeldPosition ? UpPos : HeldPos;
                break;
            case STATE.DOWN:
                transform.localPosition = DownPos;
                break;
            default:
                break;
        }
    }
}
