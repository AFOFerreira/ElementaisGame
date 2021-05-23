using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class arrastarAuxiliar : MonoBehaviour, IPointerDownHandler
{
    public Animator cartaAuxiliar;
    public GameObject maoAuxiliar,charA, cachorro,ataqueGato, maoTurno,auxiliarVirada, setaVermelha, ferrock, ataqueInimigo;
    public GameObject turnoInimigo, turnoAliado, charB;
    public static bool drag;
    public Button inimigoButton, aliadoButton;
    private void Start()
    {
        cartaAuxiliar = GetComponent<Animator>();
        drag = false;
    }

    IEnumerator turno(float tempo)
    {
        yield return new WaitForSeconds(tempo);
        auxiliarVirada.SetActive(true);
        inimigoButton.interactable = true;
        ataqueGato.SetActive(true);
        yield return new WaitForSeconds(tempo);
        Destroy(ataqueGato);
        Destroy(cachorro);
        charA.SetActive(true);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if(drag == true)
        {
            Debug.Log("enter");
            //maoAuxiliar.SetActive(false);
           
            
        }
       
    }

    public void auxiliarEspelho(){
        Destroy(maoAuxiliar);
        cartaAuxiliar.SetBool("cartaAuxiliar", true);
        StartCoroutine(turno(2.5f));
    }

    public void trocarTurno()
    {
        maoTurno.SetActive(false);
        Destroy(setaVermelha);
        StartCoroutine(AtaqueInimigo(1.5f));
    }

    IEnumerator AtaqueInimigo(float tempo)
    {
        ataqueInimigo.SetActive(true);
        yield return new WaitForSeconds(tempo);
        Destroy(ataqueInimigo);
        ferrock.SetActive(false);
       // yield return new WaitForSeconds(tempo);
        //charB.SetActive(true);
       
    }

}
