using UnityEngine;
using TMPro;

public class TextoEstado : MonoBehaviour
{
    private void FixedUpdate()
    {
        GetComponent<TextMeshProUGUI>().text = MaquinaDeEstados.miEstado.ToString();
    }
}
