using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateScene : MonoBehaviour
{
    public GameObject robotBase;
    public GameObject armPart;
    public GameObject armJoint;
    public GameObject armEnd;

    public GameObject obstacle;
    public int qtdObstacle;
    public int N;
    // Start is called before the first frame update
    void Start(){
        createRobotArm();
        //createObstacles();
    }

    // Update is called once per frame
    void Update(){
        
    }

    void createRobotArm(){
        GameObject lastArm = robotBase;
        for (int i = 0; i < N; i++)
        {
            GameObject nextArm = Instantiate(armPart, new Vector3(0, i*0.4f+0.2f, 0), Quaternion.identity);
            nextArm.transform.parent = lastArm.transform;
            GameObject joint = Instantiate(
                armJoint, 
                new Vector3(0, i*0.4f+0.2f, 0), 
                Quaternion.Euler(i%3!=1?0:90, i%3!=0?0:90, i%3!=2?0:90)
            );
            joint.transform.parent = lastArm.transform;
            lastArm = nextArm;
        }
        GameObject armEndgo = Instantiate(armEnd, new Vector3(0, N*0.4f+0.2f, 0), Quaternion.identity);
        armEndgo.transform.parent = lastArm.transform;
    }

    void createObstacles(){
        for (int i = 0; i < qtdObstacle; i++){
            tryAgain:
            float x = Random.Range(-2.0f, 2.0f);
            float z = Random.Range(-2.0f, 2.0f);
            Vector3 obstPos = new Vector3(x,0.5f,z);

            if(Vector3.Distance(obstPos, new Vector3(0.0f,0.5f,0.0f))<0.5f)
                goto tryAgain;

            Instantiate(obstacle, obstPos, Quaternion.identity);

        }
    }
}
