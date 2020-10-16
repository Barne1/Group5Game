using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LanderControllerClassic : Knockable
{
    public RocketStats Stats;

    public bool UseGravity = true;

    public bool UseRealForce = true;

    public float BoostForce;
    public float BoostFuelUse;
    [Tooltip("If checked, boost is a straight velocity change \nIf unchecked, it's a force affected by mass")]
    public bool CrutchBoost;

    private bool lockedToPlatform;

    private Rigidbody rb;
    private bool hasDoneBoost;

    public float currentFuel;
    private FuelIndicator fuelIndicator;

    private FireBurnVFX fire;
    private AudioSource thrustSource;
    private float thrustVolume;

    private List<Collision> collisionsThisTick;

    [SerializeField, Range(0f, 100f)]
    float pushForce;

    #region MOVEMENTINPUT

    private float turnDirection;
    private float KBTurnInput;
    private Vector2 gamePadTurnDirection;
    private bool burnInput;
    private bool boostInput;
    private bool pushInput;

    private ModelRotate modelRotate;

    #endregion MOVEMENTINPUT
    
    private void Awake() {
        modelRotate = GetComponentInChildren<ModelRotate>();
        fire = GetComponentInChildren<FireBurnVFX>();
        thrustSource = SoundManager.Instance.PlayLoopingSoundAtCamera("Thrust", false).GetComponent<AudioSource>();
        
        thrustVolume = thrustSource.volume;
        thrustSource.pitch = 0.2f;
        thrustSource.volume = 0;
        
        collisionsThisTick = new List<Collision>();
    }

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.angularDrag = Stats.AngularDrag;
        currentFuel = Stats.MaxFuel;
        fuelIndicator = UIManager.instance.fuelIndicator;

    }

    private void FixedUpdate()
    {
        if (gamePadTurnDirection != Vector2.zero) {
            float dot = Vector3.Dot(-transform.right, gamePadTurnDirection);
            bool dotAlmostZero = dot > -0.01f && dot < 0.01f;
            if (!dotAlmostZero) {
                turnDirection = dot;
            }
        }
        else {
            turnDirection = KBTurnInput;
        }

        if(lockedToPlatform)
        {
            return;
        }
        if (burnInput)
        {
            Burn();
        }
        if (modelRotate != null) {
            modelRotate.SetModelRotation(turnDirection);
        }
        if (turnDirection != 0)
        {
            //Rotate();
            PhysicsRotate();
        }
        Boost();
        
        fire.UpdateFlame(burnInput);

        if (pushInput)
        {
            Debug.Log("pushing");
            PushAway();
        }
        collisionsThisTick.Clear();

        //rb.angularVelocity = Vector3.zero;
        if (UseGravity)
        {
            rb.AddForce(Vector3.down * Stats.Gravity, ForceMode.Acceleration);
        }
    }

    private void Update()
    {
        if (thrustSource.volume > 0)
        {
            thrustSource.volume -= thrustVolume / 60;
        }
    }

    private void Burn()
    {
        if(currentFuel > 0)
        {
            rb.AddForce(transform.up * Stats.BurnForce);
            currentFuel -= Stats.FuelBurnRate * Time.fixedDeltaTime;
            fuelIndicator.SetState(currentFuel/Stats.MaxFuel);
            thrustSource.volume = thrustVolume;
            if (currentFuel >= 0f)
                StartCoroutine(CallLosingForFuelEmpty());
        }
    }

    private void PhysicsRotate()
    {
        float rotationMod = turnDirection;

        //if (boostInput)
        //{
        //    rotationMod *= Stats.FasterRotationMod;
        //}
        if(UseRealForce)
        {
            rb.AddTorque(Vector3.forward * rotationMod * Stats.Torque, ForceMode.Force);
        }
        else
        {
            rb.AddTorque(Vector3.forward * rotationMod * Stats.Torque, ForceMode.Acceleration);

        }
    }

    private void PushAway()
    {
        Vector3 directionToPush = Vector3.zero;

        foreach (Collision collision in collisionsThisTick)
        {
            Vector3 awayFromCollider = Vector3.zero;
            foreach (ContactPoint contactPoint in collision.contacts)
            {
                Vector3 awayFromContact = contactPoint.normal;
                awayFromCollider += awayFromContact;
            }

            directionToPush += awayFromCollider;
        }
        
        rb.AddForce(directionToPush.normalized*pushForce, ForceMode.Impulse);
    }

    private void OnCollisionStay(Collision collision)
    {
        collisionsThisTick.Add(collision);
    }

    private void Rotate()
    {
        float rotationMod = turnDirection;

        if(boostInput)
        {
            rotationMod *= Stats.FasterRotationMod;
        }

        transform.RotateAround(transform.position, Vector3.forward, Stats.RotationSpeed * Time.fixedDeltaTime * rotationMod);
    }

    public void LockToPlatform(float RefuelAmount)
    {
        lockedToPlatform = true;
        UseGravity = true;
        currentFuel = Mathf.Min(currentFuel + RefuelAmount, Stats.MaxFuel);
        fuelIndicator.SetState(currentFuel/Stats.MaxFuel);

    }

    public void Dead()
	{
        lockedToPlatform = true;
	}

    public void UnlockFromPlatform()
    {
        lockedToPlatform = false;
        UseGravity = true;
    }
    
    #region INPUTACTIONS

    //each function corresponds to a mapping in player.inputactions
    public void KBMoveInput(InputAction.CallbackContext ctx)
    {
        KBTurnInput = -ctx.ReadValue<Vector2>().x;
    }
    
    //gamepad move input is separate since we read a vector2 to determine desired direction
    public void GamePadMoveInput(InputAction.CallbackContext ctx)
    {
        gamePadTurnDirection = ctx.ReadValue<Vector2>();
    }
    
    public void BurnInput(InputAction.CallbackContext ctx)
    {
        burnInput = ctx.ReadValueAsButton();
    }
    
    public void BoostInput(InputAction.CallbackContext ctx)
    {
        boostInput = ctx.ReadValueAsButton();
    }
    
    public void PushInput(InputAction.CallbackContext ctx)
    {
        pushInput = ctx.ReadValueAsButton();
    }
    
    #endregion INPUTACTIONS

    public void Boost()
    {
        if (lockedToPlatform)
            return;
        if (boostInput && !hasDoneBoost && currentFuel > BoostFuelUse)
		{
            currentFuel -= BoostFuelUse;
            hasDoneBoost = true;
            if(CrutchBoost)
                rb.AddForce(transform.up * BoostForce, ForceMode.VelocityChange);
			else
                rb.AddForce(transform.up * BoostForce, ForceMode.Impulse);
            fuelIndicator.SetState(currentFuel / Stats.MaxFuel);

        }

        if (!boostInput)
		{
            hasDoneBoost = false;
		}
    }

    private IEnumerator CallLosingForFuelEmpty()
    {
        yield return new WaitForSeconds(5);
        if(currentFuel <= 0f)
        PlayerManager.instance.LanderIsDead();
    }

    public void CheatSetFuel(float fuel) {
        currentFuel = fuel;
    }

    public override void Knock(Vector3 knockVelocity)
    {
        rb.velocity += knockVelocity;
    }
}
