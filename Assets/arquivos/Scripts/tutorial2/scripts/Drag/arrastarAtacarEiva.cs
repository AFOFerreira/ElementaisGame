using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class arrastarAtacarEiva : MonoBehaviour,IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public GameObject animAtacarEiva,furao,ferrockCopia,ferrockPrincipal, maoAtacarEiva;
    public TextMeshProUGUI vidaEnemy;
    private RectTransform rectTransform;
    public Transform gorila;
    public static bool drag, parabens;

    private Canvas canvas;

    IEnumerator atacarEiva(float tempo)
    {
        animAtacarEiva.SetActive(true);
        vidaEnemy.text = "0";
        yield return new WaitForSeconds(tempo);
        
        Destroy(animAtacarEiva);
        
    }

    public void Awake()
    {
        drag = false;
        parabens = false;
        rectTransform = GetComponent<RectTransform>();

         canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Mouse down");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(drag == true)
       {
            //rectTransform.anchoredPosition += eventData.delta;
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
            Destroy(maoAtacarEiva);
        }
       
    }
 public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null){
            
            Debug.Log("drop");
            gorila.transform.localPosition = new Vector2(-54f,0f);

        }          
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("begin");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End");
    }


    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Eiva")
        {
            parabens = true;
            Destroy(ferrockPrincipal);
            ferrockCopia.SetActive(true);
            Debug.Log("Eiva");
            StartCoroutine(atacarEiva(1.5f));
         
           
        }
        
    }
}
