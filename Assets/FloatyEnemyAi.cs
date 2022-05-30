using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class FloatyEnemyAi : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform playerPos;
    [SerializeField] float Speed;
    [SerializeField] ParticleSystem Charge;
    [SerializeField] float ChargeSpeedMultiplier = 1;
    [SerializeField] bool canCharge = true;
    [SerializeField] bool shootprojectiles;
    [SerializeField] GameObject ProjectileToShoot;
    [SerializeField] float shootingCooldown;
    [SerializeField] AudioSource AttackFX;

    //[SerializeField] ShakePreset EnemyAttack;
    bool Shooting;
    
    bool isCharging;

    // Update is called once per frame
    private void Start()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        if(canCharge)
        {
            StartCoroutine(ChargeTimer());
        }


    }
    void FixedUpdate()
    {if (!isCharging)
        {

            rb.AddForce((playerPos.position + Vector3.up * 0.6f + playerPos.forward * 1.3f - transform.position).normalized * Speed);
        }
    else
        {
            rb.AddForce(-rb.velocity);
        }
    if(shootprojectiles && !Shooting)
        {
            StartCoroutine(shoot());
        }
  

    }
    private void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation((playerPos.position + Vector3.up * 0.6f - transform.position)), 12 * Time.deltaTime);
    }
    IEnumerator ChargeTimer()
    {
        isCharging = false;
        yield return new WaitForSeconds(Random.Range(2f, 4f) * ChargeSpeedMultiplier);
        isCharging = true;
        AttackFX.Play();
        Charge.Play();
        yield return new WaitForSeconds(0.5f * ChargeSpeedMultiplier);
        //MilkShake.Shaker.ShakeAll(EnemyAttack);
        rb.AddForce((playerPos.position + Vector3.up * 0.6f - transform.position).normalized * Speed,ForceMode.Impulse);
        yield return new WaitForSeconds(1.5f * ChargeSpeedMultiplier);
        Charge.Play();

        AttackFX.Play();

        yield return new WaitForSeconds(0.5f * ChargeSpeedMultiplier);
        //MilkShake.Shaker.ShakeAll(EnemyAttack);

        rb.AddForce((playerPos.position + Vector3.up * 0.6f - transform.position).normalized * Speed, ForceMode.Impulse);
        StartCoroutine(ChargeTimer());
    }
    IEnumerator shoot()
    {
        Shooting = true;
        yield return new WaitForSeconds(shootingCooldown);
        Charge.Play();
        AttackFX.Play();

        yield return new WaitForSeconds(0.5f * ChargeSpeedMultiplier);
        //MilkShake.Shaker.ShakeAll(EnemyAttack);
        Lean.Pool.LeanPool.Spawn(ProjectileToShoot, transform.forward + transform.position, Quaternion.LookRotation((playerPos.position + Vector3.up * 0.4f - transform.position)));
        Shooting = false;

    }
}
