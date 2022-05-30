using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MilkShake;
using Lean.Pool;
using UnityEngine.SceneManagement;
public class PlayerHealth : MonoBehaviour

{
    [SerializeField] SUPERCharacter.SUPERCharacterAIO Controller;
    [SerializeField] CardLoader Loader;
    bool HasDied;
    bool Invun;
    bool DeathMenuAnim;
    int DeathCount = 23;
    [SerializeField] CardInfo[] ListOfCards;
    [SerializeField] GameObject DeathMenu, CardHolder, CardPrefab;
    [SerializeField] GameObject[] CardsSpots;
    [SerializeField] TMP_Text DeathText, deathcounttext, FOVnumb, SensitiveNumb, Tip;
    [SerializeField] string[] tips;
    [SerializeField] SpriteRenderer DeathBackdrop;
    [SerializeField] Rigidbody rb;
    [SerializeField] DeckPosition deck;
    [SerializeField] ShakePreset Die;
    [SerializeField] AudioSource DieFX, ReviveFX;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] AudioLowPassFilter Filter;
    [SerializeField] Slider SensitivitySlider,FOVslider;
    bool StartRewinding;
    List<Vector3> RewindPositions = new List<Vector3>();
    List<CardInfo> CurrentCards = new List<CardInfo>();
    float oldTime;
    bool isPaused;
    bool GameEnd;
    public static int Sensitivity = 8,FOV = 80;
    [SerializeField] GameObject GameEndScreen;
    private void Start()
    {
        SensitivitySlider.value = Sensitivity;
        FOVslider.value = FOV;
    }
    private void Update()
    {
        Sensitivity = Mathf.RoundToInt(SensitivitySlider.value);
        FOV = Mathf.RoundToInt(FOVslider.value);
        Controller.Sensitivity = Sensitivity;
        Camera.main.fieldOfView = FOV;
        FOVnumb.text = FOV.ToString();
        SensitiveNumb.text = Sensitivity.ToString();
        if(!HasDied && Input.GetKeyDown(KeyCode.Escape) &&!GameEnd)
        {
            isPaused = !isPaused;
            if (isPaused)
            {
                Controller.lockAndHideMouse = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                oldTime = Time.timeScale;
                Time.timeScale = 0;
                PauseMenu.SetActive(true);
            }
            else
            {
                Controller.lockAndHideMouse = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = oldTime;
                PauseMenu.SetActive(false);
            }

        }
        if (HasDied && !GameEnd)
        {
            Filter.cutoffFrequency = Mathf.Lerp(Filter.cutoffFrequency, 700, 2*Time.unscaledDeltaTime);
            Time.timeScale = Mathf.Lerp(Time.timeScale, 0, 2 * Time.unscaledDeltaTime);
            if (Time.timeScale > 0.05f)
            {
                RewindPositions.Add(transform.position);
            }
            if (DeathMenuAnim)
            {
                if(Input.GetKeyDown(KeyCode.Escape))
                {
                    Revive();
                }
                else if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Alpha1))
                {
                    SelectCard(0);
                }
                else if (Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.Alpha2))
                {
                    SelectCard(1);

                }
                else if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Alpha3))
                {
                    SelectCard(2);

                }


                DeathText.rectTransform.localScale = Vector3.Lerp(DeathText.rectTransform.localScale, Vector3.one, 4 * Time.unscaledDeltaTime);
                DeathText.characterSpacing = Mathf.Lerp(DeathText.characterSpacing, 50, 2 * Time.unscaledDeltaTime);
                CardHolder.transform.localScale = Vector3.Lerp(CardHolder.transform.localScale, Vector3.one * 0.1f, 4 * Time.unscaledDeltaTime);
                DeathBackdrop.color = new Vector4(0, 0, 0, Mathf.Lerp(DeathBackdrop.color.a, 0.8f, 2 * Time.unscaledDeltaTime));
            }
        }
        else if(!isPaused && !GameEnd)
        {
            DeathText.rectTransform.localScale = Vector3.Lerp(DeathText.rectTransform.localScale, Vector3.zero, 3 * Time.unscaledDeltaTime);
            DeathText.characterSpacing = Mathf.Lerp(DeathText.characterSpacing, -23, 5 * Time.unscaledDeltaTime);
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1, 2 * Time.unscaledDeltaTime);
            Filter.cutoffFrequency = Mathf.Lerp(Filter.cutoffFrequency, 22000, Time.unscaledDeltaTime);
            CardHolder.transform.localScale = Vector3.Lerp(CardHolder.transform.localScale, Vector3.zero, 4 * Time.unscaledDeltaTime);
            DeathBackdrop.color = new Vector4(0, 0, 0, Mathf.Lerp(DeathBackdrop.color.a, 0f, 2 * Time.unscaledDeltaTime));
            if(DeathBackdrop.color.a < 0.02)
            {
                DeathMenu.SetActive(false);
            }
        }
        if (StartRewinding && !isPaused && !GameEnd)
        {
            if (RewindPositions.Count >= 1)
            {
                transform.position = RewindPositions[RewindPositions.Count - 1];
                RewindPositions.RemoveAt(RewindPositions.Count - 1);
            }
            else
            {
                StartCoroutine(ReviveTrue());
            }

        }
        if(GameEnd && Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("enemy") && !Invun && !GameEnd)
        {
            StartCoroutine(Death());
            Debug.Log("Hit");
            HasDied = true;
            if (collision.gameObject.GetComponent<Rigidbody>() != null)
            {
                rb.AddForce((new Vector3(collision.gameObject.GetComponent<Rigidbody>().velocity.x, 0, collision.gameObject.GetComponent<Rigidbody>().velocity.z).normalized + Vector3.up * 0.3f) * 40, ForceMode.Impulse);

            }
            else
            {
                rb.AddForce((new Vector3(collision.transform.forward.x, 0, collision.transform.forward.z).normalized + Vector3.up * 0.3f) * 40, ForceMode.Impulse);

            }
        }
    }
    IEnumerator Death()
    {
        DieFX.Play();
        Shaker.ShakeAll(Die);
       if (DeathCount - 1 < 0)
        {
            Invun = true;
            deck.isNotWorking = true;
            GameEnd = true;
            Loader.DisableLoading = true;
            Controller.controllerPaused = true;
            TimeManager.pause = true;
            Tip.text = tips[Random.Range(0, tips.Length)];
            Controller.lockAndHideMouse = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GameEndScreen.SetActive(true);
            MusicManager.currclip += 1;
            if (MusicManager.currclip == 3)
            {
                MusicManager.currclip = 0;
            }
            yield break;

        }

        deathcounttext.text = DeathCount.ToString();
        DeathMenuAnim = false;
        DeathText.rectTransform.localScale = Vector3.zero;
        DeathText.characterSpacing = -23;
        deck.isNotWorking = true;
        DeathBackdrop.color = new Vector4(0, 0, 0, 0);
        RewindPositions.Clear();
        Loader.DisableLoading = true;
        Controller.controllerPaused = true;
        Invun = true;

        yield return new WaitForSecondsRealtime(.7f);
        DeathText.gameObject.SetActive(true);
        for (int i = 0; i < 3; i++)
        {
            TMP_Text Desc = CardsSpots[i].GetComponentInChildren<TMP_Text>();
            GameObject GO;
            GO = LeanPool.Spawn(CardPrefab, CardsSpots[i].transform);

            GO.GetComponent<CardManager>().Card = ListOfCards[Random.Range(0, ListOfCards.Length)];
            Desc.text = GO.GetComponent<CardManager>().Card.Description;
            CurrentCards.Add(GO.GetComponent<CardManager>().Card);
        }
        DeathMenu.SetActive(true);
        DeathMenuAnim = true;
        yield return new WaitForSecondsRealtime(1f);
        DeathCount--;
        deathcounttext.gameObject.GetComponentInChildren<ParticleSystem>().Play();
        deathcounttext.text = DeathCount.ToString();
    }
    public void Revive()

    {       
        ReviveFX.Play();

        deck.isNotWorking = false;
        StartRewinding = true;
        HasDied = false;
        rb.velocity = Vector3.zero;

    
    }
    public IEnumerator ReviveTrue()
    {
        DeathText.gameObject.SetActive(false);

        CurrentCards.Clear();

        for (int i = 0; i < 3; i++)
        {
            if(CardsSpots[i].transform.childCount != 1)
            {
                LeanPool.Despawn(CardsSpots[i].transform.GetChild(1).gameObject);
            }

        }
            RewindPositions.Clear();
        StartRewinding = false;
        Loader.DisableLoading = false;
        Controller.controllerPaused = false;
        yield return new WaitForSeconds(3);
        Invun = false;
    }
    void SelectCard(int ID)
    {
 
        Loader.StoredCards.Add(CurrentCards[ID]);
        CardsSpots[ID].GetComponentInChildren<ParticleSystem>().gameObject.GetComponentInChildren<TMP_Text>().text = "";
        CardsSpots[ID].GetComponentInChildren<ParticleSystem>().Play();
        LeanPool.Despawn(CardsSpots[ID].transform.GetChild(1).gameObject);
        Revive();
    }

}
