using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnFimTutorial : MonoBehaviour
{

    public SomTutorial somTutorial;
    public void voltaMenu()
    {
        somTutorial.pararMusicaTutorial();
        SceneManager.LoadScene(1);
    }
}
