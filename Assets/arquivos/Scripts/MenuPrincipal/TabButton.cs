using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TabButton : MonoBehaviour, IPointerClickHandler
{
    public TabGroup tabGroup;
    public GerenciadorColecao colecao;
    public Button btn;
    public Image corBackground;
    // Start is called before the first frame update
    void Start()
    {
        btn = GetComponent<Button>();
        corBackground = GetComponent<Image>(); 
        tabGroup.Substribe(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
        btn.interactable = false;
    }

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    tabGroup.OnTabEnter(this);
    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    tabGroup.OnTabExit(this);
    //}

}
