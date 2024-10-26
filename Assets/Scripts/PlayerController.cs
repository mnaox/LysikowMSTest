using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float airControlFactor = 0.5f; // �������� �������� � �������
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    public float smoothSpeedTransition = 10.0f; // �������� ������������ ��� �������� ����� ������� � �����

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private float currentSpeed;
    private Vector3 velocity = Vector3.zero;
    private bool isGrounded;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // ��������, ��������� �� ����� �� �����
        isGrounded = characterController.isGrounded;

        // ������� ����� ������� � �����
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = isRunning ? runningSpeed : walkingSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, smoothSpeedTransition * Time.deltaTime);

        // �������� ������-����� � ����
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        float moveDirectionX = Input.GetAxis("Vertical") * currentSpeed;
        float moveDirectionZ = Input.GetAxis("Horizontal") * currentSpeed;

        // ���� ����� �� �����, ��������� ��������
        if (isGrounded)
        {
            moveDirection = (forward * moveDirectionX) + (right * moveDirectionZ);

            // ������
            if (Input.GetButton("Jump") && canMove)
            {
                velocity.y = jumpSpeed;
            }
            else
            {
                velocity.y = 0;
            }
        }
        else
        {
            // � ������� ��������� �������� � ���������� ��������
            moveDirection = (forward * moveDirectionX * airControlFactor) + (right * moveDirectionZ * airControlFactor);
            velocity.y -= gravity * Time.deltaTime;
        }

        // �������� ��������
        Vector3 finalMove = (moveDirection + velocity) * Time.deltaTime;
        characterController.Move(finalMove);

        // ������� ������ � ������
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}