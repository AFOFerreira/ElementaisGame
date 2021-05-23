using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopUp_MSG : MonoBehaviour
{
    [SerializeField]
    GameObject item;
    [SerializeField]
    GameObject container;
    [SerializeField]
    Button close;
    
    TextAsset arquivo;
    void Start()
    {
        arquivo = Resources.Load<TextAsset>("Atualizacoes");
        
        string[] linesInFile = arquivo.text.Split('\n');
        for (int i = 0; i < linesInFile.Length; i++)
        {
            item.transform.Find("TxtAtualizacao").GetComponent<TextMeshProUGUI>().text = linesInFile[i];
            Instantiate(item, container.transform); 
        }
        //foreach (string line in linesInFile) //Para cada linha....
        //{

        //    Instantiate(item, container.transform);
        //}

        close.onClick.AddListener(closeFunction);
    }

    private void closeFunction()
    {
        this.gameObject.SetActive(false);
    }

    void Update()
    {
       
    }
}
