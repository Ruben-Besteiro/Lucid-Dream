using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimiento : MonoBehaviour
{
    [SerializeField] float velMax = 10;
    [SerializeField] float sensibilidad = 1;
    Rigidbody rigi;
    bool salto;

    // Start is called before the first frame update
    void Start()
    {
        rigi = GetComponent<Rigidbody>();
        salto = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 vectorMov = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.rotation = Quaternion.Euler(Vector3.up * sensibilidad * Input.mousePosition.x);


        print(rigi.linearVelocity);
        if (Input.GetKey(KeyCode.W))
        {
            if (rigi.linearVelocity.z < velMax)
            {
                rigi.AddForce(vectorMov * 20f, ForceMode.Acceleration);
            } else
            {
                rigi.linearVelocity = new Vector3(rigi.linearVelocity.x, rigi.linearVelocity.y, velMax);
            }
        }
        else
        {
            if (rigi.linearVelocity.z > 0)
            {
                rigi.AddForce(vectorMov * -8f, ForceMode.Acceleration);
            }
        }
            //rigi.movePosition(transform.position + transform.right * speed * Time.fixedDeltaTime * Input.GetAxis("Horizontal");
            // Move position es para 2D
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Suelo")
        {
            salto = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Suelo")
        {
            salto = false;
        }
    }
}
