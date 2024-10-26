using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchController : MonoBehaviour
{
    public Camera playerCamera; // Камера от первого лица
    public float pickupRange = 3f; // Дальность, на которой можно подбирать объект
    public float holdDistance = 2f; // Дистанция, на которой объект будет удерживаться
    private GameObject heldObject; // Ссылка на текущий удерживаемый объект
    private Rigidbody heldObjectRb; // Rigidbody удерживаемого объекта

    //Отслеживание обьекта
    private GameObject lastLook = null;

    private void LookObject()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            GameObject currentLook = hit.collider.gameObject;

            // Если объект под курсором изменился или его Outline неактивен
            if (currentLook != lastLook || (currentLook.GetComponent<Outline>() != null && !currentLook.GetComponent<Outline>().enabled))
            {
                // Отключаем Outline у предыдущего объекта
                if (lastLook != null && lastLook.GetComponent<Outline>() != null)
                {
                    lastLook.GetComponent<Outline>().enabled = false;
                }

                // Включаем Outline у нового объекта
                if (currentLook.GetComponent<Outline>() != null)
                {
                    currentLook.GetComponent<Outline>().enabled = true;
                    lastLook = currentLook;
                }
            }
        }
        else
        {
            // Если луч не попал в объект, отключаем Outline у последнего объекта
            if (lastLook != null && lastLook.GetComponent<Outline>() != null)
            {
                lastLook.GetComponent<Outline>().enabled = false;
                lastLook = null;
            }
        }
    }

    // Настраиваемые параметры
    public float smoothSpeed = 5f;              // Скорость сглаживания движения объекта
    public float maxMoveSpeed = 15f;            // Максимальная скорость перемещения объекта
    public float dropForce = 2f;                // Сила, с которой объект сбрасывается

    void Update()
    {
        LookObject();

        if (Input.GetKeyDown(KeyCode.Mouse0)) // Кнопка для захвата или сброса объекта
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

    // Попытка подобрать объект
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

    // Захват объекта
    private void Pickup(GameObject pickObj)
    {
        heldObject = pickObj;
        heldObjectRb = heldObject.GetComponent<Rigidbody>();
        heldObjectRb.useGravity = false;
        heldObjectRb.constraints = RigidbodyConstraints.FreezeRotation;
        heldObject.gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    // Плавное перемещение объекта перед игроком
    private void MoveObject()
    {
        Vector3 targetPosition = playerCamera.transform.position + playerCamera.transform.forward * holdDistance;
        Vector3 smoothPosition = Vector3.Lerp(heldObject.transform.position, targetPosition, Time.deltaTime * smoothSpeed);
        Vector3 direction = smoothPosition - heldObject.transform.position;

        // Ограничение скорости, чтобы не было резких движений
        heldObjectRb.velocity = Vector3.ClampMagnitude(direction * maxMoveSpeed, maxMoveSpeed);
    }

    // Сброс объекта
    private void DropObject()
    {
        heldObjectRb.useGravity = true;
        heldObjectRb.constraints = RigidbodyConstraints.None;
        heldObject.gameObject.GetComponent<BoxCollider>().enabled = true;
        // Применяем силу при сбросе
        heldObjectRb.AddForce(playerCamera.transform.forward * dropForce, ForceMode.Impulse);

        heldObject = null;
        heldObjectRb = null;
    }
}