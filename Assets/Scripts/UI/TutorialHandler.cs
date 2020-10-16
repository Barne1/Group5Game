using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHandler : MonoBehaviour
{
    public HasTheTutorialBeenShownAtStartYet YouKnowTheThing;
    public GameObject Tutorial;
    // Start is called before the first frame update
    void Start()
    {
        if(YouKnowTheThing.HasIt)
        {
            Tutorial.SetActive(true);
            YouKnowTheThing.HasIt = false;
        }
    }
}
