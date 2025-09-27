using UnityEngine;

public class MaquinaDeEstados : MonoBehaviour
{
    public enum Estados
    {
        idle, run, air, die
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
            print("Maincra");
            rb.AddForce(Physics.gravity * 1.25f, ForceMode.Acceleration);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (rb.linearVelocity.y < .1f || other.contactCount != 0)        // Si aterrizamos nos devuelve nuestro salto
        {
            miEstado = Estados.idle;
        }
    }

    private void OnColissionExit(Collision other)
    {
        if (/*other.gameObject.tag is "Suelo" || */other.contactCount == 0)
        {
            miEstado = Estados.air;
            rb.AddForce(Physics.gravity * 1.5f, ForceMode.Acceleration);
        }
    }
}
