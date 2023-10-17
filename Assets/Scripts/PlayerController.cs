using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

/// <summary>
/// Class that manage the player controller
/// </summary>
[AddComponentMenu("Scripts/Player/PlayerController")]

public class PlayerController : MonoBehaviour
{
    #region VARIABLES AND REFERENCES
    //Variable para el movimiento que al ser privada le ponemos barra baja
    Vector2 _moveDirection;
    public float moveSpeed = 2;
    public float maxForwardSpeed = 8;
    public float turnSpeed = 100f;
    float _desiredForwardSpeed;
    float _forwardSpeed;
    //Aceleración y desaceleración del jugador
    const float _groundAccel = 5;
    const float _groundDecel = 25;
    //Variable para la dirección de salto
    float _jumpDirection;
    //Variable para la fuerza de salto
    float _jumpSpeed = 30000f;
    //Variable para conocer la fuerza de preparación de salto
    float _jumpEffort = 0;
    //Variable para conocer cuando estoy lista para saltar o no
    bool _readyJump = false;
    //Creamos una variable para conocer cuando estamos tocando el suelo y cuando no
    bool _onGround = true;
    //Variable para conocer la distancia hasta la que va a ir el Raycast
    float _groundRayDist = 2f;

    Animator _anim;
    Rigidbody _rb;
    #endregion

    #region UNITY METHODS
    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Hacemos que se mueva continuamente dependiendo de las teclas pulsadas
        Move(_moveDirection);
        Jump(_jumpDirection);

        //Hacemos un rayo con físicas que nos sirve para discernir si estamos apuntando o bien chocando contra algo
        //Esto sería lo que colisiona al lanzar este rayo
        RaycastHit hit;
        //Creamos el rayo (de dónde sale y con qué dirección y con qué longitud, a qué dirección va)
        Ray ray = new Ray(transform.position + Vector3.up * _groundRayDist * 0.5f, Vector3.down);
        //Usando las físicas de los Raycast, partiendo del rayo dado, con el colisionador dado como salida de datos, y con la longitud del rayo
        //Detectamos si se ha chocado con algo
        if (Physics.Raycast(ray, out hit, _groundRayDist))
        {
            //Sino estoy tocando el suelo
            if (!_onGround)
            {
                //Lo estoy tocando
                _onGround = true;
                //Medimos la velocidad en ese momento
                _anim.SetFloat("LandingVelocity", _rb.velocity.magnitude);
                //Pondré el parámetro de aterrizaje en verdadero
                _anim.SetBool("Land", true);
            }
        }
        //Si el Raycast no está chocando con nada
        else
        {
            //Será que no estamos en el suelo
            _onGround = false;
        }
        //Usamos un debbuguer para ver este Raycast
        Debug.DrawRay(transform.position + Vector3.up * _groundRayDist * 0.5f, Vector3.down * _groundRayDist, Color.red);
    }

    #endregion

    #region OWN METHODS
    //Creamos un método que recibe los inputs del InputActions Move
    public void OnMove(InputAction.CallbackContext context)
    {
        //Guardamos en _moveDirection lo recibido que en este caso será un Vector2
        _moveDirection = context.ReadValue<Vector2>();
        Debug.Log(_moveDirection);
    }

    //Parámetro que devuelve verdadero o falso
    bool IsMoveInput
    {
        //Mira lo que hay dentro del get, si se cumple la booleana es verdadera, sino es falsa
        //Si el valor de _moveDirection no es aproximadamente 0, será que estamos pulsando alguna tecla o botón
        get { return !Mathf.Approximately(_moveDirection.sqrMagnitude, 0f); }
    }

    //Método que mueve al jugador una vez recibidos los inputs
    void Move(Vector2 direction)
    {
        //Realizamos un movimiento de traslación con el jugador
        //transform.Translate(_moveDirection.x * moveSpeed * Time.deltaTime, 0, _moveDirection.y * moveSpeed * Time.deltaTime);
        //Parámetro para recoger cuanto vamos a girar sabiendo si pulsamos izquierda y derecha
        float turnAmount = direction.x;
        //Recogemos la dirección en la que nos movemos adelante o atrás
        float fDirection = direction.y;
        //Si la raíz cuadrada de la dirección es mayor que 1, ponla a 1 para que no se mueva más rápido en diagonal
        if (direction.sqrMagnitude > 1f)
            direction.Normalize();
        //Velocidad a la que queremos ir, será igual a la dirección multiplicada por velocidad máxima y el signo de la dirección
        _desiredForwardSpeed = direction.magnitude * maxForwardSpeed * Mathf.Sign(fDirection);
        //Operador ternario (equivalente a un if/else que actúa sobre la misma variable)
        float acceleration = IsMoveInput ? _groundAccel : _groundDecel;
        //Equivalente al operador ternario de arriba
        ////Si estamos pulsando alguna tecla
        //if (IsMoveInput)
        //{
        //    //El jugador acelera
        //    acceleration = _groundAccel;
        //}
        //else
        //{
        //    //Sino frena
        //    acceleration = _groundDecel;
        //}

        //Velocidad de avance irá desde la propia velocidad de avance hasta la velocidad deseada con una acelaración por segundo
        _forwardSpeed = Mathf.MoveTowards(_forwardSpeed, _desiredForwardSpeed, acceleration * Time.deltaTime);
        //Le mandamos la velocidad de avance al animator
        _anim.SetFloat("ForwardSpeed", _forwardSpeed);
        //Se gira dependiendo de si pulsamos izquierda o derecha a una velocidad por segundo
        transform.Rotate(0f, turnAmount * Time.deltaTime * turnSpeed, 0f);
    }

    //Creamos un método que recibe los inputs del InputActions Jump
    public void OnJump(InputAction.CallbackContext context)
    {
        //Guardamos en _jumpDirection lo recibido que en este caso será un float
        _jumpDirection = context.ReadValue<float>();
    }

    void Jump(float direction)
    {
        //Debug.Log(direction);
        if (direction > 0 && _onGround)
        {
            _anim.SetBool("ReadyJump", true);
            //Al pulsar una tecla hacemos que se quede preparada para saltar
            _readyJump = true;
        }
        //Si está preparada para saltar
        else if (_readyJump)
        {
            //Salta
            _anim.SetBool("Launch", true);
            //Ya no estas preparada para saltar
            _readyJump = false;
            //Se lo transmitimos al Animator
            _anim.SetBool("ReadyJump", false);
        }
    }

    //Método para decirle al personaje cuando salta
    public void Launch()
    {
        //Aplicamos una fuerza de salto
        _rb.AddForce(0f, _jumpSpeed, 0f);
        //Aquí le decimos que una vez ha saltado para de hacer esa animación
        _anim.SetBool("Launch", false);
        //Quitamos el RootMotion de el personaje
        _anim.applyRootMotion = false;
    }

    public void Land()
    {
        //Cuando esto ocurra habrá aterrizado
        _anim.SetBool("Land", false);
        //Volvemos a decirle que se active el movimiento por animación
        _anim.applyRootMotion = true;
        //Quitar animación
        _anim.SetBool("Laumch", false);
    }
    #endregion

}
