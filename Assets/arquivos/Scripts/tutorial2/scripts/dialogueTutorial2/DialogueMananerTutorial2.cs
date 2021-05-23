using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueMananerTutorial2 : MonoBehaviour
{

    public TextMeshProUGUI dialogueText, cristalFogoTXT;

    private Queue<string> sentences;

    public DialogueTutorial2 dialogue2;
    public GameObject turnoInimigo,maoAuxiliar,auxiliarVirada, turnoAliado,furao, tartaruga, gorila, telaParabens, coliderAve,coliderEva, coliderCachorro;

    public Button inimigoButton, auxiliarCemiterio, auxiliarEspelho,aliadoButton, arcoBtnFurao, arcoBtnCoelho, arcoBtnGorila;
    public GameObject charA, bandeiraGorila, bandeiraCoelho, bandeiraFurao, maoDeckMia,maoCachorro,furaoAnimIncial, tartarugaAnimIncial, gorilaAnimIncial, informacoesPlayer, cristalVermelho,maoCristal, cristalVerde, maoAtacarEiva, setaVermelha, ferbloomAtaque, turnMao,maoAuxiliar1, maoAuxiliar2, ave, charB, pontoAtaque, pontoDefesa;
    public Animator balao, balaoB, cartaAuxiliar;



    bool ativarFurao;
    
    IEnumerator AnimInicial(float tempo)
    {
       
        yield return new WaitForSeconds(tempo);
        Destroy(furaoAnimIncial);
        Destroy(tartarugaAnimIncial);
        gorilaAnimIncial.SetActive(false);
        furao.SetActive(true);
        tartaruga.SetActive(true);
        gorila.SetActive(true);

    }
    // Use this for initialization
    void Start()
    {
        ativarFurao = false;
       
        cartaAuxiliar = GetComponent<Animator>();

        StartCoroutine(AnimInicial(2.5f));
        sentences = new Queue<string>();

        TriggerDialogue();

        aliadoButton.interactable = false;
        StartCoroutine(aparecerCharA(3));
        balao.GetComponent<Animator>();
        balaoB.GetComponent<Animator>();
        furao.GetComponent<Animator>();
    }

    
    public void StartDialogue(DialogueTutorial2 dialogue)//metodo para começar o dialogo
    {

        sentences.Clear();//limpar sentenças

        foreach (string sentence in dialogue.sentences)//procurar por frases (sentenças) na classe dialogo
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();//passar de sentenças (dialogos)

    }
   
    IEnumerator aparecerCharA(int tempo)
    {
        yield return new WaitForSeconds(tempo);
        charA.SetActive(true);
       
        balao.SetBool("balaoTroca", true);
    }

    IEnumerator ferbloomAtaqueAnim(float tempo)
    {
        ferbloomAtaque.SetActive(true);
        yield return new WaitForSeconds(tempo);
        Destroy(ferbloomAtaque);
        Destroy(ave);
        yield return new WaitForSeconds(tempo);
        charA.SetActive(true);
    }

    IEnumerator painelParabens(float tempo)
    {

        yield return new WaitForSeconds(tempo);
        telaParabens.SetActive(true);

    }

  

    private void Update()
    {
        if(arrastarAtacarEiva.parabens == true)
        {
            StartCoroutine(painelParabens(3.5f));
        }

        if (Input.GetMouseButtonDown(0) && charA.activeInHierarchy == true)
        {
            DisplayNextSentence();
        }

 
        if(charA.activeInHierarchy == false && charB.activeInHierarchy == false){
            arcoBtnCoelho.interactable = true;
            arcoBtnGorila.interactable = true;
            arcoBtnFurao.interactable = true;
        }

        else if(charA.activeInHierarchy == true || charB.activeInHierarchy == true){
              
            arcoBtnCoelho.interactable = false;
            arcoBtnGorila.interactable = false;
            arcoBtnFurao.interactable = false;
        
        }

      
       
    }

    public void ativarCristal()
    {
        cristalVerde.SetActive(true);
        ativarFurao = true;

        if(ativarFurao == true){

        cristalFogoTXT.text = "2";
        Destroy(cristalVermelho);
        Destroy(maoCristal);   
        arrastarMouse.drag = true;
        maoDeckMia.SetActive(true);

        }
       
    }

    IEnumerator maoAtivarCristal(int tempo)
    {

        yield return new WaitForSeconds(tempo);
        maoCristal.SetActive(true);
    
    }
        


    public void DisplayNextSentence()//metodo para a passsage de dialogos
    {
        

        if (sentences.Count == 12)
        {

            charA.SetActive(false);
            //informacoesPlayer.SetActive(true);
            StartCoroutine(maoAtivarCristal(1));
            coliderAve.SetActive(true);
        }

        if (sentences.Count == 9)
        {

            charA.SetActive(false);
            Destroy(pontoAtaque);
            Destroy(pontoDefesa);
            StartCoroutine(ferbloomAtaqueAnim(2.5f));
        }

        if (sentences.Count == 8)
        {
            charA.SetActive(false);
            turnMao.SetActive(true);
            aliadoButton.interactable = true;
        }

          if (sentences.Count == 6)
        {
            charA.SetActive(false);
            maoAuxiliar.SetActive(true);
            arrastarAuxiliar.drag = true;
            auxiliarEspelho.interactable = true;
        
        }

        if (sentences.Count == 5)
        {
            charA.SetActive(false);
            maoAuxiliar2.SetActive(true);
            arrastarAuxiliarCemiterio.drag = true;
            auxiliarCemiterio.interactable = true;
        
        }

        if (sentences.Count == 4)
        {
            charA.SetActive(false);
            charB.SetActive(true);
            
        }

        if (sentences.Count == 3)
        {
            charA.SetActive(false);
            charB.SetActive(true);
            
        }

        if (sentences.Count == 0)
        {
            Destroy(charA);
            maoAtacarEiva.SetActive(true);
            coliderEva.SetActive(true);
            arrastarAtacarEiva.drag = true;

        }

        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;

    }

    public void botaoTurno()
    {
        if(turnMao.activeInHierarchy == true)
        {
            turnMao.SetActive(false);
            turnoInimigo.SetActive(true);
            inimigoButton.interactable = false;
            turnoAliado.SetActive(false);
            charA.SetActive(true);
            setaVermelha.SetActive(true);
        }
    }

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueMananerTutorial2>().StartDialogue(dialogue2);

    }

    public void BandeiraGorila(){

     if(bandeiraGorila.activeInHierarchy == true){

             bandeiraGorila.SetActive(false);
        }

        else if (bandeiraGorila.activeInHierarchy == false)
        {
            bandeiraGorila.SetActive(true);

        }

    }

     public void BandeiraFurao(){

    if(charA.activeInHierarchy == false || charB.activeInHierarchy == false){
        
        if(bandeiraFurao.activeInHierarchy == true){

             bandeiraFurao.SetActive(false);
        }

        else if (bandeiraFurao.activeInHierarchy == false)
        {
            bandeiraFurao.SetActive(true);

        }

 }
    }

     public void BandeiraCoelho(){

         if(charA.activeInHierarchy == false || charB.activeInHierarchy == false){

             if(bandeiraCoelho.activeInHierarchy == true){

             bandeiraCoelho.SetActive(false);
        }

        else if (bandeiraCoelho.activeInHierarchy == false)
        {
            bandeiraCoelho.SetActive(true);

        }

         }
        
    }
}
