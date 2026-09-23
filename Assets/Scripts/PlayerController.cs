using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float acceleration = 8f;
    [SerializeField] float deceleration = 10f;
    [SerializeField] float turnSpeed = 1f;
    [SerializeField] float angleOffset = 90f;

    Camera mainCamera;
    Quaternion targetRotation;
    float currentSpeed;


    void Awake()
    {
        mainCamera = Camera.main;
        targetRotation = transform.rotation;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool mouseDown = Input.GetMouseButton(0);

        if (mouseDown)
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = -mainCamera.transform.position.z;
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
            Vector2 difference = (Vector2)(mouseWorldPos - transform.position);

            if (difference.sqrMagnitude < 0.01f) return;
            float targetAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg + angleOffset;
            targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

        float targetSpeed = mouseDown ? maxSpeed : 0f;
        float rate = mouseDown ? acceleration : deceleration;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, rate * Time.deltaTime);

        if (currentSpeed > 0.001f)
        {
            transform.position += transform.up * (currentSpeed * Time.deltaTime);
        }
    }
}
