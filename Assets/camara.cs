using UnityEngine;

public class Camara : MonoBehaviour
{
    public float sensibilidad = 10f;
    private float rotacionX = 0f;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensibilidad * 2 * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidad * Time.deltaTime;

        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, -90f, 90f); // limitar para no girar 360°

        transform.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);

        GameObject.FindGameObjectWithTag("Player").transform.Rotate(Vector3.up * mouseX);
    }
}