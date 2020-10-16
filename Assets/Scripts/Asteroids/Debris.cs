//@Author: Teodor Tysklind / FutureGames / Teodor.Tysklind@FutureGames.nu

using UnityEngine;
using UnityEngine.Assertions;

public class Debris : MonoBehaviour
{
    private int noOfDebris;
    private GameObject[] debrisToSpawn;
    [SerializeField] private int debrisPoolSize = 1000;

    [SerializeField] private int minNumberOfDebris = 5;
    [SerializeField] private int maxNumberOfDebris = 10;
    
    [SerializeField] private float minDebrisSize = 0.01f;
    [SerializeField] private float maxDebrisSize = 0.1f;

    [SerializeField] private float minDebrisMagnitude = 7f;
    [SerializeField] private float maxDebrisMagnitude = 10f;
    
    [SerializeField] private GameObject debrisPrefab;

    private void Awake()
    {
        Assert.IsNotNull(debrisPrefab);
        
        if (!PoolManager.Instance.PoolExists(debrisPrefab))
        {
            PoolManager.Instance.CreatePool(debrisPrefab, debrisPoolSize);
        }
        
        noOfDebris = Random.Range(minNumberOfDebris, maxNumberOfDebris + 1);
    }
    
    public void SpawnDebris()
    {
        for (int i = 0; i < noOfDebris; i++)
        {
            float debrisSize = Random.Range(minDebrisSize, maxDebrisSize);
            Vector3 scale = new Vector3(debrisSize,debrisSize,debrisSize);
            
            GameObject debris = PoolManager.Instance.ReuseObject(debrisPrefab, transform.position, Quaternion.identity);
            Rigidbody debrisbody = debris.GetComponent<Rigidbody>();

            debris.transform.localScale = scale;
            
            Vector3 velocity = Random.insideUnitCircle * Random.Range(minDebrisSize, maxDebrisMagnitude);
            debrisbody.velocity = velocity;
            debrisbody.mass = debrisSize;
            debrisbody.AddTorque(transform.forward * 10f);
        }
    }
}
