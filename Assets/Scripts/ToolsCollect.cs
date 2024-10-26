using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsCollect : MonoBehaviour
{
    [SerializeField] GameObject questDone_1;
    [SerializeField] GameObject questDone_2;
    [SerializeField] GameObject questDone_3;
    [SerializeField] GameObject questDone;
    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.name) {
            case "tool_gas":
                questDone_1.SetActive(true);
                break;
            case "tool_box":
                questDone_2.SetActive(true);
                break;
            case "tool_bag":
                questDone_3.SetActive(true);
                break;
        }
    }

}
