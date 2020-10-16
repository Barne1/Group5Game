using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreText : MonoBehaviour
{

	public int Score { get { return score; } }
	private int score = 0;
	private Text scoreText;
	private void Awake()
	{
		scoreText = GetComponent<Text>();
		AddScore(0);
	}

	public void AddScore(int addedScore)
	{
		score += addedScore;
		scoreText.text = score.ToString();
	}
	public void CheatAddScore(float addedScore)
	{
		score += (int)addedScore;
		scoreText.text = score.ToString();
	}
}
