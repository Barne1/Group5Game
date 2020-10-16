using UnityEngine;

public class SparksVFX : MonoBehaviour {
    private ParticleSystem sparks;

    private void Awake() {
        sparks = GetComponent<ParticleSystem>();
    }

    public void Play() {
        sparks.Play();
    }
}
