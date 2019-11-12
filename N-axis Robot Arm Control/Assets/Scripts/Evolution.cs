using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evolution : MonoBehaviour
{
    public GameObject robot;
    public GameObject goal;
    public GameObject testCube;
    private GameObject testing;
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

            testing = Instantiate(testCube, new Vector3(0, 0, 0), Quaternion.identity);
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

                float theta0 = robotState[0]*Mathf.PI/180.0f;
                float theta1 = robotState[1]*Mathf.PI/180.0f;
                float theta2 = robotState[2]*Mathf.PI/180.0f;

                /*float[,] h01 = new float[4,4] {{-Mathf.Sin(theta0), 0, Mathf.Cos(theta0), 0},
                                               { Mathf.Cos(theta0), 0, Mathf.Sin(theta0), 0},
                                               {0, 1, 0, distJoints},
                                               {0, 0, 0, 1}};
                float[,] h01 = new float[4,4] {{Mathf.Cos(theta0), 0, Mathf.Sin(theta0), 0},
                                               {Mathf.Sin(theta0), 0, -Mathf.Cos(theta0), 0},
                                               {0, 1, 0, distJoints},
                                               {0, 0, 0, 1}};

                //float theta1 = robotState[1]*Mathf.PI/180.0f;
                
                float[,] h12 = new float[4,4] {{-Mathf.Sin(theta1), 0, -Mathf.Cos(theta1), distJoints*Mathf.Sin(theta1)},
                                               { Mathf.Cos(theta1), 0, -Mathf.Sin(theta1), distJoints*Mathf.Cos(theta1)},
                                               {0, -1, 0, 0},
                                               {0, 0, 0, 1}};

                float[,] h12 = new float[4,4] {{Mathf.Cos(theta1), 0, Mathf.Sin(theta1), -distJoints*Mathf.Cos(theta1)},
                                               {Mathf.Sin(theta1), 0, -Mathf.Cos(theta1), -distJoints*Mathf.Sin(theta1)},
                                               {0, 1, 0, 0},
                                               {0, 0, 0, 1}};

                float[,] h02 = dot(h01, h12);*/

                float d = distJoints;
                float g = distGround;

                float dY = g+d + d*Mathf.Cos(theta1) + d*Mathf.Cos(theta1)*Mathf.Cos(theta2);
                //float dX = d*Mathf.Cos(theta0)*Mathf.Sin(theta1) + d*Mathf.Cos(theta0)*Mathf.Sin(theta1)*Mathf.Cos(theta2) + d*Mathf.Sin(theta0)*d*Mathf.Sin(theta1);
                float dX = d*Mathf.Cos(theta0)*Mathf.Sin(theta1) + d*Mathf.Cos(theta0)*Mathf.Sin(theta1)*Mathf.Cos(theta2) - d*Mathf.Sin(theta0)*Mathf.Sin(theta2);
                float dZ = d*Mathf.Sin(theta0)*Mathf.Sin(theta1) + d*Mathf.Sin(theta0)*Mathf.Sin(theta1)*Mathf.Cos(theta2) + d*Mathf.Cos(theta0)*Mathf.Sin(theta2);

                testing.transform.position = new Vector3(dX,dY,dZ);
                //testing.transform.position = new Vector3(h02[0,3],h02[2,3]+distGround,h02[1,3]);

                if(distToGoal()<=bestFitness){
                    bestInd = i;
                    bestFitness = distToGoal();
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

        for (int i = 0; i < N; i++){
            robotState = popStates[bestInd];
        }

        for(int i=0; i<popSize; i++){
            for (int j = 0; j < N; j++){
                float angle = Random.Range(-maxStep+robotState[j], maxStep+robotState[j]);
                popStates[i][j] = angle;
            }
        }
    }

    float distToGoal(){
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
