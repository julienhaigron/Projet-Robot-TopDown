using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Rigidbody _rb;

    [Header("Parameters")]
    public float _cameraSpeed;
    private Vector2 _currentInput;
    private bool _isMoving;

    private void Start()
    {
        _currentInput = new Vector2(0, 0);
        _isMoving = false;
    }

    private void Update()
    {
        //move
        Move();
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isMoving = true;
        }
        _currentInput = context.ReadValue<Vector2>();

        if (context.canceled)
        {
            _rb.velocity = Vector3.zero;
        }
    }

    private void Move()
    {
        //TODO : clean that up

        //_rb.velocity = new Vector3(_rb.velocity.x + (_currentInput.x * _cameraSpeed * Time.deltaTime), _rb.velocity.y, _rb.velocity.z - (_currentInput.y * _cameraSpeed * Time.deltaTime));
        _rb.velocity += transform.forward * _currentInput.x * _cameraSpeed * Time.deltaTime;
        _rb.velocity += transform.right * - _currentInput.y * _cameraSpeed * Time.deltaTime;
    }


}
