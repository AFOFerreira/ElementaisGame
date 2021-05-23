using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class arrastarAuxiliarCemiterio : MonoBehaviour, IPointerDownHandler
{
    public GameObject  auxiliarCemiterio,arcoAtivo1, gorilaInicial,maoAuxiliar2, charA, ferrockRenascer;
    public Animator auxiliar;
    public TextMeshProUGUI cristalTerra;
    public static bool drag;
    private void Start()
    {
        drag = false;
        auxiliar = GetComponent<Animator>();
    }
    IEnumerator AparecerauxiliarCemiterio(float tempo, float tempo2)
    {
        auxiliar.SetBool("cemiterio", true);
        yield return new WaitForSeconds(tempo);
        cristalTerra.text = "1";
        arcoAtivo1.SetActive(true);
        auxiliarCemiterio.SetActive(true);
        gorilaInicial.SetActive(true);
        yield return new WaitForSeconds(tempo2);
        Destroy(gorilaInicial);
        ferrockRenascer.SetActive(true);
        charA.SetActive(true);
    }

    public void Auxiliar_Cemiterio(){

        Destroy(maoAuxiliar2);
        StartCoroutine(AparecerauxiliarCemiterio(2.5f, 1f));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(drag == true)
        {
            Destroy(maoAuxiliar2);
            //StartCoroutine(AparecerauxiliarCemiterio(2.5f, 1f));
        }
       
    }
}
