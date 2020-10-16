using UnityEngine;

public class SmokeVFX : MonoBehaviour
{
    [SerializeField, Range(0f,50f)] float minSmoke = 0f;
    [SerializeField, Range(0f, 50f)] private float maxSmoke = 25f;

    private ParticleSystem particleSystem;

    private void Awake() {
        particleSystem = GetComponent<ParticleSystem>();
    }

    public void SetSmokePercent(float percent) {
        float tValue = Mathf.Clamp(percent, 0, 1);
        float smokeDensity = Mathf.Lerp(minSmoke, maxSmoke, tValue);
        Debug.Log(smokeDensity);
        ParticleSystem.EmissionModule emission = particleSystem.emission;
        emission.rateOverTime = smokeDensity;
    }
}
