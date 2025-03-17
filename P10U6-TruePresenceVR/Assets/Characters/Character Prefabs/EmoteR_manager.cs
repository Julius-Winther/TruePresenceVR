using UnityEngine;
using CrazyMinnow.SALSA;

public class EmoteR_manager : MonoBehaviour
{
    public Emoter Normal;
    public Emoter Angry;
    public Salsa SALSA;
    public Eyes eyes;

    public bool isAngry;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isAngry == true)
        {
            SALSA.emoter = Angry;
            //Normal.enabled = false;
            //Angry.enabled = true;
            
           
            
        } else
        {
            SALSA.emoter = Normal;
            //Normal.enabled = true;
            //Angry.enabled = false;
        }
    }
}
