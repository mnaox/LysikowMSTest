using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private GameObject collider;
    private void OnTriggerEnter(Collider other)
    {
        _anim.SetTrigger("DoorAction");
        collider.SetActive(false);
        
    }
}
