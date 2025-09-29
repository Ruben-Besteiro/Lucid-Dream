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
        if (miEstado == Estados.air)
        {
            rb.AddForce(Physics.gravity * 1.25f, ForceMode.Acceleration);       // Aplicamos una gravedad personalizada
        }

        /*if (miEstado = Estados.wallRun && collision.contactCount == 0)
        {
            miEstado = Estados.air;
        }*/
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.9f)            // Esto se ejecuta el frame que aterrizamos
            {
                miEstado = Estados.idle;
            }
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if ((Mathf.Abs(rb.linearVelocity.x) > .1f || Mathf.Abs(rb.linearVelocity.z) > .1f) && (miEstado != Estados.air && miEstado != Estados.wallRun))
        {
            miEstado = Estados.run;
        }
        else if (miEstado != Estados.air)
        {
            miEstado = Estados.idle;
        }

        foreach (ContactPoint contact in other.contacts)
        {
            Vector3 normal = contact.normal;

            // Si alguna de sus colisiones es una pared 
            if ((Mathf.Abs(normal.x) > 0.75f || Mathf.Abs(normal.z) > 0.75f)/* && (new Vector2(rb.linearVelocity.x, rb.linearVelocity.z).magnitude) > 10*/)
            {
                miEstado = Estados.wallRun;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if ((Mathf.Abs(contact.normal.x) > 0.75f || Mathf.Abs(contact.normal.z) > 0.75f))
            {
                if (rb.linearVelocity.y == 0)       // Si estamos tocando el suelo
                {
                    miEstado = Estados.idle;
                } else
                {
                    miEstado = Estados.air;
                }
                return;
            }
        }

        if ((miEstado == Estados.wallRun && rb.linearVelocity.y != 0) || (miEstado == Estados.run && collision.contactCount == 0))
        {
            miEstado = Estados.air;
        }
    }
}