using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using TMPro;

public class LeaderboardController : MonoBehaviour
{
    public int memberId;
    int maxScores = 10;
    public TMP_Text[] entries;
    private void Start()
    {
        LootLockerSDKManager.StartGuestSession((response) => { 
        if(response.success)
            {
                Debug.Log("Loot Success");
                ShowScores();
            }
        else
            {
                Debug.Log("Faild");
            }
        
        
        });
    }
    public void ShowScores()
    {
        LootLockerSDKManager.GetScoreList(memberId,maxScores, (response) => {
            if (response.success)
            {
                LootLockerLeaderboardMember[] scores = response.items;
                for(int i = 0; i < scores.Length; i++)
                {

                    entries[i].text =   scores[i].score + "   " + scores[i].member_id.Substring(6) + "  <" + (scores[i]).rank;
                }
                if(scores.Length < maxScores)
                {
                    for(int i = scores.Length; i < maxScores; i++)
                    {
                        entries[i].text = "none"+ "  <" + (i + 1).ToString();
                    }
                }
            }
            else
            {
                Debug.Log("Faild");
            }


        });
    }
    public void SubmitScore()
    {
        if (TimeManager.playerName != "")
        {
            LootLockerSDKManager.SubmitScore("xxxxxx"+ TimeManager.playerName, Mathf.RoundToInt(TimeManager.score), memberId, (response) => {
                if (response.success)
                {
                    Debug.Log("Loot Success");
                }
                else
                {
                    Debug.Log("Faild");
                }
            });
        }
        else
        {
            LootLockerSDKManager.SubmitScore("xxxxxx"+"Guest" + Random.Range(1000, 9999), Mathf.RoundToInt(TimeManager.score), memberId, (response) => {
                if (response.success)
                {
                    Debug.Log("Loot Success");
                }
                else
                {
                    Debug.Log("Faild");
                }
            });
        }
        StartCoroutine(WaitShow());

    }
    IEnumerator WaitShow()
    {
        yield return new WaitForSeconds(2);
        ShowScores();
    }
}
