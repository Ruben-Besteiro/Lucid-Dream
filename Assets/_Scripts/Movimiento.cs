using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Movimiento : MonoBehaviour
{
    private _PLAYERACTIONS controls;
    private Vector2 vectorMov;
    private Rigidbody rb;

    [SerializeField] private float fuerzaMov = 40f;
    [SerializeField] private const float velocidadMaxima = 10f;
    private float fuerzaSalto;
    [SerializeField] private float fuerzaSaltoReal = 0;
    [SerializeField] private const float fuerzaSaltoMaxima = 2000;
    [SerializeField] Image forceCurrentImage;

    private void Awake()
    {
        controls = new _PLAYERACTIONS();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        forceCurrentImage.fillAmount = 0;
        //Destroy(forceCurrentImage.gameObject);
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
        fuerzaSalto = 100;
    }

    private void OnJumpCanceled(InputAction.CallbackContext ctx)
    {
        if (fuerzaSaltoReal > fuerzaSaltoMaxima)
        {
            fuerzaSaltoReal = 2000;
        }

        rb.AddForce(Vector3.up * fuerzaSaltoReal, ForceMode.Impulse);
        print("Se ha saltado con una fuerza de " + fuerzaSaltoReal);
        fuerzaSaltoReal = 0;
        MaquinaDeEstados.miEstado = MaquinaDeEstados.Estados.air;
    }

    private void FixedUpdate()
    {
        // MOVIMIENTO
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
        Mathf.Clamp(velocidadMaxima, -Mathf.Infinity, velocidadMaxima);
        Mathf.Clamp(rb.linearVelocity.z, -velocidadMaxima, velocidadMaxima);


        // SALTO
        if (controls.Player.Jump.IsPressed() && (MaquinaDeEstados.miEstado == MaquinaDeEstados.Estados.idle || MaquinaDeEstados.miEstado == MaquinaDeEstados.Estados.move))
        {
            print(MaquinaDeEstados.miEstado);
            fuerzaSaltoReal += fuerzaSalto;
            print(fuerzaSaltoReal);
        }
        forceCurrentImage.fillAmount = fuerzaSaltoReal / fuerzaSaltoMaxima;
        print(forceCurrentImage.fillAmount);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Suelo")
        {
            print(fuerzaSaltoReal);
            Mathf.Clamp(fuerzaSaltoReal, 0, fuerzaSaltoMaxima);
            MaquinaDeEstados.miEstado = MaquinaDeEstados.Estados.idle;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Suelo")
        {
            MaquinaDeEstados.miEstado = MaquinaDeEstados.Estados.air;
        }
    }
}