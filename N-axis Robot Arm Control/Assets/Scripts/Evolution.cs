using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evolution : MonoBehaviour
{
    public GameObject robot;
    public GameObject goal;
    private GameObject armEnd;
    private List<Vector3> robotState = new List<Vector3>();
    private List<List<Vector3> > popStates = new List<List<Vector3> >();
    private bool robotCreated = false;

    //---------- Evolution parameters ----------// 
    private int N = 0;
    public int popSize;
    public float maxStep;
    public bool colliding=false;

    void Awake(){
        N = GetComponent<CreateScene>().N;
    }

    void Start(){
        for (int i = 0; i < N; i++){
            robotState.Add(new Vector3(0,0,0));
        }
        
        for(int i=0; i<popSize; i++){
            popStates.Add(new List<Vector3>());
            for (int j= 0; j < N; j++){
                float x = Random.Range(-maxStep+robotState[j].x, maxStep+robotState[j].x);
                float y = Random.Range(-maxStep+robotState[j].y, maxStep+robotState[j].y);
                float z = Random.Range(-maxStep+robotState[j].z, maxStep+robotState[j].z);
                popStates[i].Add(new Vector3(x,y,z));
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
        }

        if(robotCreated){
            int bestInd = -1;
            float bestFitness = 10;

            for (int i = 0; i < popSize; i++){
                GameObject lastPiece = robot.transform.GetChild(1).gameObject;

                // Test individual arm position
                for(int j=0; j<N; j++){
                    lastPiece.transform.rotation = Quaternion.Euler(popStates[i][j].x, popStates[i][j].y, popStates[i][j].z);                    
                    lastPiece = lastPiece.transform.GetChild(1).gameObject;
                }
                
                // Check collisions
                lastPiece = robot.transform.GetChild(1).gameObject;
                colliding=false;
                for(int j=0; j<N; j++){
                    if(lastPiece.GetComponent<CheckCollision>().colliding==true){
                        colliding=true;
                    }
                    lastPiece = lastPiece.transform.GetChild(1).gameObject;
                }

                // Calculate fitness
                /*if(colliding==true){
                    continue;
                }else */if(distToGoal()<=bestFitness){
                    bestInd = i;
                    bestFitness = distToGoal();
                }
            }
            newPopulation(bestInd);
        }
    }

    void newPopulation(int bestInd){
        Debug.Log("best: "+bestInd);
        if(bestInd==-1){
            GameObject lastPiece = robot.transform.GetChild(1).gameObject;
            for(int j=0; j<N; j++){
                lastPiece.transform.rotation = Quaternion.Euler(robotState[j].x, robotState[j].y, robotState[j].z);                    
                lastPiece = lastPiece.transform.GetChild(1).gameObject;
            }
            return;
        }

        for (int i = 0; i < N; i++){
            robotState = popStates[bestInd];
        }

        for(int i=1; i<popSize; i++){
            for (int j= 0; j < N; j++){
                float x = Random.Range(-maxStep+robotState[j].x, maxStep+robotState[j].x);
                float y = Random.Range(-maxStep+robotState[j].y, maxStep+robotState[j].y);
                float z = Random.Range(-maxStep+robotState[j].z, maxStep+robotState[j].z);
                popStates[i][j] = new Vector3(x,y,z);
            }
        }
    }

    float distToGoal(){
        Vector3 robotP = armEnd.transform.position;
        Vector3 goalP = goal.transform.position;
        return Vector3.Distance(robotP, goalP);
    }
}
