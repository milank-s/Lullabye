using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Networking;
public class Airplanes : MonoBehaviour
{
    public bool debugGetFakePlanes;
    public Transform airport; 
    public Transform airplaneParent; 
    public Airplane airplanePrefab;
    public Airplane [] airplanes;
    public int airplaneLimit = 100; 
    public string arrivalCode = "YVR";
    public string departCode = "YVR";
    string api = "http://aviation-edge.com/v2/public/flights?key=d24318-7a42aa";
    string arriveArgument = "&arrIata=";
    string departArgument = "&depIata=";
    string limitArgument = "&limit=";
    private UnityWebRequest www = null;
	int attemptCount = 0;
    bool dataInit = false; 
    string airplaneString; 

	public IEnumerator GetAirplanes(bool getArriving = true) {

		while(dataInit == false && attemptCount < 3) {

            //Debug.Log("Web request sent.");
            string argument = (getArriving ? arriveArgument + arrivalCode : departArgument + departCode) + limitArgument + airplaneLimit.ToString();
            UnityWebRequest www = UnityWebRequest.Get(api + argument);    
			yield return www.SendWebRequest();
	
			attemptCount++;

			if(www.isNetworkError || www.isHttpError) {
				//Debug.Log(www.error);
			} else { 
                //Debug.Log(www.isDone);
                airplaneString = www.downloadHandler.text;
                dataInit = true; 
            }
		}

        if(dataInit && !debugGetFakePlanes) {
            CreatePlanes();
        } else {
            CreateFakePlanes();
        }

        Debug.Log((dataInit ? "Got" : "No") + " plane data.");

        dataInit = false; 
        airplaneString = "";
    }
    
	void CreatePlanes() {
       
        var airplaneData = JSON.Parse(airplaneString);

        Debug.Log("Creating " + airplaneData.Count + " airplanes.");

        airplanes = new Airplane[airplaneData.Count];

        for(int i = 0; i < airplaneData.Count; i++) {

            airplanes[i] = Instantiate(airplanePrefab.gameObject, Vector3.zero, Quaternion.identity,airplaneParent).GetComponent<Airplane>();

            airplanes[i].latitude = airplaneData[i]["geography"]["latitude"].AsFloat;
            airplanes[i].longitude = airplaneData[i]["geography"]["longitude"].AsFloat;
            airplanes[i].altitude = airplaneData[i]["geography"]["altitude"].AsFloat;
            airplanes[i].direction = airplaneData[i]["geography"]["direction"].AsFloat;
            airplanes[i].horizontal = airplaneData[i]["speed"]["horizontal"].AsFloat;
            airplanes[i].isGround = airplaneData[i]["speed"]["isGround"].AsInt == 1;
            airplanes[i].vertical = airplaneData[i]["speed"]["vertical"].AsFloat;
            airplanes[i].Init();

        }
        

    }

    void CreateFakePlanes() {
        

        Debug.Log("Creating " + airplaneLimit + " airplanes.");

        airplanes = new Airplane[airplaneLimit];

        for(int i = 0; i < airplaneLimit; i++) {

            airplanes[i] = Instantiate(airplanePrefab.gameObject, Vector3.zero, Quaternion.identity,airplaneParent).GetComponent<Airplane>();
            airplanes[i].latitude = Random.Range(-60f,60f);
            airplanes[i].longitude =  Random.Range(-180f,180f);
            airplanes[i].altitude = Random.Range(1000f,30000f);
            airplanes[i].direction = Random.Range(0f,360f);
            airplanes[i].horizontal = Random.Range(800f,925f);
            airplanes[i].isGround = false;
            airplanes[i].vertical = Random.Range(1000f,30000f);
            airplanes[i].Init();

        }
        
    }
}

/*
[
{
    https://aviation-edge.com/developers/

    "geography": {
        "latitude": 47.3428,
        "longitude": -122.025,
        "altitude": 10972.8,
        "direction": 343.57
    },
    "speed": {
        "horizontal": 818.676,
        "isGround": 0,
        "vertical": 1.188
    },

etc...
}
]
*/

