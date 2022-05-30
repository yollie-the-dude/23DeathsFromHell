using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;
using Lean.Pool;
public class CardLoader : MonoBehaviour
{
    //for activating cards
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject[] HandsCooldown;

    [SerializeField] GameObject[] Hands;
    float Cooldown;
    [SerializeField] GameObject SideBar;
    [SerializeField] GameObject CardPrefab;
    public List<CardInfo> StoredCards = new List<CardInfo>();
    [SerializeField] List<GameObject> CurrentCards = new List<GameObject>();
    [SerializeField] GameObject meleeHitBox;
    [SerializeField] ParticleSystem swordParticles;
    public bool DisableLoading;
    [SerializeField] ShakePreset Attack;
    [SerializeField] ParticleSystem Charge;
    [SerializeField] AudioSource Source;
    [SerializeField] AudioSource ChargeSound;
    bool isReloading;
    //symbol for the card types, doing it here since its hard to sync the symbol in case of changes
    public static Sprite[] CardTypeSymbols;
    [SerializeField] Sprite[] CardTypeSymbolsRef;
    private void Start()
    {
        ReloadCards();
        CardTypeSymbols = CardTypeSymbolsRef;
    }

    private void Update()
    {

        if(Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            isReloading = true;
            StartCoroutine(Reloading());
        }
        Cooldown -= Time.deltaTime;
        if(Cooldown < 0 && !DisableLoading)
        {
            for (int i = 0; i < 3; i++)
            {
                HandsCooldown[i].SetActive(false);
            }
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    StartCoroutine(Activate(0));
                }
                else
                {
                    RemoveCardAt(0);
                }
            }
            if (Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    StartCoroutine(Activate(1));
                }
                else
                {
                    RemoveCardAt(1);
                }
            }
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    StartCoroutine(Activate(2));
                }
                else
                {
                    RemoveCardAt(2);
                }
            }
        }
        else if(!DisableLoading)
        {
            for(int i = 0; i < 3; i++)
            {
                if(Hands[i].transform.childCount != 1)
                {
                    HandsCooldown[i].SetActive(true);
                    HandsCooldown[i].GetComponentInChildren<TMPro.TMP_Text>().text = (Mathf.RoundToInt(Cooldown * 100f) * 0.01f).ToString();
                }

            }
        }
        for (int i = 3; i < CurrentCards.Count; i++)
        {
            CurrentCards[i].transform.localPosition = Vector3.Lerp(CurrentCards[i].transform.localPosition , new Vector3(0, -0.27f + 0.1f * (i - 3)), 8 * Time.deltaTime);
        }



    }
    IEnumerator Activate(int ID)
    {
        if (Hands[ID].transform.childCount != 1)
        {
            Shaker.ShakeAll(Attack);
            Hands[ID].GetComponent<cardPosition>().UseCard();

            CardInfo CurrCard = Hands[ID].GetComponentInChildren<CardManager>().Card;
            CardManager localLoader = Hands[ID].GetComponentInChildren<CardManager>();


            Cooldown = CurrCard.cooldown;

            float localSpread = localLoader.localSpread;
            float localDamage = localLoader.localDamage;
            float localSize = localLoader.localSize;
            float localrepeat = localLoader.localRepeatamount;

            localLoader.localDurability--;
            Source.clip = CurrCard.Soundfx;
            Source.volume = CurrCard.Volume * 0.8F;


            if (localLoader.localDurability <= 0)
            {
                RemoveCardAt(ID);
            }

            float chargedVar = 0;
            if (CurrCard.Charged)
            {
                Charge.Play();
                ChargeSound.Play();
            }
            while (CurrCard.Charged && (Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.Alpha2) || Input.GetKey(KeyCode.Alpha3) || Input.GetMouseButton(0) || Input.GetMouseButton(2) || Input.GetMouseButton(1)))
            {
                yield return null;
                chargedVar += Time.deltaTime * 2;
                chargedVar = Mathf.Clamp(chargedVar, 1, 4);
            }
            ChargeSound.Stop();
            Charge.Stop();
            if (CurrCard.ChargeIncreasesRepeat)
            {
                localrepeat *= chargedVar * 2;
            }
            else if (CurrCard.Charged)
            {
                localDamage *= chargedVar;
            }
            if (CurrCard.OneshotSound)
            {
                Source.Play();
            }

            //Attacking
            if (CurrCard.isAttackCard)
            {
                for (int i = 0; i < localrepeat; i++)
                {
                    if(!CurrCard.OneshotSound)
                    {
                        Source.Play();
                    }

                    Shaker.ShakeAll(Attack);
                    if (CurrCard.Proj == ProjectileMethod.Projectile)
                    {
                        Vector3 Rand = (new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f) * localSpread);
                        GameObject GO = LeanPool.Spawn(CurrCard.ProjectileObj, transform.position, Quaternion.LookRotation(transform.forward + Rand));
                        GO.GetComponent<ProjectileManager>().Speed = CurrCard.ProjectileSpeed;
                        GO.GetComponent<ProjectileManager>().Range = localSize;
                        GO.GetComponent<ProjectileManager>().Damage = Mathf.RoundToInt(localDamage);

                    }
                    else if (CurrCard.Proj == ProjectileMethod.Melee)
                    {
                        swordParticles.transform.localEulerAngles = new Vector3(swordParticles.transform.localEulerAngles.x, swordParticles.transform.localEulerAngles.y, Random.Range(-30f, 30f));
                        swordParticles.transform.localScale = new Vector3(swordParticles.transform.localScale.x, .7f * localSize, swordParticles.transform.localScale.y);
                        swordParticles.Play();
                        meleeHitBox.transform.localScale = Vector3.one * localSize;
                        yield return new WaitForFixedUpdate();
                        meleeHitBox.GetComponent<SwordHitbox>().Damage = Mathf.RoundToInt(localDamage);
                        meleeHitBox.SetActive(true);
                        yield return new WaitForFixedUpdate();
                        meleeHitBox.SetActive(false);

                    }



                    rb.AddForce(Camera.main.transform.rotation * Vector3.back * CurrCard.Recoil);
                    yield return new WaitForSeconds(CurrCard.RepeatCooldown);
                }
            }
            else
            {
                Source.Play();
            }

            if (CurrCard.ApplyAdditionalCards)
            {
                AddCardsToCurrDeck(CurrCard.AdditionalCards);
            }
            if(CurrCard.ApplyBuffs)
            {


                if (CurrCard.OnlyApplyToHeld)
                {
                    for (int i = 2; i > -1; i--)
                    {
                        if (CurrentCards[i].GetComponentInChildren<CardManager>().Card.Thistype == CurrCard.StatApply && CurrentCards[i].GetComponentInChildren<CardManager>().Card != CurrCard)
                        {
                            CurrentCards[i].GetComponentInChildren<CardManager>().localDurability += CurrCard.durabilityIncrease;
                            CurrentCards[i].GetComponentInChildren<CardManager>().gameObject.GetComponentInChildren<ParticleSystem>().Play();
                            CurrentCards[i].GetComponentInChildren<CardManager>().localSpread += CurrCard.spread;
                            CurrentCards[i].GetComponentInChildren<CardManager>().localDamage += CurrCard.Damage;
                            CurrentCards[i].GetComponentInChildren<CardManager>().localSize += CurrCard.RangeOrSize;
                            CurrentCards[i].GetComponentInChildren<CardManager>().localRepeatamount += CurrCard.RepeatAmount;

                        }
                    }
                }
                else if(CurrCard.ApplyToSpecificName)
                {
                    for (int i = CurrentCards.Count - 1; i > -1; i--)
                    {
                        if (CurrentCards[i].GetComponentInChildren<CardManager>().Card.name.Contains(CurrCard.NameToApplyTo))
                        {
                            CurrentCards[i].GetComponentInChildren<CardManager>().localDurability += CurrCard.durabilityIncrease;
                            CurrentCards[i].GetComponentInChildren<CardManager>().gameObject.GetComponentInChildren<ParticleSystem>().Play();

                            CurrentCards[i].GetComponentInChildren<CardManager>().localSpread += CurrCard.spread;
                            CurrentCards[i].GetComponentInChildren<CardManager>().localDamage += CurrCard.Damage;
                            CurrentCards[i].GetComponentInChildren<CardManager>().localSize += CurrCard.RangeOrSize;
                            CurrentCards[i].GetComponentInChildren<CardManager>().localRepeatamount += CurrCard.RepeatAmount;

                        }
                    }
                }
                else
                {
                    for (int i = CurrentCards.Count - 1; i > -1; i--)
                    {
                        if (CurrentCards[i].GetComponentInChildren<CardManager>().Card.Thistype == CurrCard.StatApply)
                        {
                            CurrentCards[i].GetComponentInChildren<CardManager>().localDurability += CurrCard.durabilityIncrease;
                            CurrentCards[i].GetComponentInChildren<CardManager>().localSpread += CurrCard.spread;
                            CurrentCards[i].GetComponentInChildren<CardManager>().localDamage += CurrCard.Damage;
                            CurrentCards[i].GetComponentInChildren<CardManager>().localSize +=CurrCard.RangeOrSize;
                            CurrentCards[i].GetComponentInChildren<CardManager>().localRepeatamount += CurrCard.RepeatAmount;
                            CurrentCards[i].GetComponentInChildren<CardManager>().gameObject.GetComponentInChildren<ParticleSystem>().Play();

                        }
                    }
                }
            }


            if (CurrentCards.Count == 0)
            {

                StartCoroutine(Reloading());
            }


        }

    }
    void AddCardsToCurrDeck(CardInfo[] Cards)
    {
        for (int i = 0; i < Cards.Length; i++)
        {
            GameObject GO;
            if (CurrentCards.Count < 3)
            {

                if(Hands[0].transform.childCount !=1)
                {
                    GO = LeanPool.Spawn(CardPrefab, Hands[0].transform);
                }
                else if (Hands[1].transform.childCount != 1)
                {
                    GO = LeanPool.Spawn(CardPrefab, Hands[1].transform);
                }
                else
                {
                    GO = LeanPool.Spawn(CardPrefab, Hands[2].transform);
                }
                GO.transform.SetSiblingIndex(0);


            }
            else
            {
                GO = LeanPool.Spawn(CardPrefab, SideBar.transform);
                GO.transform.localScale /= 5;
                GO.GetComponentsInChildren<ParticleSystem>()[0].transform.localScale /= 5;

            }

            GO.GetComponent<CardManager>().Card = Cards[i];
            CurrentCards.Add(GO);
        }
    }
    void RemoveCardAt(int HandId)
    {
        if (Hands[HandId].transform.childCount != 1)
        {
            CurrentCards.RemoveAt(CurrentCards.IndexOf(Hands[HandId].transform.GetChild(0).gameObject));
            LeanPool.Despawn(Hands[HandId].transform.GetChild(0).gameObject);
            if (CurrentCards.Count - 2 > 0)
            {
                CurrentCards[2].GetComponentInChildren<ParticleSystem>().transform.localScale *= 5;


                CurrentCards[2].transform.localScale *= 5;
                CurrentCards[2].transform.parent = Hands[HandId].transform;
                CurrentCards[2].transform.localPosition = Vector3.zero;
                CurrentCards[2].transform.localRotation = Quaternion.identity;
                CurrentCards[2].transform.SetSiblingIndex(0);


            }

        }
 
    }
    IEnumerator Reloading()
    {
        isReloading = true;
        for (int i = CurrentCards.Count - 1; i > -1; i--)
        {
            LeanPool.Despawn(CurrentCards[i]);
            CurrentCards.RemoveAt(i);

        }
        
            yield return new WaitForSeconds(0.8f);
            ReloadCards();
        isReloading = false;
    }
    void ReloadCards()
    {
        for (int i = CurrentCards.Count - 1; i > -1; i--)
        {
            LeanPool.Despawn(CurrentCards[i]);
            CurrentCards.RemoveAt(i);

        }
        for (int i = 0; i < StoredCards.Count; i++)
        {
            CardInfo temp = StoredCards[i];
            int randomIndex = Random.Range(i, StoredCards.Count);
            StoredCards[i] = StoredCards[randomIndex];
            StoredCards[randomIndex] = temp;
        }
        for (int i =0; i < StoredCards.Count; i++)
        {
            GameObject GO;
            if (i < 3)
            {
                GO = LeanPool.Spawn(CardPrefab, Hands[i].transform);
                GO.transform.SetSiblingIndex(0);
            }
            else
            {

                GO = LeanPool.Spawn(CardPrefab, SideBar.transform);
                GO.transform.localScale /= 5;
                GO.GetComponentsInChildren<ParticleSystem>()[0].transform.localScale /= 5;




            }

            GO.GetComponent<CardManager>().Card = StoredCards[i];
            CurrentCards.Add(GO);
        }

    }
}
