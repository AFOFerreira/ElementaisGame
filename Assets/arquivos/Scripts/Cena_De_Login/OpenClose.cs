using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OpenClose : MonoBehaviour
{
    public TextMeshProUGUI versionProject;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        //StartCoroutine(Main.Instance.web.GetVersionProject(versionProject));
    }

    public void Abrir(GameObject go) 
    {
        go.SetActive(true);
    }
    public void Fechar(GameObject go) 
    {
        go.SetActive(false);
    }
}
