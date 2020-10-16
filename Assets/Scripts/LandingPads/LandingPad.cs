using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(SphereCollider)), RequireComponent(typeof(Collider))]
public class LandingPad : MonoBehaviour
{
    //const values
    private const string playerTag = "Player";
    
    //Changeable in editor
    [Header("Landing Pad Pull")] [SerializeField]
    private Vector3 areaOffset;
    [SerializeField, Range(0f,20f)]
    private float radius = 2f;
    [SerializeField] 
    public bool active = true;
    [SerializeField, Range(0f,20f)] private float pullingForce = 10f;

    [Header("Docking & Alignment to Pad")]
    [SerializeField, Range(0f, 90f), Tooltip("The maximum angle deviancy allowed to dock")] 
    private float alignAngle = 30f;
    [SerializeField, Range(0f, 3f)] 
    private float alignCircleRadius;
    [SerializeField, Range(0f, 3f)] 
    private float secondsToDock = 1f;
    [SerializeField, Range(0, 20)] private int setScoreMultiplier = 1;
    public int ScoreMultiplier { get; private set; }

    [Header("Pad Ejection"), SerializeField] 
    private bool ejectEnabled = true;
    [SerializeField] private Vector3 ejectionDirection = Vector3.up;
    [SerializeField, Range(0f, 10f)] private float ejectionForce = 3f;
    [SerializeField, Range(0f, 10f)] private float secondsUntilEjection = 3f;

    [Header("Pad Floodlights"), SerializeField] 
    private GameObject activePlatformFloodlights;
    [SerializeField]private GameObject inactivePlatformFloodlights;
    private Light[] activeLights;
    

    [Header("Gizmos"), SerializeField] private bool drawGizmos = true;
    [SerializeField] private Vector3 alignGizmosOffset;

    //Gizmos references
    private SphereCollider pullTrigger;
    
    //private variables
    private Rigidbody playerBody;
    private bool playerWithinRange = false;
    private bool dockingInProgress = false;
    private bool playerDocked = false;
    private Coroutine dockingCoroutine;
    private Coroutine ejectCoroutine;

    private void Awake()
    {
        activeLights  = GetComponentsInChildren<Light>();
        ScoreMultiplier = setScoreMultiplier;
    }

    #region TRIGGER
    private void OnTriggerEnter(Collider other)
    {
        if (active && other.gameObject.CompareTag(playerTag))
        {
            playerBody = other.GetComponent<Rigidbody>();
            playerWithinRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            playerWithinRange = false;
        }
    }

    #endregion TRIGGER
    private void FixedUpdate()
    {
        LandingPadPull();
    }

    private void LandingPadPull()
    {
        if (active && playerWithinRange)
        {
            Vector3 forceTowardsPad = (transform.position - playerBody.position).normalized * pullingForce;
            playerBody.AddForce(forceTowardsPad);
        }
    }

    //set by landingPadCollision on child object with collider for pad
    public void TouchdownOnPad(Collision other)
    {
        if(!active)
		{
            return;
		}
        if (other.gameObject.CompareTag(playerTag))
        {
            if (playerBody == null)
            {
                playerBody = other.gameObject.GetComponent<Rigidbody>();
            }
            
            if (!dockingInProgress)
            {
                dockingInProgress = true;
                dockingCoroutine = StartCoroutine(Docking(other.transform));
            }
        }
    }
    
    //set by landingPadCollision on child object with collider for pad
    public void LeavePad(Collision other)
    {
        if (dockingCoroutine != null)
        {
            StopCoroutine(dockingCoroutine);
        }
        if (ejectCoroutine != null)
        {
            StopCoroutine(ejectCoroutine);
        }
        
        dockingInProgress = false;
        playerDocked = false;
    }

    public IEnumerator EjectCountdown()
    {
        yield return new WaitForSeconds(secondsUntilEjection);
        Eject();
    }

    public void Eject()
	{
        active = false;
        activePlatformFloodlights.SetActive(false);
        inactivePlatformFloodlights.SetActive(true);
        Vector3 actualEjectionDirection = transform.TransformDirection(ejectionDirection);

        //playerBody.gameObject.GetComponent<PlayerManager>().ReleaseLander();
        playerBody.AddForce(actualEjectionDirection * ejectionForce, ForceMode.Impulse);
        SoundManager.Instance.PlaySound("Release", transform.position);
    }

    private IEnumerator Docking(Transform player)
    {
        float timePassed = 0;
        while (timePassed < secondsToDock)
        {
            timePassed += Time.deltaTime;
            if (!CheckAlignment(player) || !CheckIfWithinCircle(player))
            {
                string dockingFailedReason =
                    CheckAlignment(player) ? "player not inside circle." : "player not aligned";
                
                dockingInProgress = false;
                break;
            }
            yield return null;
        }
        //if we didnt break docking
        if (dockingInProgress)
        {
            SoundManager.Instance.PlaySound("Landing", transform.position);
            StartCoroutine(MusicPlayer.Instance.PlayOneShot("Achievement"));
            player.GetComponent<PlayerManager>().LockLander(this);
            
            LandingPadManager.instance.CheckWin();

            playerDocked = true;
            foreach (Light l in activeLights)
            {
                l.color = Color.cyan;
            }

            if (ejectEnabled)
            {
                ejectCoroutine = StartCoroutine(EjectCountdown());
            }
        }
    }

    public void Activate() {
        this.active = true;
    }

    private bool CheckAlignment(Transform player)
    {
        return Vector3.Angle(transform.up, player.up) <= alignAngle;
    }

    private bool CheckIfWithinCircle(Transform player)
    {
        Vector3 localPlayerPos = transform.InverseTransformPoint(player.position);
        Vector2 localNoY = new Vector2(localPlayerPos.x, localPlayerPos.z);
        return localNoY.sqrMagnitude <= alignCircleRadius * alignCircleRadius;
    }

#if UNITY_EDITOR
	#region GIZMOS
	private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            DrawColliderGizmo();
            DrawAlignGizmo();
            DrawEjectGizmo();
        }
    }

    private void DrawColliderGizmo()
    {
        pullTrigger = GetComponent<SphereCollider>();

        pullTrigger.radius = radius;
        pullTrigger.center = areaOffset;
        
        Gizmos.matrix = transform.localToWorldMatrix;

        if (!active)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Collider[] colliders = Physics.OverlapSphere(transform.TransformPoint(Vector3.zero + areaOffset), radius);
            foreach (Collider c in colliders)
            {
                if (c.gameObject.tag == playerTag)
                {
                    Gizmos.color = Color.green;
                }
            }
        }

        Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.3f);
        Gizmos.DrawSphere(Vector3.zero + areaOffset, radius);

        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.white;
    }

    private void DrawAlignGizmo()
    {
        const int angleRadius = 1;
        Handles.matrix = transform.localToWorldMatrix;
        
        Handles.color = Color.magenta;
        Handles.DrawSolidDisc(Vector3.zero + alignGizmosOffset, Vector3.up, alignCircleRadius);
        
        Handles.color = dockingInProgress ? Color.green : Color.red;
        /*
        Vector3 startpoint2 = Quaternion.AngleAxis(alignAngle, new Vector3(1, 0, 0)) * Vector3.up;
        Handles.DrawSolidArc(Vector3.zero+ alignGizmosOffset, -Vector3.right, startpoint2, alignAngle * 2, angleRadius);
        */
        Vector3 startpoint1 = Quaternion.AngleAxis(alignAngle, new Vector3(0, 0, 1)) * (Vector3.up + alignGizmosOffset);
        Handles.DrawSolidArc(Vector3.zero+ alignGizmosOffset, Vector3.back, startpoint1, alignAngle * 2, angleRadius);
        
        Handles.color = Color.white;
        Handles.DrawLine(Vector3.zero + alignGizmosOffset, Vector3.up + alignGizmosOffset);
        
        
        Handles.matrix = Matrix4x4.identity;
    }

    private void DrawEjectGizmo()
    {
        Vector3 actualEjectionDirection = transform.TransformDirection(ejectionDirection);
        Gizmos.DrawLine(transform.position, transform.position + actualEjectionDirection * ejectionForce/2f);
    }

    #endregion GIZMOS
#endif
}
