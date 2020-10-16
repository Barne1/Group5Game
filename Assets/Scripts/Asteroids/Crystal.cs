//@Author: Teodor Tysklind / FutureGames / Teodor.Tysklind@FutureGames.nu

using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class Crystal : MonoBehaviour
{
    private int minerals;
    
    private Asteroid3D asteroid;

    [SerializeField] private ParticleSystem onDestroyParticle;
    [SerializeField] private int minMinerals = default;
    [SerializeField] private int maxMinerals = default;
    
    [SerializeField] private GameObject crystalGraphicsVariantsParent;

    private CrystalGlow glow;
    private Collider[] colliders;
    
    private void Awake()
    {
        Assert.IsNotNull(crystalGraphicsVariantsParent);
        
        MeshRenderer[] crystalGraphicsVariants = crystalGraphicsVariantsParent.GetComponentsInChildren<MeshRenderer>();

        int graphicSelected = Random.Range(0, crystalGraphicsVariants.Length);
        crystalGraphicsVariants[graphicSelected].enabled = true;
        
        glow = GetComponent<CrystalGlow>();
        glow.SetRenderer(crystalGraphicsVariants[graphicSelected]);
        glow.Setup();

        asteroid = GetComponentInParent<Asteroid3D>();

        colliders = GetComponents<Collider>();
    }

    private void OnEnable()
    {
        minerals = Random.Range(minMinerals, maxMinerals + 1);
        
        foreach (Collider c in colliders) 
        {
            c.enabled = true;
        }
        
        glow.SetColor((float)minerals/maxMinerals);
    }
    
    
    private void Disable()
    {
        if (onDestroyParticle != null)
        {
            onDestroyParticle.Play();
        }

        SoundManager.Instance.PlaySound("CrystalBreak", transform.position);
        asteroid.RemoveCrystal(gameObject);
    }

    public int Mine(int miningMagnitude)
    {
        int minedMinerals;
        
        if (miningMagnitude < minerals)
        {
            minedMinerals = miningMagnitude;
        }
        else
        {
            minedMinerals = minerals;
        }

        minerals -= miningMagnitude;
        
        glow.SetColor((float)minerals/maxMinerals);
        
        if (minerals <= 0) 
        {
            foreach (Collider c in colliders) 
            {
                c.enabled = false;
            }
        }
        
        return minedMinerals;
    }

    private void Update()
    {
        if (minerals <= 0)
        {
            Disable();
        }
    }

    public void SetPosition()
    {
        transform.localPosition = transform.up/2 + transform.up * transform.localScale.x / 2;
    }

}
