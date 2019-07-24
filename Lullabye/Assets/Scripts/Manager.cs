using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Manager : MonoBehaviour
{

    public static Manager instance; 
    public Earth earth; 
    public Airplanes airplanes; 
    void Awake() {
        instance = this; 
    }

    void Start() {
        StartCoroutine(GameCoroutine());
    }

    IEnumerator GameCoroutine() {
        Debug.Log("Requesting earth data.");
        yield return StartCoroutine(earth.GetLocation());
        Debug.Log("Requesting plane data.");
        yield return StartCoroutine(airplanes.GetAirplanes());  
        Debug.Log("Requesting plane data.");
        yield return StartCoroutine(airplanes.GetAirplanes(false));
    }

    void Update() {
        Shader.SetGlobalVector("_PlayerPos", transform.position); //"transform" is the transform of the Player
        Shader.SetGlobalVector("_LeftHand", transform.position); //"transform" is the transform of the Player
        Shader.SetGlobalVector("_RightHand", transform.position); //"transform" is the transform of the Player
    }

}
