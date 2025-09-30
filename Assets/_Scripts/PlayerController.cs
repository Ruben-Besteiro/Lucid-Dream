using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public _PLAYERACTIONS controls;
    private Vector2 vectorMov;
    private Rigidbody rb;

    [SerializeField] private float fuerzaMov = 10f;
    //[SerializeField] private const float velocidadMaxima = 1000f;         No usamos velocidad máxima, sino un vector de aceleración y uno de fricción
    private float fuerzaSalto;
    [SerializeField] private float fuerzaSaltoReal = 0;
    [SerializeField] private const float fuerzaSaltoMaxima = 2500;

    [SerializeField] private float fuerzaDash = 3000f;
    [SerializeField] private float dashCooldown = 2f; 
    private float lastDashTime = -Mathf.Infinity;

    [SerializeField] Image forceCurrentImage;

    private void Awake()
    {
        controls = new _PLAYERACTIONS();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        forceCurrentImage.fillAmount = 0;
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Move.performed += OnMovePerformed;
        controls.Player.Move.canceled += OnMoveCanceled;
        controls.Player.Jump.performed += OnJumpPerformed;
        controls.Player.Jump.canceled += OnJumpCanceled;
        controls.Player.Dash.performed += OnDashPerformed;
    }

    private void OnDisable()
    {
        controls.Player.Move.performed -= OnMovePerformed;
        controls.Player.Move.canceled -= OnMoveCanceled;
        controls.Player.Jump.performed -= OnJumpPerformed;
        controls.Player.Jump.canceled -= OnJumpCanceled;
        controls.Player.Dash.performed -= OnDashPerformed;
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
        fuerzaSaltoReal = 0;
    }

    private void OnDashPerformed(InputAction.CallbackContext ctx)
    {
        Dash();
    }

    private void FixedUpdate()
    {
        Movimiento();
        Salto();
        Dash();
    }




    void Movimiento()
    {
        Vector3 vectorMovVerdadero = transform.TransformDirection(new Vector3(vectorMov.x, 0, vectorMov.y).normalized);     // Con TransformDirection pasamos de global a local
        //Vector3 vectorMovVerdadero = transform.TransformDirection(vectorMov.normalized);

        if (vectorMovVerdadero != Vector3.zero)         // Esto chequea si estamos llamando a OnMovePerformed
        {
            if (MaquinaDeEstados.miEstado != MaquinaDeEstados.Estados.air)
            {
                rb.AddForce(vectorMovVerdadero * fuerzaMov * 4, ForceMode.Acceleration);        // Vamos más rápido en el suelo que en el aire
            }
            else
            {
                rb.AddForce(vectorMovVerdadero * fuerzaMov * 3.33f, ForceMode.Acceleration);
            }
        }
        rb.linearVelocity -= new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z) / 10;         // Esto simula la fricción (para mí es más intuitivo que usar un Physics Material)

        //print(new Vector2(rb.linearVelocity.x, rb.linearVelocity.z).magnitude);
    }

    void Salto()
    {
        if (controls.Player.Jump.IsPressed() && (MaquinaDeEstados.miEstado == MaquinaDeEstados.Estados.idle || MaquinaDeEstados.miEstado == MaquinaDeEstados.Estados.run))
        {
            fuerzaSaltoReal += fuerzaSalto;
        }
        forceCurrentImage.fillAmount = fuerzaSaltoReal / fuerzaSaltoMaxima;
    }

    void Dash()
    {
        if (controls.Player.Dash.triggered && Time.time >= lastDashTime + dashCooldown)
        {
            Vector3 dashDirection = transform.forward;

            rb.AddForce(dashDirection * fuerzaDash, ForceMode.Impulse);

            lastDashTime = Time.time;

            Debug.Log("Dash");
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        foreach (ContactPoint contact in other.contacts)
        {
            Vector3 normal = contact.normal;

            // Si alguna de sus colisiones es una pared 
            if ((Mathf.Abs(normal.x) > 0.75f || Mathf.Abs(normal.z) > 0.75f) && (new Vector2(rb.linearVelocity.x, rb.linearVelocity.z).magnitude) > 10)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation;       // No va xd
            }
        }
    }
}