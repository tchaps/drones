using UnityEngine;
using System.Collections;

[System.Serializable]
public class ApplicationSettings
{
    public int populationSize;
    public string trackId;
    public int nbSelectedDrone;
    public int droneStep;
    public float droneMoveSpeed;
    public float defMutationProb;
    public float defMutationPerc;
    public float defCrossSwapProb;
    public float defMutationAmount;
}
