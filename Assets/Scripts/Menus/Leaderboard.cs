using TMPro;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    public TextMeshProUGUI LeaderboardText;
    public LeaderboardList TheList;

    public int MaxShown = 5;

    void Start()
    {
        LeaderboardText.text = "";
        int shownNames = Mathf.Min(MaxShown, TheList.NamesAndScores.Count);
  		for (int i = 0; i < shownNames; i++)
		{
            LeaderboardText.text += $"{TheList.NamesAndScores[i].Name}: {TheList.NamesAndScores[i].Score} points\n";
		}
    }


}
