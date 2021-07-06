using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinalScreen : MonoBehaviour
{  
    //quando é iniciado o componente.
    void Start()
    {
        ChamaAd();
    }    

    public void VoltarMenu() 
    {
        Main.Instance.ad.DestroyBanner();
        Main.Instance.MainMenu();

    }

   void ChamaAd() 
    {
        Main.Instance.ad.RequestBanner();
    }
}
