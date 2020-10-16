using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    public static EndScreen instance;
	public ScoreText Score;
	public GameObject ScoreObject;
	public GameObject RetryObject;

	//text based on if you win or lose
	public TextMeshProUGUI endGameText;

	public LeaderboardList Leaderboard;
	public TextMeshProUGUI NameField;


	private void Start()
	{
		instance = this;
		ScoreObject.SetActive(false);
		RetryObject.SetActive(false);
	}

	public void BringUpScoreScreen(bool win = false)
	{
		PlayerManager.instance.DisableControls();
		ScoreObject.SetActive(true);
		if (win)
		{
			endGameText.text = "You Win!";
		}
	}

	public void EnterName()
	{
		Leaderboard.AddNameAndScore(NameField.text, Score.Score);
		ScoreObject.SetActive(false);
		RetryObject.SetActive(true);
	}
	private IEnumerator ResetDelay()
	{
		yield return new WaitForSeconds(3);
		BackToMainMenu();
	}

	public void BackToMainMenu()
	{
		SceneManager.LoadScene(0);
	}

	public void ResetScene()
	{
		SceneManager.LoadScene(1);
	}


}
