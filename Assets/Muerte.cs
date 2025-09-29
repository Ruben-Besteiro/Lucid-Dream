using TMPro;
using UnityEngine;

public class Muerte : MonoBehaviour
{
    [SerializeField] GameObject timer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag is "Player")
        {
            timer.GetComponent<Timer>().setTiempo(0);
        }
    }
}
