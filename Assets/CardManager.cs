using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class CardManager : MonoBehaviour
{
    public CardInfo Card;


    public float localDurability;
    public float localDamage;
    public float localRepeatamount;
    public float localSize;
    public float localSpread;

    
    [SerializeField] SpriteRenderer BaseImage, WeaponTypeSymbol;
    [SerializeField] TMP_Text CooldownTime, Name,Durability,Repetition,Attack;

    void OnEnable()
    {

        StartCoroutine(ManualStart());
    }
    IEnumerator ManualStart()
    {
        yield return new WaitForEndOfFrame();
        localDurability = Card.Durability;
        localDamage = Card.Damage;
        localRepeatamount = Card.RepeatAmount;
        localSize = Card.RangeOrSize;
        localSpread = Card.spread;
        Name.text = Card.CardName;
        CooldownTime.text = (Card.cooldown * 100).ToString();
        BaseImage.sprite = Card.CardSprite;
        WeaponTypeSymbol.sprite = CardLoader.CardTypeSymbols[(int)Card.Thistype];
        if (Card.turnSpriteDiagonal) BaseImage.transform.localEulerAngles = new Vector3(0, 180, -45);
        else BaseImage.transform.localEulerAngles = new Vector3(0, 180, 0);
    }
    private void Update()
    {
        Durability.text = localDurability.ToString();
        Repetition.text = localRepeatamount.ToString();
        Attack.text = localDamage.ToString();

    }

}
