using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public float RepairAmount = 10f;
    public float RefuelAmount = 10f;

    private LanderControllerClassic pLander;
    private PlayerCamera pCamera;
    private PlayerDamage pDamage;
    private MineralStorage pMinerals;
    private Mining pMiner;

    private bool isDead;

    private LandingPad landedPad;
    private bool isLockedToPlatform;

    private bool cameraIsMoving;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        pLander = GetComponent<LanderControllerClassic>();
        pCamera = GetComponent<PlayerCamera>();
        pDamage = GetComponent<PlayerDamage>();
        pMinerals = GetComponentInChildren<MineralStorage>();
        pMiner = GetComponentInChildren<Mining>();
    }

    public void LockLander(LandingPad thisPad)
    {
        isLockedToPlatform = true;
        pLander.LockToPlatform(RefuelAmount);
        pDamage.RepairShip(RepairAmount);
        pMinerals.UnloadMinerals(thisPad.ScoreMultiplier);
        pMiner.minerEnabled = false;
        landedPad = thisPad;
        StartCoroutine(WaitUntilCamera(true));
    }

    public void PrepareToReleaseLander()
    {
        if (!isLockedToPlatform) //Not fel med inputen, detta är ett hack
            return;
        isLockedToPlatform = false;
        StartCoroutine(WaitUntilCamera(false));
    }

    private void ReleaseLander()
    {
        pLander.UnlockFromPlatform();
        pMiner.minerEnabled = true;
		landedPad.Eject();
    }

    private IEnumerator WaitUntilCamera(bool landed)
    {
        cameraIsMoving = true;
        if(landed)
        {
            yield return StartCoroutine(pCamera.SwitchToWorld());
        }
        else
        {
            yield return StartCoroutine(pCamera.SwitchToPlayer());
            ReleaseLander();
        }
        cameraIsMoving = false;

    }

    public void DisableControls()
    {
        pLander.Dead();
    }

    public void LanderIsDead()
    {
        if (isDead)
            return;

        isDead = true;
        StartCoroutine(MusicPlayer.Instance.PlayOneShot("Fail"));
        pLander.Dead();
        StartCoroutine(DelayEndScreen());
    }

    IEnumerator DelayEndScreen()
	{
        yield return new WaitForSeconds(3f);
        EndScreen.instance.BringUpScoreScreen();
	}

    #region INPUTACTIONS

    public void BurnInput(InputAction.CallbackContext ctx)
    {
        if(isLockedToPlatform && !cameraIsMoving)
        PrepareToReleaseLander();
    }

    public void CheatMenuInput(InputAction.CallbackContext ctx)
    {
        CheatMenu.instance.TurnOnOff();
    }
    
    public void CheatMenuConfirmInput(InputAction.CallbackContext ctx)
    {
        CheatMenu.instance.ConfirmInput();
    }
    
    public void PauseMenuInput(InputAction.CallbackContext ctx)
    {
        UIManager.instance.pauseMenu.Toggle();
    }
    
    #endregion INPUTACTIONS
}
