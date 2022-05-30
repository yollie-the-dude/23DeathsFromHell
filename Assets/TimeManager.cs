using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public static float TimeLeft;
    public static int score;
    [SerializeField] TMPro.TMP_Text timeNo,timeNo2;
    public static string playerName;
    [SerializeField] TMPro.TMP_InputField Field;
    public static bool pause ;

    private void Start()
    {
        pause = true;
        StartCoroutine(PassiveScore());
        score = 0;
        TimeLeft = 0;
    }
    private void Update()
    {
        playerName = Field.text;
        if (!pause) timeNo.text = timeNo2.text = score.ToString() ;
        if (!pause) TimeLeft += Time.deltaTime;
    }
    IEnumerator PassiveScore()
    {
        yield return new WaitForSeconds(0.1f);
        if(!pause)
        {
            score += 10;
        }


        StartCoroutine(PassiveScore());
    }
}
