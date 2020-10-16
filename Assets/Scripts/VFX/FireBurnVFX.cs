using UnityEngine;

public class FireBurnVFX : MonoBehaviour
{
    private const float maxSize = 20f;
    //input is converted in minLength and maxLength to make numbers represent flame size accurately
    [SerializeField, Range(0f, maxSize)] private float inputMinLength = 2f;
    [SerializeField, Range(0f, maxSize)] private float inputMaxLength = 18f;
    private float minLength;
    private float maxLength;

    [SerializeField] private bool changeSpeedWithBurn = true;
    [SerializeField, Range(0f, 30f)] private float minSpeed = 1f;
    [SerializeField, Range(0f, 30f)] private float maxSpeed = 10f;
    
    private Material fireMat;
    private static readonly int FirePowerOpacity = Shader.PropertyToID("_Fire_Power_Opacity");
    private static readonly int FireSpeed = Shader.PropertyToID("_Fire_Speed");
    
    //The seconds it takes for value to go from min to max, or vice versa
    [SerializeField, Range(0f,10f)] private float secondsToReachMax = 1f;
    //T value determines how much to interpolate between min and max speed and length
    private float tValue = 0;
    
    private void Awake()
    {
        fireMat = GetComponent<MeshRenderer>().sharedMaterial;
        //Conversion to make larger numbers correspond with larger flame
        minLength = maxSize - inputMinLength;
        maxLength = maxSize - inputMaxLength;
    }

    private void Start()
    {
        SetLength(minLength);
        SetSpeed(minSpeed);
    }

    public void UpdateFlame(bool burning)
    {
        float tValueChangeSpeed = Time.deltaTime / secondsToReachMax;
        tValue += burning ? tValueChangeSpeed : -tValueChangeSpeed;
        tValue = Mathf.Clamp(tValue, 0, 1f);

        float newLength = Mathf.Lerp(minLength, maxLength, tValue);
        SetLength(newLength);
        if (changeSpeedWithBurn)
        {
            float newSpeed = Mathf.Lerp(minSpeed, maxSpeed, tValue);
            SetSpeed(newSpeed);
        }
    }

    private void SetLength(float length)
    {
        fireMat.SetFloat(FirePowerOpacity, length);
    }

    private void SetSpeed(float speed)
    {
        Vector4 fireSpeed = new Vector4(0, speed, 0, 0);
        fireMat.SetVector(FireSpeed, fireSpeed);
    }
}
