using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Evolution : MonoBehaviour
{
    public GameObject robot;
    public GameObject goal;
    public GameObject testCube;
    //private GameObject testing;
    private GameObject armEnd;
    private List<float> robotState = new List<float>();
    private List<List<float> > popStates = new List<List<float> >();
    private bool robotCreated = false;

    //---------- Evolution parameters ----------// 
    private int N = 0;
    public int popSize;
    public float maxStep;
    public bool colliding=false;

    //---------- Forward Kinematics ----------//
    private float distGround = 0.2f;
    private float distJoints = 0.4f;

    void Awake(){
        N = GetComponent<CreateScene>().N;
    }

    void Start(){
        for (int i = 0; i < N; i++){
            robotState.Add(0);
        }
        robotState[0]=0;
        robotState[1]=0;
        robotState[2]=0;
        
        for(int i=0; i<popSize; i++){
            popStates.Add(new List<float>());
            for (int j= 0; j < N; j++){
                float angle = Random.Range(-maxStep+robotState[j], maxStep+robotState[j]);

                popStates[i].Add(angle);
            }
        }
    }

    // Update is called once per frame
    void Update(){
        if(!robotCreated && robot.transform.childCount != 0){
            robotCreated=true;
            Debug.Log("created");

            // Update arm end object
            GameObject lastPiece = robot.transform.GetChild(1).gameObject;
            for(int i=0; i<N-1; i++){
                lastPiece = lastPiece.transform.GetChild(1).gameObject;
            }
            armEnd = lastPiece.transform.GetChild(1).gameObject;

            //testing = Instantiate(testCube, new Vector3(0, 0, 0), Quaternion.identity);
        }
        //robotCreated = false;
        if(robotCreated){
            int bestInd = -1;
            float bestFitness = 1000;// inf

            for (int i = 0; i < popSize; i++){
                 GameObject lastPiece = robot.transform.GetChild(1).gameObject;

                // Test individual arm position
                for(int j=0; j<N; j++){
                    lastPiece.transform.localRotation = Quaternion.Euler(
                         j%3!=2?0:popStates[i][j],
                         j%3!=0?0:-popStates[i][j],
                         j%3!=1?0:-popStates[i][j]
                        );
                    lastPiece = lastPiece.transform.GetChild(1).gameObject;
                }
                
                // Check collisions
                /*lastPiece = robot.transform.GetChild(1).gameObject;
                colliding=false;
                for(int j=0; j<N; j++){
                    if(lastPiece.GetComponent<CheckCollision>().colliding==true){
                        colliding=true;
                    }
                    lastPiece = lastPiece.transform.GetChild(1).gameObject;
                }*/

                if(distToGoal(i)<=bestFitness){
                    bestInd = i;
                    bestFitness = distToGoal(i);
                }
            }

            newPopulation(bestInd);
        }
    }

    void newPopulation(int bestInd){
        /*if(bestInd==-1){
            GameObject lastPiece = robot.transform.GetChild(1).gameObject;
            for(int j=0; j<N; j++){
                lastPiece.transform.rotation = Quaternion.Euler(robotState[j].x, robotState[j].y, robotState[j].z);                    
                lastPiece = lastPiece.transform.GetChild(1).gameObject;
            }
            return;
        }*/

       GameObject lastPiece = robot.transform.GetChild(1).gameObject;
        // Test individual arm position
        for(int j=0; j<N; j++){
            lastPiece.transform.localRotation = Quaternion.Euler(
                    j%3!=2?0:popStates[bestInd][j],
                    j%3!=0?0:-popStates[bestInd][j],
                    j%3!=1?0:-popStates[bestInd][j]
                );
            lastPiece = lastPiece.transform.GetChild(1).gameObject;
        }

        for (int i = 0; i < N; i++){
            robotState = popStates[bestInd];
        }
        for(int i=0;i<N;i++){
            popStates[0][i] = robotState[i];
        }
        
        for(int i=1; i<popSize; i++){
            for (int j = 0; j < N; j++){
                float angle = Random.Range(-maxStep+robotState[j], maxStep+robotState[j]);
                popStates[i][j] = angle;
            }
        }
    }

    float distToGoal(int i){
        // Calculation of end position with 1, 2, 3 and 4 joints

        // float theta0 = popStates[i][0]*Mathf.PI/180.0f;
        // float theta1 = popStates[i][1]*Mathf.PI/180.0f;
        // float theta2 = popStates[i][2]*Mathf.PI/180.0f;

        // float d = distJoints;
        // float g = distGround;
        // 1 joints
        // float dX = 0;
        // float dY = g+d;
        // float dX = 0;

        // 2 joints
        // float dX = d*Mathf.Cos(theta0)*Mathf.Sin(theta1);
        // float dY = g+d + d*Mathf.Cos(theta1);
        // float dZ = d*Mathf.Sin(theta0)*Mathf.Sin(theta1);


        //testing.transform.position = new Vector3(dX,dY,dZ);
        // 3 joints
        // float dX = d*Mathf.Cos(theta0)*Mathf.Sin(theta1) + d*Mathf.Cos(theta0)*Mathf.Sin(theta1)*Mathf.Cos(theta2) - d*Mathf.Sin(theta0)*Mathf.Sin(theta2);
        // float dY = g+d + d*Mathf.Cos(theta1) + d*Mathf.Cos(theta1)*Mathf.Cos(theta2);
        // float dZ = d*Mathf.Sin(theta0)*Mathf.Sin(theta1) + d*Mathf.Sin(theta0)*Mathf.Sin(theta1)*Mathf.Cos(theta2) + d*Mathf.Cos(theta0)*Mathf.Sin(theta2);

        // 4 joints
        // float dX = d*Mathf.Cos(theta0)*Mathf.Sin(theta1) + 2*(d*Mathf.Cos(theta0)*Mathf.Sin(theta1)*Mathf.Cos(theta2) - d*Mathf.Sin(theta0)*Mathf.Sin(theta2));
        // float dY = g+d + d*Mathf.Cos(theta1) + 2*d*Mathf.Cos(theta1)*Mathf.Cos(theta2);
        // float dZ = d*Mathf.Sin(theta0)*Mathf.Sin(theta1) + 2*(d*Mathf.Sin(theta0)*Mathf.Sin(theta1)*Mathf.Cos(theta2) + d*Mathf.Cos(theta0)*Mathf.Sin(theta2));
        
        //Vector3 robotP = new Vector3(dX, dY, dZ);

        Vector3 robotP = armEnd.transform.position;
        Vector3 goalP = goal.transform.position;
        return Vector3.Distance(robotP, goalP);
    }

    float[,] dot(float[,] A, float[,] B){
        float[,] C = new float[4,4];

        float temp;
        for (int i = 0; i < 4; i++){
            for (int j = 0; j < 4; j++){
                temp = 0;
                for (int k = 0; k < 4; k++){
                    temp += A[i, k] + B [k, j];
                }
                C[i, j] = temp;
            }
        }
        return C;
    }
}
