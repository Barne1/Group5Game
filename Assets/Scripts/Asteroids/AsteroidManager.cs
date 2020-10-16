//@Author: Teodor Tysklind / FutureGames / Teodor.Tysklind@FutureGames.nu

using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidManager : MonoBehaviour
{
    private PoolManager poolManager;
    private Vector2[] rightAngleDirections = new Vector2[] {Vector2.up, Vector2.right, Vector2.left, Vector2.down};
    
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private int asteroidPoolSize = 100;
    [SerializeField] private GameObject spawnPositionsParent;
    
    [Header("Settings for Asteroids spawning on start")]
    
    [Tooltip("Asteroids will spawn with a random velocity of a magnitude between these values")]
    [SerializeField] private float minMagnitude = 0.2f;
    [Tooltip("Asteroids will spawn with a random velocity of a magnitude between these values")]
    [SerializeField] private float maxMagnitude = 2f;

    [Tooltip("Asteroids randomized scale, 1 through this value")] 
    [SerializeField] private float minScale = 1f;
    [SerializeField] private float maxScale = 4f;

    private void Awake()
    {
        poolManager = PoolManager.Instance;
        poolManager.CreatePool(asteroidPrefab, asteroidPoolSize);

        SpawnAsteroidsOnStart();
    }

    private void SpawnAsteroidsOnStart()
    {
        Transform[] spawnPositions = spawnPositionsParent.GetComponentsInChildren<Transform>();
        
        foreach (Transform spawnTransform in spawnPositions)
        {
            GameObject asteroid = poolManager.ReuseObject(asteroidPrefab, spawnTransform.position, Quaternion.identity);

            asteroid.GetComponent<Rigidbody2D>().velocity = randomizeVelocity();

            float size = Random.Range(minScale, maxScale);
            
            asteroid.transform.localScale = new Vector3(size,size,size);
        }
    }

    private Vector2 randomizeVelocity()
    {
        Vector2 direction = Random.insideUnitCircle.normalized;
        Vector2 velocity = direction * Random.Range(minMagnitude, maxMagnitude);

        return velocity;
    }
}
