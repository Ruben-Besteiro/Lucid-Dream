using UnityEngine;
using UnityEngine.InputSystem;

public class Movimiento : MonoBehaviour
{
    private _PLAYERACTIONS controls;
    private Vector2 vectorMov;
    private Rigidbody rb;

    [SerializeField] private float fuerzaMov = 40f;
    [SerializeField] private float velocidadMaxima = 10f;
    //[SerializeField] private float fuerzaSalto = 10;

    private void Awake()
    {
        controls = new _PLAYERACTIONS();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Move.performed += OnMovePerformed;
        controls.Player.Move.canceled += OnMoveCanceled;
        controls.Player.Jump.performed += OnJumpPerformed;
    }

    private void OnDisable()
    {
        controls.Player.Move.performed -= OnMovePerformed;
        controls.Player.Move.canceled -= OnMoveCanceled;
        controls.Player.Jump.performed -= OnJumpPerformed;
        controls.Player.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        vectorMov = ctx.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        vectorMov = Vector3.zero;
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        rb.AddForce(Vector3.up * 1000, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(Vector3.up * 10 * Time.deltaTime * Input.mousePosition.x);

        Vector3 vectorMovVerdadero = transform.TransformDirection(new Vector3(vectorMov.x, 0, vectorMov.y).normalized);     // Con TransformDirection pasamos de global a local

        if (vectorMovVerdadero != Vector3.zero)         // Esto chequea si estamos llamando a OnMovePerformed
        {
            rb.AddForce(vectorMovVerdadero * fuerzaMov, ForceMode.Acceleration);
        } else
        {
            rb.linearVelocity -= rb.linearVelocity / 10;        // Si no le damos a nada pierde un 10% de su velocidad por cada actualización de físicas
        }

        Mathf.Clamp(rb.linearVelocity.x, -velocidadMaxima, velocidadMaxima);        // Limitamos la velocidad (la velocidad máxima se puede cambiar al hacer acciones)
        Mathf.Clamp(rb.linearVelocity.y, -velocidadMaxima, velocidadMaxima);
        Mathf.Clamp(rb.linearVelocity.z, -velocidadMaxima, velocidadMaxima);
    }
}