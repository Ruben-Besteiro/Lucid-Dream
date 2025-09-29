using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] int tiempo;
    TextMeshProUGUI textoTiempo;
    [SerializeField] Image fade;

    public void setTiempo (int tiempoNuevo)
    {
        tiempo = tiempoNuevo;
    }

    void Start()
    {
        textoTiempo = GetComponent<TextMeshProUGUI>();
        fade.CrossFadeAlpha(0, 0, false);
        fade.GetComponent<Image>().enabled = true;

        switch (SceneManager.GetActiveScene().name)         // Cada nivel tendrá un límite de tiempo distinto
        {
            case "SampleScene":
                tiempo = 100;
                break;
            default:
                tiempo = 999999999;
                break;
        }
        textoTiempo.text = tiempo.ToString();
        StartCoroutine(cuentaAtras());
    }

    private void FixedUpdate()
    {
        if (tiempo <= 0)
        {
            textoTiempo.text = "Has muerto";
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().controls.Disable();     // Cuando morimos, los controles se desactivan
            fade.CrossFadeAlpha(1, .33f, true);
        }
    }

    IEnumerator cuentaAtras()
    {
        while (tiempo > 0)
        {
            yield return new WaitForSeconds(1);
            tiempo -= 1;
            textoTiempo.text = tiempo.ToString();
        }
    }
}
