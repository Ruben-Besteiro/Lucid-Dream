using UnityEngine;
using UnityEngine.InputSystem;

public class Movimiento : MonoBehaviour
{
    private _PLAYERACTIONS controls;
    private Vector2 vectorMov;
    private Rigidbody rb;

    [SerializeField] private float fuerzaMov = 40f;
    [SerializeField] private float velocidadMaxima = 10f;
    [SerializeField] private float fuerzaSalto = 5f;

    private void Awake()
    {
        controls = new _PLAYERACTIONS();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Evita que el rigidbody se caiga al inclinarse
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
        rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        Vector3 vectorMovVerdadero = new Vector3(vectorMov.x, 0f, vectorMov.y).normalized;

        if (vectorMovVerdadero != Vector3.zero)         // Esto chequea si estamos llamando a OnMovePerformed
        {
            rb.AddForce(vectorMovVerdadero * fuerzaMov, ForceMode.Acceleration);
        } else
        {
            rb.linearVelocity -= rb.linearVelocity / 10;        // Si no le damos a nada pierde un 10% de su velocidad por frame
        }

        // Limitamos velocidad máxima
        Vector3 velocidadPlano = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (velocidadPlano.magnitude > velocidadMaxima)
        {
            Vector3 velocidadLimitada = velocidadPlano.normalized * velocidadMaxima;
            rb.linearVelocity = new Vector3(velocidadLimitada.x, rb.linearVelocity.y, velocidadLimitada.z);
        }
    }
}
