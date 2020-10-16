using System.Collections.Generic;
using UnityEngine;

public struct NameAndScore
	{
	public string Name;
	public int Score;
		}

[CreateAssetMenu(fileName = "LeaderboardList", menuName = "Leaderboard List")]
public class LeaderboardList : ScriptableObject
{
    public List<NameAndScore> NamesAndScores = new List<NameAndScore>();
	public void ClearStats()
	{
		NamesAndScores.Clear();
	}

    public void AddNameAndScore(string name, int score)
	{
		NameAndScore leaderboardEntry = new NameAndScore();
		leaderboardEntry.Name = name;
		leaderboardEntry.Score = score;
		NamesAndScores.Add(leaderboardEntry);
		SortTheList();
	}

	private void SortTheList()
    {
        for (int i = 0; i < NamesAndScores.Count; i++)
        {
            for (int j = i; j < NamesAndScores.Count; j++)
            {
				if (NamesAndScores[i].Score < NamesAndScores[j].Score)
                {
					Swap(i, j);
                }
            }
			Debug.Log($"{NamesAndScores[i].Name} has score {NamesAndScores[i].Score}");
		}
		
	}

	private void Swap(int no1, int no2)
    {
		NameAndScore placeholder = NamesAndScores[no1];
		NamesAndScores[no1] = NamesAndScores[no2];
		NamesAndScores[no2] = placeholder;

    }
}
