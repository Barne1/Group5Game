using UnityEngine;

public class MineralStorage : MonoBehaviour
{
	public static MineralStorage instance;

	public int StorageLeft { get { return Stats.MaxMinerals - mineralsStored; } }
	private int mineralsStored;

	public RocketStats Stats;
	public GameObject[] BackpackMinerals;

	private Rigidbody rb;
	private ScoreText scoreText;
	private StorageUI storageIndicator;
	private Mining miner;

	private void Awake() {
		instance = this;
		miner = GetComponent<Mining>();
	}

	private void Start()
	{
		rb = GetComponentInParent<Rigidbody>();
		scoreText = UIManager.instance.scoreText;
		storageIndicator = UIManager.instance.storageUI;
		rb.mass = Stats.Mass;
		HideBackpackMinerals();

	}

	private void HideBackpackMinerals()
    {
		for (int i = 0; i < BackpackMinerals.Length; i++)
		{
			BackpackMinerals[i].SetActive(false);
		}
	}

	public void UnloadMinerals(int scoreMultiplier)
	{
		rb.mass = Stats.Mass;
		scoreText.AddScore(mineralsStored * scoreMultiplier);
		mineralsStored = 0;
		storageIndicator.SetState((float)mineralsStored/Stats.MaxMinerals);
		HideBackpackMinerals();


	}

	public void AddMineral(int addedMineral)
	{
		if(mineralsStored < Stats.MaxMinerals)
		{
			mineralsStored = Mathf.Min(mineralsStored + addedMineral, Stats.MaxMinerals);
			rb.mass += Stats.MineralWeight;
		}
		AddBackpackMineral();

		storageIndicator.SetState((float)mineralsStored/Stats.MaxMinerals);
		miner.minerEnabled = StorageLeft > 0;
	}

	private void AddBackpackMineral()
    {
		if (BackpackMinerals.Length == 0)
			return;
		//This is ugly
		int mineralTier = Stats.MaxMinerals/ BackpackMinerals.Length;
		mineralTier = Mathf.FloorToInt(mineralsStored / mineralTier);

        for (int i = 0; i < mineralTier; i++)
        {
			BackpackMinerals[i].SetActive(true);
        }

    }

	public void CheatAddMineral(float minerals) {
		AddMineral((int)minerals);
	}
}
