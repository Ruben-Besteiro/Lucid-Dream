using UnityEngine;

public class MaquinaDeEstados : MonoBehaviour
{
    public enum Estados
    {
        idle, run, air, wallRun
    }
    public static Estados miEstado;
    Rigidbody rb;

    void Start()
    {
        miEstado = Estados.idle;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((rb.linearVelocity.x > .1f || rb.linearVelocity.z > .1f) && miEstado != Estados.air)
        {
            miEstado = Estados.run;
        } else if (miEstado != Estados.air)
        {
            miEstado = Estados.idle;
        }

        if (miEstado == Estados.air)
        {
            rb.AddForce(Physics.gravity * 1.25f, ForceMode.Acceleration);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        foreach (ContactPoint contact in other.contacts)
        {
            Vector3 normal = contact.normal;

            // Si alguna de sus colisiones es una pared 
            if ((Mathf.Abs(normal.x) > 0.75f || Mathf.Abs(normal.z) > 0.75f)/* && (new Vector2(rb.linearVelocity.x, rb.linearVelocity.z).magnitude) > 20*/)
            {
                miEstado = Estados.wallRun;
            } else if (Mathf.Abs(normal.y) > 0.75f)      // Si es suelo
            {

            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.9f)
            {
                miEstado = Estados.idle;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if ((Mathf.Abs(contact.normal.x) > 0.75f || Mathf.Abs(contact.normal.z) > 0.75f))
            {
                return;
            }
        }
        miEstado = Estados.air;     // Esto solo se ejecuta si no está tocando el suelo (estamos en el aire)
    }
}