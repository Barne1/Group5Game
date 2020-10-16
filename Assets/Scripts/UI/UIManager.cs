using UnityEngine;

public class UIManager : MonoBehaviour
{
    [System.NonSerialized]
    public static UIManager instance;
    public FuelIndicator fuelIndicator;

    public ScoreText scoreText;
    public StorageUI storageUI;

    public PauseMenu pauseMenu;
    
    private void Awake()
    {
        instance = this;
    }
}
