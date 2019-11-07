using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollision : MonoBehaviour
{
    public bool colliding=false;
    // Start is called before the first frame update
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        
    }

    public void OnCollisionEnter(Collision collision){
        //Debug.Log("Collision enter");
        colliding = true;
    }

    public void OnCollisionExit(Collision collision){
        //Debug.Log("Collision exit");
        colliding = false;
    }
}
