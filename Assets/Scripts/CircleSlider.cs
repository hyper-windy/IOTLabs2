using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleSlider : MonoBehaviour
{
    
    // Start is called before the first frame update
    public bool b=true;
    public Image img;
    public float speed=0.5f;

    float time=0f;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(b){
            time+=Time.deltaTime*speed;
            img.fillAmount=time;
            if (time>1){
                time=0;
            }
        }
    }
}
