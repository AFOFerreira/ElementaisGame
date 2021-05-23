using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class dialogueManagerBTutorial2 : MonoBehaviour
{

    public TextMeshProUGUI dialogueText, cristalTerra;

    private Queue<string> sentences;
    public dialogueBTutorial2 dialogueB2; 
    public GameObject charB, charA, informacoesPlayer, coliderCachorro,arcoAtivo2, maoCachorro, maoCristal2;

    bool ativarCoelho;

    // Use this for initialization
    void Start()
    {

        ativarCoelho = false;

        sentences = new Queue<string>();

        TriggerDialogue();
       

    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && charB.activeInHierarchy == true)
        {
            DisplayNextSentence();
        }

        if(maoCristal2.activeInHierarchy){
            ativarCoelho = true;
        }

    }

 
    public void StartDialogue(dialogueBTutorial2 dialogue)//metodo para começar o dialogo
    {


        sentences.Clear();//limpar sentenças

        foreach (string sentence in dialogue.sentences)//procurar por frases (sentenças) na classe dialogo
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();//passar de sentenças (dialogos)


    }

    public void DisplayNextSentence()//metodo para a passsage de dialogos
    {

        if (sentences.Count == 1)//se a contagem do dialogo for igual a zero, vai para o metodo do fim do dialogo
        {

            charB.SetActive(false);
            charA.SetActive(true);
          
        }

        if (sentences.Count == 0)//se a contagem do dialogo for igual a zero, vai para o metodo do fim do dialogo
        {

            charB.SetActive(false);
            charA.SetActive(true);
            //informacoesPlayer.SetActive(true);
            //StartCoroutine(ativarMaoCristal(1.5f));
        }


        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;


    }

    public void TriggerDialogue()
    {
        FindObjectOfType<dialogueManagerBTutorial2>().StartDialogue(dialogueB2);

    }

    IEnumerator ativarCristal(float tempo)
    {
        yield return new WaitForSeconds(tempo);
        maoCachorro.SetActive(true);
        //informacoesPlayer.SetActive(false);
        Destroy(maoCristal2);
        arrastarAtaqueCachorro.drag = true;
    }

    IEnumerator ativarMaoCristal(float tempo)
    {
        yield return new WaitForSeconds(tempo);
        maoCristal2.SetActive(true);
    }

    public void cristalAtivar2()
    {
        
        if(ativarCoelho == true){
        StartCoroutine(ativarCristal(1f));
        cristalTerra.text = "0";
        arcoAtivo2.SetActive(true);
        coliderCachorro.SetActive(true);
        

        }
     
    }
  
}
