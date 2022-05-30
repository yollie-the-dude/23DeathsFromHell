using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileMethod
{
    Null,
    Projectile,
    Melee,
}

public enum CardType
{
    Melee,
    Ranged,
    Throwing,
    Support,
}
[CreateAssetMenu(fileName = "Card", menuName
= "Cards/New Card")]
public class CardInfo : ScriptableObject
{
    public string CardName;
    public CardType Thistype;
    public float cooldown;
    public bool isAttackCard = true;
    public Sprite CardSprite;
    public bool turnSpriteDiagonal;
    public string Description;

    public AudioClip Soundfx;
    public float Volume;
    public bool OneshotSound;


    [Header("General settings")]
    public int Durability = 1;

    [Header("Attack Settings")]
    public int Damage;
    public int RepeatAmount;
    public bool Charged;
    public bool ChargeIncreasesRepeat;
    public float RepeatCooldown;
    public float Recoil;

    [Header("Projectile Settings")]
    public ProjectileMethod Proj;
    public GameObject ProjectileObj;
    public float spread;
    public float ProjectileSpeed;
    public float RangeOrSize;

    [Header("Support Settings")]
    public bool ApplyAdditionalCards;
    public bool ApplyBuffs;
    public int durabilityIncrease;
    public CardInfo[] AdditionalCards;
    public CardType StatApply;
    public bool OnlyApplyToHeld;
    public bool ApplyToSpecificName;
    public string NameToApplyTo;

}
