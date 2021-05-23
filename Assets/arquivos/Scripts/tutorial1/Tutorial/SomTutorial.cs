using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SomTutorial : MonoBehaviour
{
    private AudioBase audioBaseScript;
    // Start is called before the first frame update
    void Start()
    {
        GameObject audioController = GameObject.Find("GerenciadorSom");
        audioBaseScript = audioController.GetComponent<AudioBase>();

        audioBaseScript.playMusicaTutorial();
    }

    public void pararMusicaTutorial()
    {
        audioBaseScript.stopMusicaTutorial();
    }
}
