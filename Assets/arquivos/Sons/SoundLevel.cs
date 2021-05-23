using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLevel : MonoBehaviour
{
  
    // Start is called before the first frame update
    void Start()
    {

       Main.Instance.audioBase.playMusicaLobby();
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
