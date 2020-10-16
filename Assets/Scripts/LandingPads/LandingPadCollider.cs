using UnityEngine;

public class LandingPadCollider : MonoBehaviour
{
    private LandingPad parentPad;

    private void Awake()
    {
        parentPad = GetComponentInParent<LandingPad>();
    }

    private void OnCollisionEnter(Collision other)
    {
        parentPad.TouchdownOnPad(other);
    }
    
    private void OnCollisionExit(Collision other)
    {
        parentPad.LeavePad(other);
    }
}
