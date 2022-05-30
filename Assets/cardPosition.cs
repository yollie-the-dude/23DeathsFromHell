using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardPosition : MonoBehaviour
{
    [SerializeField] ParticleSystem part;
    [SerializeField] bool IsDamageNumber;
    public int Damage;
    [SerializeField] TMPro.TMP_Text numb;
    Vector3 RandomVector;
    void OnEnable()
    {

        StartCoroutine(ManualStart());
    }
    IEnumerator ManualStart()
    {
        yield return new WaitForEndOfFrame();
        if (IsDamageNumber)
        {
            RandomVector = Random(new Vector3(-1, 0, -1), new Vector3(1, 1, 1)) + transform.position;
            numb.text = Damage.ToString();
            numb.fontSize = 70* (Damage * 0.1f + 1);
            numb.color = new Vector4(1, 1, 1, 0);
            gameObject.GetComponent<RectTransform>().localScale *= 3f;
            Lean.Pool.LeanPool.Despawn(gameObject, 2);
        }
    }
    void Update()
    {
        if(!IsDamageNumber)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, 15 * Time.deltaTime);
        }
        else
        {
            gameObject.GetComponent<RectTransform>().localScale= Vector3.Lerp(gameObject.GetComponent<RectTransform>().localScale, Vector3.one * 0.006f, 4 * Time.deltaTime);
            if(Vector3.Distance(transform.position, RandomVector) < 0.05)
            {
                numb.color = new Vector4(1, 1, 1, Mathf.Lerp(numb.color.a, 0, 3 * Time.deltaTime));
            }
            else
            {
                numb.color = new Vector4(1, 1, 1, Mathf.Lerp(numb.color.a, 1, 6 * Time.deltaTime));
            }

            transform.position = Vector3.Lerp(transform.position, RandomVector, 2 * Time.deltaTime);
            
        }
        transform.LookAt(Camera.main.transform);
    }
    public void UseCard()
    {
        part.Play();
        transform.localPosition += Vector3.forward * 0.5f;
    }
    public Vector3 Random( Vector3 min, Vector3 max)
    {
        return((new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z))).normalized);
    }
}
