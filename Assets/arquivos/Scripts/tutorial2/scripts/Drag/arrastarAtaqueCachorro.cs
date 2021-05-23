using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
public class arrastarAtaqueCachorro : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public GameObject maoCachorro, miaPrincipal, miaCopia,coliderCachorro, charA;
    private RectTransform rectTransform;
    public static bool drag;

     public Transform coelho;

    public void OnDrag(PointerEventData eventData)
    {
        if(drag == true)
        {
            rectTransform.anchoredPosition += eventData.delta;
            Destroy(maoCachorro);
        }
       
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Mouse down");
    }

     public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null){
            
            Debug.Log("drop");
            coelho.transform.localPosition = new Vector2(-263f,-175f);

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
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "cachorro")
        {
            
            Destroy(miaPrincipal);
            miaCopia.SetActive(true);
            Destroy(coliderCachorro);

            
        }
    }

 

}
