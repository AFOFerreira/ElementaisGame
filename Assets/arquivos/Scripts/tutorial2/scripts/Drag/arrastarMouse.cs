using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class arrastarMouse : MonoBehaviour,IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public GameObject charA, maoMia,coliderAve, pontoAtaque,ferbloomPrincipal, ferbloomCopia, pontoDefesa, instantiateAtaque, instantiateDefesa;
    public TextMeshProUGUI vidaInimigo;
    private RectTransform rectTransform;
    public Transform furao;
    public static bool drag, drop;

    private Canvas canvas;
    
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
            
            maoMia.SetActive(false);
            
        }
  
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null){
            
            Debug.Log("drop");
            furao.transform.localPosition = new Vector2(-197,-90);

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

    public void Awake()
    {
          
        drag = false;
        rectTransform = GetComponent<RectTransform>();

        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ave")
        {
            ferbloomCopia.SetActive(true);
            Destroy(ferbloomPrincipal);
            vidaInimigo.text = "20";
            Destroy(coliderAve);
            Instantiate(pontoAtaque, instantiateAtaque.transform);
            Instantiate(pontoDefesa, instantiateDefesa.transform);

            if (charA.activeInHierarchy == false)
            {
                charA.SetActive(true);
            }
        }
    }



}
