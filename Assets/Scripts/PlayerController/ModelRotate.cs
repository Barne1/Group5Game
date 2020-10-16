using UnityEngine;

public class ModelRotate : MonoBehaviour {
    private Quaternion defaultRotation;
    [SerializeField, Range(0f, 90f)] float maxRotation = 90f;
    [SerializeField, Range(0f,50f)] float rotationSpeed = 0.5f;
    private float rotationDirection = 0;
    private void Awake() {
        defaultRotation = transform.localRotation;
    }

    private void Update() {
        float frameRotation = Time.deltaTime * this.rotationSpeed;
        if (Mathf.Approximately(rotationDirection, 0)) {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, defaultRotation, frameRotation);
        }
        else {
            Quaternion desiredRotation = Quaternion.AngleAxis(maxRotation * rotationDirection, Vector3.up) * defaultRotation;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, desiredRotation, frameRotation);
        }
    }

    public void SetModelRotation(float rotationMod) {
        rotationDirection = rotationMod;
    }
}
