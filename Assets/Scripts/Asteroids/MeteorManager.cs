using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class MeteorManager : MonoBehaviour
{
    [Header("Spawn points")]
    [SerializeField] private float boundsSizeX;
    [SerializeField] private float boundsSizeY;
    [SerializeField] private float resolution = 5f;
    
    [Header("Settings")] 
    [SerializeField] private float magnitude = 5f;
    [SerializeField] private int interval = 30;
    private int intervalInFrames;
    
    private int noOfPointsX;
    private Vector3[] positions;
    private float timer;
    private AsteroidManager3D asteroidManager;
    
    private void OnValidate()
    {
        noOfPointsX = (int) (boundsSizeX / resolution);
        int noOfPointsY = (int) (boundsSizeY / resolution);
        positions = new Vector3[noOfPointsX*2 + noOfPointsY*2];
        
        positions[0] = new Vector3(-boundsSizeX/2, -boundsSizeY/2) + transform.position;
        positions[noOfPointsX] = new Vector3(boundsSizeX/2, -boundsSizeY/2)+ transform.position;
        positions[noOfPointsX*2] = new Vector3(-boundsSizeX/2, boundsSizeY/2)+ transform.position;
        positions[noOfPointsX*2+noOfPointsY] = new Vector3(boundsSizeX/2, boundsSizeY/2)+ transform.position;
      
        for (int i = 1; i < noOfPointsX; i++)
        {
            positions[i] = new Vector3(-boundsSizeX/2 + i * resolution, boundsSizeY/2, 0)+ transform.position;
            positions[i+noOfPointsX] = new Vector3(-boundsSizeX/2 + i * resolution, -boundsSizeY/2, 0)+ transform.position;
        }

        for (int i = 1; i < noOfPointsY; i++)
        {
            positions[i+noOfPointsX*2] = new Vector3(-boundsSizeX/2 , -boundsSizeY/2+  i * resolution, 0)+ transform.position;
            positions[i+noOfPointsX*2 + noOfPointsY] = new Vector3(boundsSizeX/2 , -boundsSizeY/2+  i * resolution, 0)+ transform.position;
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 position1 = new Vector3(-boundsSizeX/2, -boundsSizeY/2, 0)+ transform.position;
        Vector3 position2 = new Vector3(boundsSizeX/2, -boundsSizeY/2, 0)+ transform.position;
        Vector3 position3 = new Vector3(-boundsSizeX/2, boundsSizeY/2, 0)+ transform.position;
        Vector3 position4 = new Vector3(boundsSizeX/2, boundsSizeY/2, 0)+ transform.position;
        
        foreach (Vector3 position in positions)
        {
            Gizmos.DrawSphere(position, 1f);
        }
        
        Handles.DrawLine(position1, position2);
        Handles.DrawLine(position3, position4);
        Handles.DrawLine(position1, position3);
        Handles.DrawLine(position2, position4);
    }
#endif

    private void Awake()
    {
        intervalInFrames = (int) (interval / Time.deltaTime);
        timer = intervalInFrames;
        asteroidManager = GetComponent<AsteroidManager3D>();
    }

    private void Update()
    {
        timer--;

        if (timer == 0)
        {
            ThrowMeteor();
            timer += intervalInFrames;
        }
        
    }

    private void ThrowMeteor()
    {
        Vector3 position = positions[Random.Range(0, positions.Length)];
        Vector3 velocity = (Vector3.zero - position).normalized * magnitude;
        
        asteroidManager.SpawnAsteroid(position, velocity);
    }
}
