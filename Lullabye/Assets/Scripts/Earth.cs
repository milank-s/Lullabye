using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
public class Earth : MonoBehaviour
{

    public EarthData earthData = null; 
	public bool foundLocation = false; 
	public Transform earthTarget, earth;
	public float lat, lon; 
	int attemptCount = 0;

	public IEnumerator GetLocation() {
		
		earthData = null; 

		while(earthData == null && attemptCount < 3) {

			UnityWebRequest www = UnityWebRequest.Get("http://ip-api.com/json") ;
			yield return www.SendWebRequest();
	
			attemptCount++;

			if(www.isNetworkError || www.isHttpError) {
				Debug.Log(www.error);
			}
			else { 
				//Debug.Log(www.downloadHandler.text);
				earthData = JsonUtility.FromJson<EarthData>(www.downloadHandler.text);
			}

			if(earthData != null && earthData.status == "success") {
				SetupData();
			} else {
				earthData = null;
			}

		}

		Debug.Log((foundLocation ? "Got" : "No") + " earth data.");

    }
    
	void SetupData() {

		foundLocation = true; 

		earthData.lon = (int)earthData.lon;
		earthData.lat = (int)earthData.lat;

		lat = earthData.lat; 
		lon = earthData.lon; 

		SetTransform(earthTarget,new Vector3(lat,lon,0f));

    }

    public static void SetTransform(Transform worldTransform, Vector3 rot) {
		worldTransform.Rotate(Vector3.up * rot.y, Space.World);
        worldTransform.Rotate(Vector3.right * -rot.x, Space.World);
        worldTransform.Rotate(Vector3.forward * rot.z, Space.Self);
    }

}

[System.Serializable]
public class EarthData 
	{
		public string asName;
		public string city;
		public string country;
		public string countryCode;
		public string ispName;
		public float lat;
		public float lon;
		public string orgName;
		public string ip;
		public string region;
		public string regionName;
		public string status; 
		public string timeZone;
		public string zip;

	/// 	"country": "COUNTRY",
	/// 	"countryCode": "COUNTRY CODE",
	/// 	"region": "REGION CODE",
	/// 	"regionName": "REGION NAME",
	/// 	"city": "CITY",
	/// 	"zip": "ZIP CODE",
	/// 	"lat": LATITUDE,
	/// 	"lon": LONGITUDE,
	/// 	"timezone": "TIME ZONE",
	/// 	"isp": "ISP NAME",
	/// 	"org": "ORGANIZATION NAME",
	/// 	"as": "AS NUMBER / NAME",
	/// 	"query": "IP ADDRESS USED FOR QUERY"
	}

