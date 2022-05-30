using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public float Range;
    public float Speed;
    public int Damage;
    public LayerMask layers;
    public bool piercing;
    public bool spinning;
    [SerializeField] Transform spinObject;
    Vector3 previousPos;
    // Start is called before the first frame update
    void OnEnable()
    {

        StartCoroutine(ManualStart());
    }
    IEnumerator ManualStart()
    {
        yield return new WaitForEndOfFrame();
        previousPos = transform.position;
        gameObject.GetComponentInChildren<TrailRenderer>().Clear();
        Lean.Pool.LeanPool.Despawn(gameObject, Range);
    }
    // Update is called once per frame
    void Update()
    {
        if(spinning)
        {
            spinObject.localEulerAngles = new Vector3(90, spinObject.localEulerAngles.y + Speed * 50 * Time.deltaTime, 0);
        }
        transform.position += Speed * transform.forward * Time.deltaTime;
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(previousPos, transform.forward, out hit, Vector3.Distance(transform.position, previousPos),layers))
        {
            transform.position = hit.point;
        }
        previousPos = transform.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(!piercing)
        {
            Lean.Pool.LeanPool.Despawn(gameObject);

        }
    }

}
