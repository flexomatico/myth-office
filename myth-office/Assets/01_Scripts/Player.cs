using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    /// <summary>
    /// based on    https://docs.unity3d.com/ScriptReference/CharacterController.Move.html
    ///             https://micha-l-davis.medium.com/isometric-player-movement-in-unity-998d86193b8a
    /// </summary>
    
    private Vector3 _direction;
    private CharacterController _controller;
    private Vector3 _playerVelocity;
    private bool _groundedPlayer;
    private float _playerSpeed = 2.0f;
    private float _gravityValue = -9.81f;

    private void Start()
    {
        _controller = gameObject.AddComponent<CharacterController>();
    }

    void Update()
    {
        _groundedPlayer = _controller.isGrounded;
        if (_groundedPlayer && _playerVelocity.y < 0)
        {
            _playerVelocity.y = 0f;
        }
        
        _controller.Move(_direction * Time.deltaTime * _playerSpeed);

        if (_direction != Vector3.zero)
        {
            //gameObject.transform.forward = _direction;
        }

        _playerVelocity.y += _gravityValue * Time.deltaTime;
        _controller.Move(_playerVelocity * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 readVector = context.ReadValue<Vector2>();
        Vector3 swizzledVec = new Vector3(readVector.x, 0, readVector.y);
        _direction = IsoVectorConvert(swizzledVec);
    }

    private Vector3 IsoVectorConvert(Vector3 vector)
    {
        Quaternion rotation = Quaternion.Euler(0, 45.0f, 0);
        Matrix4x4 rotationMat = Matrix4x4.Rotate(rotation);
        return rotationMat.MultiplyPoint3x4(vector);
    }
}
