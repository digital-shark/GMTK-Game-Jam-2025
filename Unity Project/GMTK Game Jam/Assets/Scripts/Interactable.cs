using UnityEngine;

public class Interactable : MonoBehaviour
{
    public enum STATE 
    {
        UP,
        HELD,
        DOWN,
    }

    public STATE buttonState = STATE.UP;
    STATE oldState = STATE.HELD;

    public float holdTime;
    public float timer;

    public bool interacting;
    bool toggle = true;

    public Vector3 Up;
    public Vector3 Held;
    public Vector3 Down;

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
                buttonTimerFinished.Invoke();
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
                transform.rotation = Quaternion.Euler(Up);
                break;
            case STATE.HELD:
                transform.rotation = Quaternion.Euler(Held);
                break;
            case STATE.DOWN:
                transform.rotation = Quaternion.Euler(Down);
                break;
            default:
                break;
        }
    }
}
