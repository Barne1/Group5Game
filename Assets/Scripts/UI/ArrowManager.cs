using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ArrowManager : MonoBehaviour
{
    [SerializeField] int maxArrows = 3;
    [SerializeField] private bool updateEveryFrame = true;
    [SerializeField, Tooltip("If not updating every frame, how often to update")] 
    private float secondsToUpdate = 0.5f;

    [SerializeField] private bool onlyPointTowardsAvailablePad = true;
    [Header("DistanceOptions")]
    [SerializeField] private float maxDistance = 50f;
    [SerializeField] private float minDistance = 10f;
    [SerializeField] private float invisDistance = 5f;
    
    [SerializeField] private bool changeColorWithDistance = true;
    [SerializeField] private Color colorAtMaxDistance;
    [SerializeField] private Color colorAtMinDistance;
    
    [SerializeField] private bool changeScaleWithDistance = true;
    [SerializeField] private Vector3 scaleAtMaxDistance;
    [SerializeField] private Vector3 scaleAtMinDistance;

    [SerializeField] private GameObject arrowPrefab; 
    private Transform[] arrows;
    private Image[] arrowsGraphic;
    

    private Transform player;
    private LandingPadManager padManager;
    private Camera cam;

    private void Awake()
    {
        //setup for sqrDistance
        maxDistance *= maxDistance;
        minDistance *= minDistance;
        
        arrows = new Transform[maxArrows];
        for (int i = 0; i < maxArrows; i++)
        {
            arrows[i] = Instantiate(arrowPrefab, this.transform).transform;
            arrows[i].gameObject.SetActive(false);
        }

        if (changeColorWithDistance)
        {
            arrowsGraphic = new Image[maxArrows];
            for (int i = 0; i < maxArrows; i++)
            {
                arrowsGraphic[i] = arrows[i].GetComponentInChildren<Image>();
            }
        }
    }

    private void Start()
    {
        player = PlayerManager.instance.transform;
        padManager = LandingPadManager.instance;
        cam = Camera.main;

        StartCoroutine(UpdateArrows());
    }

    private void FixedUpdate()
    {
        transform.position = cam.WorldToScreenPoint(player.position);
    }

    IEnumerator UpdateArrows()
    {
        while (true)
        {
            Transform[] landingPads = padManager.GetClosestPads(true, player);

            for (int i = 0; i < arrows.Length; i++)
            {
                if (i > landingPads.Length - 1)
                {
                    arrows[i].gameObject.SetActive(false);
                }
                else
                {
                    arrows[i].gameObject.SetActive(true);
                    arrows[i].rotation = GetRotation(landingPads[i]);
                    if (changeColorWithDistance || changeScaleWithDistance)
                    {
                        float sqrDistance = GetDirection(landingPads[i].position).sqrMagnitude;
                        //T value expresses how far inbetween we are max and min
                        float tValue = Mathf.InverseLerp(minDistance, maxDistance, sqrDistance);

                        if (sqrDistance < invisDistance * invisDistance)
                        {
                            arrows[i].gameObject.SetActive(false);
                        }
                        else
                        {
                            if (changeColorWithDistance)
                            {
                                arrowsGraphic[i].color = Color.Lerp(colorAtMinDistance, colorAtMaxDistance, tValue);
                            }

                            if (changeScaleWithDistance)
                            {
                                arrowsGraphic[i].transform.localScale =
                                    Vector3.Lerp(scaleAtMinDistance, scaleAtMaxDistance, tValue);
                            }
                        }
                    }
                }
            }

            if (updateEveryFrame)
                yield return null;
            else
                yield return new WaitForSeconds(secondsToUpdate);
        }
    }

    private Quaternion GetRotation(Transform pad)
    {
        Vector2 direction = GetDirection(pad.position);
        return Quaternion.LookRotation(Vector3.forward, direction);
    }

    private Vector2 GetDirection(Vector3 padPos)
    {
        return (padPos - player.position);
    }
}
