using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckPosition : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Vector3 Velocity;
    [SerializeField] Vector3 PositionVector;
    [SerializeField] Vector2 MousePos;
    [SerializeField] float MouseMultiplier;
    [SerializeField] float RigidMultiplier;

    [SerializeField] Vector3 Velocitymultiplier;

    [SerializeField] float Smooth;
    [SerializeField] float VelocitySmooth;
    public bool isNotWorking;
    bool internalNotWorking;
    Vector3 lastpos;


    private void Update()
    {
        if(!isNotWorking)
        {
            internalNotWorking = true;
            Velocity = Vector3.Lerp(Velocity, Vector3.zero, VelocitySmooth * (rb.velocity.magnitude + 1) * Time.deltaTime);
            Velocity = Quaternion.Inverse(transform.rotation) * rb.velocity * RigidMultiplier;
            //MousePos = new Vector2(Input.GetAxisRaw("Mouse X") * MouseMultiplier, Input.GetAxisRaw("Mouse Y") * MouseMultiplier);
            PositionVector = Vector3.Lerp(PositionVector, -(Vector3)MousePos + (new Vector3(Velocity.x * Velocitymultiplier.x, Velocity.y * Velocitymultiplier.y, Velocity.z * Velocitymultiplier.z)), Smooth * (rb.velocity.magnitude * 0.3f + 1) * Time.deltaTime);
            transform.localPosition = PositionVector;
        }
        else
        {
            if(internalNotWorking)
            {
                lastpos = transform.position;
                internalNotWorking = false;
            }

            transform.position = lastpos;
            PositionVector = transform.localPosition;
        }

    }

}
