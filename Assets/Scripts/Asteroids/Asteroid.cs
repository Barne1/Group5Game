//@Author: Teodor Tysklind / FutureGames / Teodor.Tysklind@FutureGames.nu

using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Asteroid : MonoBehaviour
{
    private Rigidbody2D body;
    private float unhookMagnitude = 1f;
    
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        Transform[] crystals = GetComponentsInChildren<Transform>();

        List<Crystal> activeCrystals = new List<Crystal>(); 

        foreach (Transform t in crystals)
        {
            if (Random.Range(0f, 1f) < 0.5f)
            {
                t.gameObject.SetActive(false);
            }
        }
    }
    
    private void Explode()
    {
        
    }

    private void OnEnable()
    {
        
    }

    private void Update()
    {
        if (body.velocity.magnitude < 0.5f)
        {
            //UnHook();
        }
    }

    private void UnHook()
    {
        body.velocity = (Random.insideUnitCircle.normalized * unhookMagnitude);

    }

    private void ActivateCrystal()
    {
        
    }
}
