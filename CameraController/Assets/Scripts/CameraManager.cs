using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] GameObject[] cam = new GameObject[3];
    private void Update()
    {
        if (Input.GetButtonDown("Cam1"))
        {
            cam[0].SetActive(true);
            cam[1].SetActive(false);
            cam[2].SetActive(false);
        }
        else if (Input.GetButtonDown("Cam2"))
        {
            cam[0].SetActive(false);
            cam[1].SetActive(true);
            cam[2].SetActive(false);
        }
        else if (Input.GetButtonDown("Cam3"))
        {
            cam[0].SetActive(false);
            cam[1].SetActive(false);
            cam[2].SetActive(true);
        }
    }
}
