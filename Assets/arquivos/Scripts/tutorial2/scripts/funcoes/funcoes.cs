using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class funcoes : MonoBehaviour
{
   public void pularTutorial2()
    {
        Main.Instance.MainMenu();
    }

    public void pularTutorial1()
    {
        Main.Instance.Tutorial2Menu();
    }

    public void MenuPrincipalMult() 
    {
        Main.Instance.ad.DestroyBanner();
        Main.Instance.MainMenu();
    }


    private void Start()
    {
        Main.Instance.audioBase.playMusicaTutorial();
    }
}
