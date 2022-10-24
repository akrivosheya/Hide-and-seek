using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 6.0f;
    public bool IsMoving { get; set; }

    private CharacterController _charController;
    private float _speedBoost = 2f;

    void Start()
    {
        IsMoving = true;
        _charController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if(IsMoving)
        {
            float fixedSpeed = (Input.GetKey(KeyCode.LeftShift)) ? speed * _speedBoost : speed;
            float deltaX = Input.GetAxis("Horizontal") * fixedSpeed;
            float deltaY = Input.GetAxis("Vertical") * fixedSpeed;
            Vector3 movement = new Vector3(deltaX, deltaY, 0);
            movement = Vector3.ClampMagnitude(movement, fixedSpeed) * Time.deltaTime;
            movement = transform.TransformDirection(movement);
            _charController.Move(movement);//Problems with toaching other objets
        }
    }
}
