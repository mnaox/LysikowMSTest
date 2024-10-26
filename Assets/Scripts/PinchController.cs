using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchController : MonoBehaviour
{
    public Camera playerCamera; // ������ �� ������� ����
    public float pickupRange = 3f; // ���������, �� ������� ����� ��������� ������
    public float holdDistance = 2f; // ���������, �� ������� ������ ����� ������������
    private GameObject heldObject; // ������ �� ������� ������������ ������
    private Rigidbody heldObjectRb; // Rigidbody ������������� �������

    //������������ �������
    private GameObject lastLook = null;

    private void LookObject()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            GameObject currentLook = hit.collider.gameObject;

            // ���� ������ ��� �������� ��������� ��� ��� Outline ���������
            if (currentLook != lastLook || (currentLook.GetComponent<Outline>() != null && !currentLook.GetComponent<Outline>().enabled))
            {
                // ��������� Outline � ����������� �������
                if (lastLook != null && lastLook.GetComponent<Outline>() != null)
                {
                    lastLook.GetComponent<Outline>().enabled = false;
                }

                // �������� Outline � ������ �������
                if (currentLook.GetComponent<Outline>() != null)
                {
                    currentLook.GetComponent<Outline>().enabled = true;
                    lastLook = currentLook;
                }
            }
        }
        else
        {
            // ���� ��� �� ����� � ������, ��������� Outline � ���������� �������
            if (lastLook != null && lastLook.GetComponent<Outline>() != null)
            {
                lastLook.GetComponent<Outline>().enabled = false;
                lastLook = null;
            }
        }
    }

    // ������������� ���������
    public float smoothSpeed = 5f;              // �������� ����������� �������� �������
    public float maxMoveSpeed = 15f;            // ������������ �������� ����������� �������
    public float dropForce = 2f;                // ����, � ������� ������ ������������

    void Update()
    {
        LookObject();

        if (Input.GetKeyDown(KeyCode.Mouse0)) // ������ ��� ������� ��� ������ �������
        {
            if (heldObject == null)
            {
                TryPickupObject();
            }
            else
            {
                DropObject();
            }
        }

        if (heldObject != null)
        {
            MoveObject();
        }
    }

    // ������� ��������� ������
    private void TryPickupObject()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            if (hit.collider.gameObject.GetComponent<Rigidbody>() != null)
            {
                Pickup(hit.collider.gameObject);
            }
        }
    }

    // ������ �������
    private void Pickup(GameObject pickObj)
    {
        heldObject = pickObj;
        heldObjectRb = heldObject.GetComponent<Rigidbody>();
        heldObjectRb.useGravity = false;
        heldObjectRb.constraints = RigidbodyConstraints.FreezeRotation;
        heldObject.gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    // ������� ����������� ������� ����� �������
    private void MoveObject()
    {
        Vector3 targetPosition = playerCamera.transform.position + playerCamera.transform.forward * holdDistance;
        Vector3 smoothPosition = Vector3.Lerp(heldObject.transform.position, targetPosition, Time.deltaTime * smoothSpeed);
        Vector3 direction = smoothPosition - heldObject.transform.position;

        // ����������� ��������, ����� �� ���� ������ ��������
        heldObjectRb.velocity = Vector3.ClampMagnitude(direction * maxMoveSpeed, maxMoveSpeed);
    }

    // ����� �������
    private void DropObject()
    {
        heldObjectRb.useGravity = true;
        heldObjectRb.constraints = RigidbodyConstraints.None;
        heldObject.gameObject.GetComponent<BoxCollider>().enabled = true;
        // ��������� ���� ��� ������
        heldObjectRb.AddForce(playerCamera.transform.forward * dropForce, ForceMode.Impulse);

        heldObject = null;
        heldObjectRb = null;
    }
}