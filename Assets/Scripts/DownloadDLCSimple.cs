using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadDLCSimple : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(DownloadDLCFromServer());
    }

    private IEnumerator DownloadDLCFromServer()
    {
        GameObject go = null;
        string url = "http://drive.usercontent.google.com/u/0/uc?id=1pFKow8VgKg6Cdjc6pOA8qIaOsk4lInyZ&export=download";
        using(UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url))
        {
            yield return www.SendWebRequest();
            if(www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogWarning("Error get request at : " + url + " " + www.error);
            }
            else
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
                go = bundle.LoadAsset(bundle.GetAllAssetNames()[0]) as GameObject;
                bundle.Unload(false);
                yield return new WaitForEndOfFrame();
            }
            www.Dispose();
        }

        InstantiateGameObjectFromDLC(go);
    }

    private void InstantiateGameObjectFromDLC(GameObject go)
    {
        if (go != null)
        {
            GameObject instanceGo = Instantiate(go);
            instanceGo.transform.position = Vector3.zero;
        }
        else
        {
            Debug.LogWarning("Your DLC is null");
        }
        
    }
}
