using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioClip[] Clips;
    [SerializeField] AudioSource Source;
    public static int currclip = 0;
    bool firstplay;
    bool fadeout;
    [SerializeField] TMPro.TMP_Text artistText;
    [SerializeField] string[] artists;

    private void Start()
    {
        Source.clip = Clips[currclip];

    }
    private void Update()
    {
        if(fadeout)
        {
            artistText.color = new Vector4(0, 0, 0, Mathf.Lerp(artistText.color.a, 0, Time.deltaTime));
        }
        if (Source.isPlaying && !firstplay)
        {
            firstplay = false;
                StartCoroutine(ShowArtist());

        }
        if (!Source.isPlaying && !TimeManager.pause)
        {
            if (currclip == 2)
            {
                Source.clip = Clips[3];
            }
            Source.Play();
        }
    }
    IEnumerator ShowArtist()
    {
        artistText.text = artists[currclip];
        yield return new WaitForSeconds(2);
        fadeout = true;
    }
}
