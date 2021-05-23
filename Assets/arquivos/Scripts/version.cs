using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class version : MonoBehaviour
{
    TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = "Versão: "+ Application.version;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
