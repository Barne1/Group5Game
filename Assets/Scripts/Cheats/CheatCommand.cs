using UnityEngine.Events;

[System.Serializable]
public class CheatCommand
{
    public string commandName;
    public string description;
    public UnityEvent<float> actionToTrigger;

    public void Activate(float value)
    {
        actionToTrigger.Invoke(value);
    }
}
