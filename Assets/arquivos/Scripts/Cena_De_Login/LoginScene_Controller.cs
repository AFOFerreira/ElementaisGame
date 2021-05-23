using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LoginScene_Controller : MonoBehaviour
{
    public GameObject LoginComponnent;

    public void ShowLogin() 
    {
        Instantiate(LoginComponnent, Canvas.FindObjectOfType<Canvas>().transform);
    }
}
