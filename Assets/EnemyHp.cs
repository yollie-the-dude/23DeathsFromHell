using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;
public class EnemyHp : MonoBehaviour
{
    Rigidbody rb;
    Transform playerPos;
    Material BaseMat;
    [SerializeField] Material WhiteMat;
    [SerializeField] ParticleSystem MeleeHit;
    [SerializeField] ParticleSystem RangedHit;
    [SerializeField] ShakePreset Hit, Die;
    [SerializeField] GameObject DeathParticle;
    [SerializeField] AudioSource HitFX;
    [SerializeField] GameObject DamageNumber;
    [SerializeField] bool startingtotem;
    [SerializeField] ParticleSystem startingParticles;
    [SerializeField] AudioSource Music;
    int Health;
    public int MaxHealth;

    bool isDead;
    bool hitStunned;

    private void Start()
    {
        Health = MaxHealth;
        BaseMat = GetComponentInChildren<MeshRenderer>().material;
        rb = gameObject.GetComponent<Rigidbody>();
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void Update()
    {
        MeleeHit.transform.LookAt(playerPos);
        RangedHit.transform.LookAt(playerPos);


    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("SwordPlayer"))
        {
            MeleeHit.Play();
            TakeDamage(other.GetComponent<SwordHitbox>().Damage);

            StartCoroutine(GetHit(30));
        }
        else if(other.CompareTag("PlayerProjectile"))
        {
            RangedHit.transform.forward = other.transform.forward;
            RangedHit.Play();
            TakeDamage(other.GetComponent<ProjectileManager>().Damage);
            StartCoroutine(GetHit(30));
        }
    }
    IEnumerator GetHit(float Knockback)
    {

        HitFX.Play();
        if (hitStunned)
        {
            gameObject.GetComponentInChildren<MeshRenderer>().material = WhiteMat;
            rb.constraints = RigidbodyConstraints.FreezePosition;
            yield return new WaitForSeconds(Knockback * 0.012f);
            rb.constraints = RigidbodyConstraints.None;
            gameObject.GetComponentInChildren<MeshRenderer>().material = BaseMat;
            rb.AddForce((playerPos.position - transform.position).normalized * -Knockback * 0.2f, ForceMode.Impulse);
            yield break;
        }

        Shaker.ShakeAll(Hit);
        gameObject.GetComponentInChildren<MeshRenderer>().material = WhiteMat;
        hitStunned = true;
        rb.constraints = RigidbodyConstraints.FreezePosition;
        yield return new WaitForSeconds(Knockback * 0.012f);
        rb.constraints = RigidbodyConstraints.None;
        gameObject.GetComponentInChildren<MeshRenderer>().material = BaseMat;
        hitStunned = false;

        rb.AddForce((playerPos.position - transform.position).normalized *-Knockback, ForceMode.Impulse);

    }

    void TakeDamage(int damage)
    {

        GameObject yes = Lean.Pool.LeanPool.Spawn(DamageNumber, transform.position, Quaternion.identity);
        yes.GetComponent<cardPosition>().Damage = damage;
        Health -= damage;
        if (Health <= 0 && !isDead)
        {
            if(startingtotem)
            {
                Music.Play();
                startingParticles.Play();
                TimeManager.pause = false;
            }
            Shaker.ShakeAll(Die);
            TimeManager.score += 100;
            isDead = true;
            Instantiate(DeathParticle, transform.position, Quaternion.identity);
            Destroy(gameObject, 0.1f);
        }

    }
}
