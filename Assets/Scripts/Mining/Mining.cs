using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mining : MonoBehaviour
{
    public RocketStats Stats;

    public bool minerEnabled = true;
    [SerializeField] private int crystalsMinedPerTick = 1;
    [SerializeField, Range(0f, 20f)] private float minMiningTime;
    [SerializeField, Range(0f, 20f)] private float maxMiningTime;
    
    private float timeToMine = 0.5f;
    private LineRenderer laser;
    
    [SerializeField] private bool drawGizmos = true;
    private List<Vector3> GizmosUnavailablePoints;
    private List<Vector3> GizmosAvailablePoints;
    
    [SerializeField, Header("Collider Type")]
    bool using2DColliders = false;
    private ContactFilter2D laserStoppedBy2D;
    
    [SerializeField] LayerMask crystalLayer;
    [SerializeField] private LayerMask laserStoppedBy;

    private bool currentlyMining = false;
    private Transform crystalTransform ;
    MineralStorage storage;

    private AudioSource audioSource;
    private float audioVolume;

    [SerializeField, Range(-3f, 3f)] private float minPitch = 1;
    [SerializeField, Range(-3f, 3f)] private float maxPitch = 1;

    private void Awake() {
        SetUp();
    }

    private void Start()
    {
        storage = MineralStorage.instance;
        audioSource = SoundManager.Instance.PlayLoopingSoundAtCamera("Mining").GetComponent<AudioSource>();
        audioVolume = audioSource.volume;
        audioSource.volume = 0;
    }

    void SetUp() {
        //used in OnDrawGizmos as well
        GizmosAvailablePoints = new List<Vector3>();
        GizmosUnavailablePoints = new List<Vector3>();
        
        if (using2DColliders) {
            laserStoppedBy2D = new ContactFilter2D();
            laserStoppedBy2D.useTriggers = false;
        }

        laser = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        laser.SetPosition(0, transform.position);
        if (crystalTransform != null)
        {
            laser.SetPosition(1, crystalTransform.position);
            timeToMine = GetTimeToMine(crystalTransform.position);
        }
        if (minerEnabled && !currentlyMining)
        {
            Crystal crystalToMine = FindClosestCrystal();
            if (crystalToMine != null) {
                crystalTransform = crystalToMine.transform;
                
                laser.enabled = true;

                timeToMine = GetTimeToMine(crystalTransform.position);
                
                StartCoroutine(MinerChargeUp(crystalToMine));
            }
            else {
                crystalTransform = null;
            }
        }
    }

    float GetTimeToMine(Vector3 crystalPosition) {
        float sqrDistanceToCrystal = (transform.position - crystalPosition).sqrMagnitude;
        
        //T value for interpolation between max distance and min distance from asteroid
        //right by crystal = 0, at max range = 1
        
        float percentFromMaxDistance = sqrDistanceToCrystal / (Stats.MiningLaserRange * Stats.MiningLaserRange);
        
        return Mathf.Lerp(minMiningTime, maxMiningTime, percentFromMaxDistance);
    }

    IEnumerator MinerChargeUp(Crystal crystal)
    {
        audioSource.volume = audioVolume;
        currentlyMining = true;
        float timePassed = 0;
        float percentComplete = 0;
        
        while (timePassed < timeToMine) {
            timePassed = percentComplete * timeToMine;
            timePassed += Time.deltaTime;
            
            percentComplete = timePassed / timeToMine;
            audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, percentComplete);

            bool inRange = (transform.position - crystalTransform.position).sqrMagnitude < Stats.MiningLaserRange * Stats.MiningLaserRange;
            bool inSight = CheckClearance(crystal.transform.position);
            bool crystalStillValid = inRange && inSight;
            
            if (!crystalStillValid) {
                currentlyMining = false;
                audioSource.volume = 0;
                break;
            }
            
            yield return null;
        }

        if (currentlyMining) {
            AddMinerals(crystal);
            audioSource.volume = 0;
            currentlyMining = false;
        }
        laser.enabled = false;
    }

    public void AddMinerals(Crystal crystal) {
        int crystalsToMine = Mathf.Min(crystalsMinedPerTick, storage.StorageLeft);
        int crystalsMined = crystal.Mine(crystalsToMine);
        storage.AddMineral(crystalsMined);
    }
    
    #region FINDINGCRYSTALS
    
    private Crystal FindClosestCrystal()
    {
        Crystal[] availableCrystals = GetAvailableCrystals();
        
        if (availableCrystals == null || availableCrystals.Length < 1)
        {
            return null;
        }
        
        Crystal closestCrystal = availableCrystals[0];
        float closestSqrMagnitude = float.MaxValue;
        foreach (Crystal crystal in availableCrystals) {
            float sqrDistance;
            if (!using2DColliders) {
                Vector3 vectorToCrystal = transform.position - crystal.transform.position;
                sqrDistance = vectorToCrystal.sqrMagnitude;
            }
            else {
                Vector2 vectorToCrystal = transform.position - crystal.transform.position;
                sqrDistance = vectorToCrystal.sqrMagnitude;
            }
            
            if (sqrDistance < closestSqrMagnitude)
            {
                closestSqrMagnitude = sqrDistance;
                closestCrystal = crystal;
            }
        }

        return closestCrystal;
    }
    
    private Crystal[] GetAvailableCrystals ()
    {
        Crystal[] inRangeCrystals = GetCrystalsInRange();
        
        if (inRangeCrystals.Length < 1)
        {
            return null;
        }
        
        List<Crystal> availableCrystals = new List<Crystal>();
        foreach (Crystal crystal in inRangeCrystals) {
            bool crystalLineOfSight = CheckClearance(crystal.transform.position);
            if (crystalLineOfSight) {
                availableCrystals.Add(crystal);
            }

            if (drawGizmos) {
                List<Vector3> gizmoList = crystalLineOfSight ? GizmosAvailablePoints : GizmosUnavailablePoints;
                gizmoList.Add(crystal.transform.position);
            }
        }
        return availableCrystals.ToArray();
    }

    bool CheckClearance(Vector3 crystalPosition) {
        if (!using2DColliders) {
            if (!Physics.Linecast(transform.position, crystalPosition, out RaycastHit hit, laserStoppedBy)) {
                return true;
            }
            else {
                return false;
            }
        }
        else {
            //2 since we only care if we hit 2 or more colliders
            RaycastHit2D[] results = new RaycastHit2D[2];
            int collidersHit = Physics2D.Linecast(transform.position, crystalPosition, laserStoppedBy2D, results);
            if (collidersHit <= 1) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    
    private Crystal[] GetCrystalsInRange()
    {
        Collider[] collidersHit = Physics.OverlapSphere(transform.position, Stats.MiningLaserRange, crystalLayer);
        if (collidersHit == null)
        {
            return null;
        }
        Crystal[] crystals = new Crystal[collidersHit.Length];
        
        for (int i = 0; i < collidersHit.Length; i++)
        {
            crystals[i] = collidersHit[i].GetComponent<Crystal>();
        }

        return crystals;
    }

    #endregion FINDINGCRYSTALS

    public void CheatMinMiningSpeed(float speed) {
        minMiningTime = speed;
    }
    public void CheatMaxMiningSpeed(float speed) {
        maxMiningTime = speed;
    }
    
    private void OnDrawGizmos()
    {
        SetUp();
        
        if (drawGizmos)
        {
            Gizmos.DrawWireSphere(transform.position, Stats.MiningLaserRange);
            
            Crystal closestCrystal = FindClosestCrystal();
            
            Gizmos.color = Color.green;
            foreach (Vector3 point in GizmosAvailablePoints)
            {
                Gizmos.DrawLine(transform.position, point);
            }
            Gizmos.color = Color.red;
            foreach (Vector3 point in GizmosUnavailablePoints)
            {
                Gizmos.DrawLine(transform.position, point);
            }
            
            if (closestCrystal != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(transform.position, closestCrystal.transform.position);
            }

            
            Gizmos.color = Color.white;
        }
    }
}
