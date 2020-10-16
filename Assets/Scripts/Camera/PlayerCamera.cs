using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float DistanceFromPlayer;

    public Transform WorldPosition, FollowPosition;

    public float SwitchToWorldTime, SwitchToPlayerTime;
    public float WorldFoV, PlayerFoV;


    private Camera theCamera;
    private bool isInTransit;
    private bool isLanded;

    // Start is called before the first frame update
    void Start()
    {
        theCamera = Camera.main;
        PlayerFoV = theCamera.fieldOfView;

        //This is to prevent crashes if the prefab loses the world transform
        if(WorldPosition == null)
        {
            WorldPosition = FollowPosition;
        }
    }

    public IEnumerator SwitchToWorld()
    {
        isLanded = true;
        yield return StartCoroutine(SwitchCameraPositions(FollowPosition, WorldPosition, SwitchToWorldTime, WorldFoV));
    }

    public IEnumerator SwitchToPlayer()
    {
        isLanded = false;
        yield return StartCoroutine(SwitchCameraPositions(WorldPosition, FollowPosition, SwitchToPlayerTime , PlayerFoV));
    }

    void Update()
    {
        if(isInTransit)
        {
            return;
        }
        if(isLanded)
        {
            theCamera.transform.position = WorldPosition.position;
        }
        else
        {
            theCamera.transform.position = FollowPosition.position;
        }
    }


    private IEnumerator SwitchCameraPositions(Transform startPosition, Transform endPosition, float timeBetween, float FoV)
    {
        isInTransit = true;
        float originalFov = theCamera.fieldOfView;
        //This is an ugly solution switching cameras mid transition
        //I like working with lerps
        float time = timeBetween * Vector3.Distance(startPosition.position, theCamera.transform.position) / Vector3.Distance(startPosition.position, endPosition.position);
        Debug.Log(time);
        while (time < timeBetween)
        {
            time += Time.deltaTime;
            theCamera.transform.position = Vector3.Lerp(startPosition.position, endPosition.position, time / timeBetween);
            theCamera.fieldOfView = Mathf.Lerp(originalFov, FoV, time / timeBetween);
            yield return null;
        }
        isInTransit = false;
    }

}
