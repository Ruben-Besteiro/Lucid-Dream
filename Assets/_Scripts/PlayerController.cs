using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private _PLAYERACTIONS controls;
    private Vector2 vectorMov;
    private Rigidbody rb;

    [SerializeField] private float fuerzaMov = 10f;
    [SerializeField] private const float velocidadMaxima = 150f;
    private float fuerzaSalto;
    [SerializeField] private float fuerzaSaltoReal = 0;
    [SerializeField] private const float fuerzaSaltoMaxima = 2500;
    [SerializeField] Image forceCurrentImage;

    private void Awake()
    {
        controls = new _PLAYERACTIONS();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        forceCurrentImage.fillAmount = 0;
        //GetComponent<MeshRenderer>().enabled = false;       // Hacemos que el personaje sea invisible para que sea solo la cámara
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Move.performed += OnMovePerformed;
        controls.Player.Move.canceled += OnMoveCanceled;
        controls.Player.Jump.performed += OnJumpPerformed;
        controls.Player.Jump.canceled += OnJumpCanceled;
    }

    private void OnDisable()
    {
        controls.Player.Move.performed -= OnMovePerformed;
        controls.Player.Move.canceled -= OnMoveCanceled;
        controls.Player.Jump.performed -= OnJumpPerformed;
        controls.Player.Jump.canceled -= OnJumpCanceled;
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

    private void OnJumpPerformed(InputAction.CallbackContext ctx)       // Solo ocurre una vez
    {
        fuerzaSalto = fuerzaSaltoMaxima / 20;
    }

    private void OnJumpCanceled(InputAction.CallbackContext ctx)        // Cuanto más rápido vayas, más alto saltas
    {                                                                   // Esto es una feature, no un bug ( ͡° ͜ʖ ͡°) (arreglado creo)
        if (fuerzaSaltoReal > fuerzaSaltoMaxima)
        {
            fuerzaSaltoReal = fuerzaSaltoMaxima;
        }

        rb.AddForce(Vector3.up * fuerzaSaltoReal, ForceMode.Impulse);
        MaquinaDeEstados.miEstado = MaquinaDeEstados.Estados.air;
    }

    private void FixedUpdate()
    {
        Vector3 vectorMovVerdadero = transform.TransformDirection(new Vector3(vectorMov.x, 0, vectorMov.y).normalized);     // Con TransformDirection pasamos de global a local

        if (vectorMovVerdadero != Vector3.zero)         // Esto chequea si estamos llamando a OnMovePerformed
        {
            if (MaquinaDeEstados.miEstado != MaquinaDeEstados.Estados.air)
            {
                rb.AddForce(vectorMovVerdadero * fuerzaMov * 2, ForceMode.Acceleration);        // Vamos más rápido en el suelo
            } else
            {
                rb.AddForce(vectorMovVerdadero * fuerzaMov * 1.75f, ForceMode.Acceleration);
            }
        }
        rb.linearVelocity -= new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z) / 10;         // Esto hace que perdamos inercia en las direcciones que no estemos pulsando

        Mathf.Clamp(rb.linearVelocity.x, -velocidadMaxima, velocidadMaxima);
        Mathf.Clamp(rb.linearVelocity.z, -velocidadMaxima, velocidadMaxima);


        print(new Vector2(rb.linearVelocity.x, rb.linearVelocity.z).magnitude);


        // SALTO
        if (controls.Player.Jump.IsPressed() && (MaquinaDeEstados.miEstado == MaquinaDeEstados.Estados.idle || MaquinaDeEstados.miEstado == MaquinaDeEstados.Estados.run))
        {
            fuerzaSaltoReal += fuerzaSalto;
        }
        forceCurrentImage.fillAmount = fuerzaSaltoReal / fuerzaSaltoMaxima;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag is "Pared")
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;        // No funciona
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (/*other.gameObject.tag is "Suelo" ||*/ other.contactCount == 0)
        {
            MaquinaDeEstados.miEstado = MaquinaDeEstados.Estados.air;
        }
        fuerzaSaltoReal = 0;
    }
}