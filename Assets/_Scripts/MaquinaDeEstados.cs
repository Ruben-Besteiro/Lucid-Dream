using UnityEngine;

public class MaquinaDeEstados : MonoBehaviour
{
    public enum Estados
    {
        idle, move, air, die
    }
    public static Estados miEstado;

    void Start()
    {
        miEstado = Estados.idle;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag is "Suelo")
        {
            print("Has tocado el suelo");
            miEstado = Estados.idle;
        }
    }

    private void OnColissionExit(Collision other)
    {
        if (other.gameObject.tag is "Suelo")
        {
            print("Has saltado");
            miEstado = Estados.air;
        }
    }
}
