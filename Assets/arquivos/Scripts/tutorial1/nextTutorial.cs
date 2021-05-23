using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class nextTutorial : MonoBehaviour
{
    Button btn;

    // Start is called before the first frame update
    void Start() 
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(nextLevel);
    }

    private void nextLevel()
    {
        //Main.Instance.ad.DestroyBanner();
        Main.Instance.Tutorial1Menu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
