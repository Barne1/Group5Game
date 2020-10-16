using UnityEngine;

public class CrystalGlow : MonoBehaviour {
    [SerializeField] private Color emptyColor = Color.black;
    [SerializeField] private Color fullColor = Color.magenta;
    [SerializeField, Range(0f, 100f)] private float fullIntensity = 1f;

    private Renderer renderer;
    private MaterialPropertyBlock propertyBlock;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    public void SetRenderer(Renderer r) {
        renderer = r;
    }

    public void Setup() {
        propertyBlock = new MaterialPropertyBlock();
    }
    
    public void SetColor(float tValue) {
        renderer.GetPropertyBlock(propertyBlock);
        
        Color newColor = Color.Lerp(emptyColor, fullColor, tValue);
        float intensity = Mathf.Lerp(0, fullIntensity, tValue);
        propertyBlock.SetColor(EmissionColor, newColor * intensity);
        
        renderer.SetPropertyBlock(propertyBlock);
    }
}
