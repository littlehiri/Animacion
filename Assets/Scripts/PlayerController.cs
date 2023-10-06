using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class that manage the player controller
/// </summary>
[AddComponentMenu("Scripts/Player/PlayerController")]

public class PlayerController : MonoBehaviour
{
    #region VARIABLES AND REFERENCES
    //variable para el movimiento que al ser privada le ponemos
    Vector2 _moveDirection;
    public float maxForwardSpeed = 8;
    public float moveSpeed = 2;
    public float turnAroundSpeed = 100f;
    float _desiredForwardSpeed;
    float _forwardSpeed;
    //Aceleracion y desaceleracion del jugador
    const float _groundAccel = 5;
    const float _groundDecel = 25;
    //Variable para la direccion de salto
    float _jumpDirection;

    Animator _anim;
    #endregion

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Hacemos que se mueva continuamente dependiendo de las teclas
        Move(_moveDirection);
    }


    #endregion

    #region Own Methods
    //Creamos un metodo que recibe los inputs del InputsActions Move
    public void OnMove(InputAction.CallbackContext context)
    {
        //Guardamos en _moveDirection lo recibido que en este caso
        _moveDirection = context.ReadValue<Vector2>();
        Debug.Log(_moveDirection);
    }
    
    //Parametro que devuelve verdadero o falso
    bool IsMoveInput
    {
        //Mira lo que hay dentro del get, si se cumple la booleana es verdadero, sino es falsa
        //Si el valor de _MoveDirection no es aproximadamente 0
        get { return !Mathf.Approximately(_moveDirection.sqrMagnitude, 0f); }
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        //Guardamos en _jumpDirection lo recibido que en este caso será un float
        _jumpDirection = context.ReadValue<float>();
        Debug.Log(_jumpDirection);
    }

    void Move(Vector2 direction)
    {
        //Realizamos un movimiento de translacion
        //transform.Translate(_moveDirection.x * moveSpeed * Time.deltaTime, 0, _moveDirection.y);
        //Parametro 
        float fDirection = direction.y;
        float turnAmount = direction.x;
        //Si la raiz cuadrada de la dirección es mayor que 1, ponla a 1 para que no se mueva más rapido en diagonal 
        if (direction.sqrMagnitude > 1f)
            direction.Normalize();
        //velocidad a la que queremos ir, será igual a la direccion multiplicada a la direccion maxima
        _desiredForwardSpeed = direction.magnitude * maxForwardSpeed *Mathf.Sign(fDirection);
        //Operador ternario equivalente a un if/else que actua sobre la misma variable
        float acceleration = IsMoveInput ? _groundAccel : _groundDecel;

        //velocidad de avance será igual a desde la propia velocidad de vance hasta la velocidad deseada con una aceleracion
        _forwardSpeed = Mathf.MoveTowards(_forwardSpeed, _desiredForwardSpeed, acceleration * Time.deltaTime);
        //Le mandamos la velocidad de avance al animator   
        _anim.SetFloat("ForwardSpeed", _forwardSpeed);
        //Se gira dependiendo de si pulsamos izquierda o derecha a una velocidad por segundo
        transform.Rotate(0f, turnAmount * Time.deltaTime, 0f);
    }

    void Jump(float direction)
    {
        Debug.Log(direction);
    }
    #endregion
}
