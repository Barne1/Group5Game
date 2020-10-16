using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelIndicator : MonoBehaviour
{
    private Image[] states;
    private int maxState;
    private int currentState;

    private void Awake()
    {
        states = GetComponentsInChildren<Image>();
        maxState = states.Length-1;
        currentState = maxState;
        states[currentState].enabled = true;
    }

    //Gets value from 
    public void SetState(float percent)
    {
        int state = Mathf.CeilToInt(percent * maxState);
        if (state != currentState)
        {
            states[currentState].enabled = false;
            states[state].enabled = true;
            currentState = state;
        }
    }
}
