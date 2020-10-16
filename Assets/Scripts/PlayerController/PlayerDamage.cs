using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class PlayerDamage : MonoBehaviour
{
    public float MaxHealth, MinimumImpactForce;

    public float CurrentHealth;

    public bool IsInvincible;

    [SerializeField] private SmokeVFX smoke;
    [SerializeField] private SparksVFX sparks;

    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;
        UpdateVFX();
    }

    public void RepairShipFull()
    {
        CurrentHealth = MaxHealth;
        
    }

    public void RepairShip(float repairAmount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + repairAmount, MaxHealth);

    }

    private void OnCollisionEnter(Collision collision)
    {
        float impactStrength = collision.impulse.magnitude; //* Time.fixedDeltaTime;
        if (impactStrength > MinimumImpactForce)
        {
            string collisionString = "Collision";
            collisionString += UnityEngine.Random.Range(1, 5);
            SoundManager.Instance.PlaySound(collisionString, transform.position);
            CurrentHealth -= impactStrength - MinimumImpactForce;
            UpdateVFX();
            sparks.Play();
            if (CurrentHealth < 0f && !IsInvincible)
            {
                PlayerManager.instance.LanderIsDead();
            }
        }
    }

    private void UpdateVFX() {
        float percentEmpty = (MaxHealth - CurrentHealth) / MaxHealth;
        smoke.SetSmokePercent(percentEmpty);
    }

    public void CheatSetGodMode(float value)
    {
        if (value > 0.5f)
        {
            IsInvincible = true;
        }
        else
        {
            IsInvincible = false;
        }
    }
}
