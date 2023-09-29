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
    #endregion

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        float speed = 2;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(x, 0, z);
        transform.Translate(movement * speed * Time.deltaTime);
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
    }

    void Move(Vector2 direction)
    {
        //Realizamos un movimiento de translacion
        //transform.Translate(x, 0, z);
    }
    #endregion
}
