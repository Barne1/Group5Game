//@Author: Teodor Tysklind / FutureGames / Teodor.Tysklind@FutureGames.nu

using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class Asteroid3D : Knockable
{
    private Rigidbody body;
    private int noOfActiveCrystals = 0;
    private Crystal[] crystals;
    private Debris debris;
    private float diameter;
    
    [SerializeField] private float explosionMagnitude = 3f;

    [SerializeField] private float crystalsSize = 10f;
    [SerializeField] private float explosionOffset = 10f;
    [SerializeField] private GameObject ExplosionAnimationObject;
    [SerializeField] private float explosionDelay = 0.5f;
    [SerializeField] private float restimulateVelocityThreshold = 0.2f;
    [SerializeField] private float restimulationVelocity = 0.3f;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        debris = GetComponent<Debris>();
        crystals = GetComponentsInChildren<Crystal>();

        Assert.IsNotNull(crystals);
    }

    private void Update()
    {
        RestimulateVelocity();
    }
    
    private void Explode()
    {
        Collider[] collidersHit = Physics.OverlapSphere(transform.position, transform.localScale.x / 2 + explosionOffset);
        
        foreach (Collider collider in collidersHit)
        {
            Knockable knockable = collider.gameObject.GetComponent<Knockable>();
            
            if (knockable != null)
            {
                collider.gameObject.GetComponent<Knockable>().Knock((collider.transform.position-transform.position) * explosionMagnitude * 2);
            }
        }
        
        if (ExplosionAnimationObject != null)
        {
            GameObject explosionAnimationObject = Instantiate(ExplosionAnimationObject, transform.position, Quaternion.identity);
            explosionAnimationObject.transform.localScale = new Vector3(diameter/2, diameter/2, diameter/2);
            SoundManager.Instance.PlaySound("AsteroidExplosion", transform.position);
        }
        
        debris.SpawnDebris();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        noOfActiveCrystals = 0;
        
        foreach (Crystal crystal in crystals)
        {
            crystal.transform.localScale = new Vector3(crystalsSize,crystalsSize,crystalsSize);
            crystal.gameObject.SetActive(false);
        }
        
        foreach (Crystal crystal in crystals)
        {
            if (Random.Range(0f, 1f) < 0.5f)
            {
                crystal.gameObject.SetActive(true);
                noOfActiveCrystals++;
            }
        }
    }

    public void RemoveCrystal(GameObject crystal)
    {
        noOfActiveCrystals--;
        
        crystal.SetActive(false);

        if (noOfActiveCrystals == 0)
        {
            Invoke("Explode", explosionDelay);
        }
    }




    public void SetAsteroidSize(float diameter, float mass)
    {
        this.diameter = diameter;
        transform.localScale = new Vector3(diameter, diameter, diameter);
        body.mass = mass;

        foreach (Crystal crystal in crystals)
        {
            float asteroidSize = transform.localScale.x;
            float crystalSize = crystal.transform.localScale.x / asteroidSize;
            crystal.transform.localScale = new Vector3(crystalSize, crystalSize, crystalSize);
            
            crystal.SetPosition();
        }
    }
    
    
    private void RestimulateVelocity()
    {
        if (body.velocity.magnitude > restimulateVelocityThreshold)
        {
            return;
        }
        
        body.velocity = body.velocity.normalized * restimulationVelocity;
    }

    public override void Knock(Vector3 knockVelocity)
    {
        gameObject.GetComponent<Rigidbody>().velocity += knockVelocity;
    }
}
