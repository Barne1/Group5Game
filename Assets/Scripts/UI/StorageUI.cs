using UnityEngine;
using UnityEngine.UI;

public class StorageUI : MonoBehaviour {
    private Image[] states;
    private int maxState;
    private int currentState;

    private void Awake()
    {
        states = GetComponentsInChildren<Image>();
        maxState = states.Length-1;
        currentState = 0;
        states[currentState].enabled = true;
    }
    
    public void SetState(float percent)
    {
        int state = Mathf.CeilToInt(percent * (maxState - 0.99f));
        state = Mathf.Clamp(state, 0, maxState);
        if (state != currentState)
        {
            states[currentState].enabled = false;
            states[state].enabled = true;
            currentState = state;
        }
    }
}
