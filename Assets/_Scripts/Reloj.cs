using UnityEngine;

public class Reloj : MonoBehaviour
{
    GameObject timer;

    private void Start()
    {
        timer = GameObject.Find("Timer");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag is "Player")
        {
            timer.GetComponent<Timer>().setTiempo(timer.GetComponent<Timer>().getTiempo() + 10);
            Destroy(gameObject);
        }
    }
}
