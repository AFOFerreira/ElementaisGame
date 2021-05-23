using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ad_Controller : MonoBehaviour
{
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        //Banner();
    }
    public void Video(){
        Main.Instance.ad.display_Video();
    }
    public void Banner(){
        Main.Instance.ad.RequestBanner();
    }

    public void Interstitial(){
        Main.Instance.ad.bannerPopUp();
    }
}
