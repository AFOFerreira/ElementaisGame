using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class facebookLogin : MonoBehaviour
{
    Button btn;
    FacebookSetings facebook;



    // Start is called before the first frame update
    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(clicado);
    }

    void clicado()
    {
        Main.Instance.facebook.FacebookLogin();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
