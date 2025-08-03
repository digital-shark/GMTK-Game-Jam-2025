using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;

public class radio : MonoBehaviour
{
    public Interactable nextStation;
    public Interactable previousStation;
    public Interactable volumeUp;
    public Interactable volumeDown;

    public int station = 0;

    public EventReference buttonClick;

    public EventReference track1;
    public EventReference track2;
    public EventReference track3; 
    public EventReference track4;
    public EventReference track5;

    public EventInstance[] instances = new EventInstance[5];

    public float volume = 0.7f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        instances[0] = AudioManager.instance.createInstance(track1);
        instances[1] = AudioManager.instance.createInstance(track2);
        instances[2] = AudioManager.instance.createInstance(track3);
        instances[3] = AudioManager.instance.createInstance(track4);
        instances[4] = AudioManager.instance.createInstance(track5);

        instances[station].setVolume(volume);
        instances[station].start();
    }

    bool toggleone = true, toggleTwo = true, toggleThree = true, toggleFour = true;

    private void OnDestroy()
    {
        foreach (var item in instances)
        {
            item.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (nextStation.buttonState == Interactable.STATE.HELD && toggleone)
        {
            toggleone = false;
            instances[station].stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            station = Mathf.Min(instances.Length - 1, station + 1);
            instances[station].start();
        }
        else if (nextStation.buttonState == Interactable.STATE.UP)
        {
            toggleone = true;
        }

        if (previousStation.buttonState == Interactable.STATE.HELD && toggleTwo)
        {
            toggleTwo = false;
            instances[station].stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            station = Mathf.Max(0, station - 1);
            instances[station].start();
        }
        else if (previousStation.buttonState == Interactable.STATE.UP)
        {
            toggleTwo = true;
        }

        if (volumeUp.buttonState == Interactable.STATE.HELD && toggleThree)
        {
            toggleThree = false;
            volume = Mathf.Min(volume + 0.1f, 1);
            instances[station].setVolume(volume);
        }
        else if (volumeUp.buttonState == Interactable.STATE.UP)
        {
            toggleThree = true;
        }

        if (volumeDown.buttonState == Interactable.STATE.HELD && toggleFour)
        {
            toggleFour = false;
            volume = Mathf.Max(volume - 0.1f, 0);
            instances[station].setVolume(volume);
        }
        else if (volumeDown.buttonState == Interactable.STATE.UP)
        {
            toggleFour = true;
        }

    }
}
