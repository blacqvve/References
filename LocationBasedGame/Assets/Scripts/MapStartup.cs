using Mapbox.Unity.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MapStartup : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject loadingScreen;
    [SerializeField]
    private AbstractMap map;
    [SerializeField]
    private Text errorText;

    private bool loaded = false;
    private bool connection = true;
    private bool location = true;
    float waitTime = 2f;
    private int maxWait = 90;
    private bool isRunning;

    private void Awake()
    {
        if (map!=null)
        {
            map.OnInitialized += () =>
            {
                var visualizer = map.MapVisualizer;
                visualizer.OnMapVisualizerStateChanged += (ModuleState s) =>
                {
                    print("Map State:" + s.ToString());
                    if (s!=ModuleState.Finished)
                    {
                        loadingScreen.SetActive(true);
                        StartCoroutine(Wait());
                    }
                };
            };
        }
        loadingScreen.SetActive(true);
        errorText.gameObject.SetActive(false);
    }

 

    private void Start()
    {
//#if PLATFORM_ANDROID
//        CheckLocation();


//#endif
        //if (Application.internetReachability==NetworkReachability.NotReachable)
        //{
        //    isRunning = false;
        //}

        map = this.gameObject.GetComponent<AbstractMap>();
        map.OnInitialized += Loading;
    }

    private void CheckLocation()
    {
        {

        if (!Input.location.isEnabledByUser||Input.location.status!=LocationServiceStatus.Running)
            location = false;
            StartCoroutine(WaitForLocaiton());
        }
    }

    void Loading()
    {
       
        StartCoroutine(Wait());
      
    }
    IEnumerator WaitForNetwork(string url)
    {
        isRunning = true;
        UnityWebRequest www = new UnityWebRequest(url);
       
        float elapsedTime = 0.0f;
        while (!www.isDone)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 10.0f && www.downloadProgress <= 0.5) break;
            yield return null;
        }
        if (!www.isDone||!string.IsNullOrEmpty(www.error))
        {
            print("Load Failed");
            loadingScreen.SetActive(true);
            errorText.text = "Bağlantı Hatası Tekrar Bağlanılıyor";
            errorText.gameObject.SetActive(true);
            isRunning = false;
            yield break;

        }
        errorText.gameObject.SetActive(false);
        loadingScreen.SetActive(false);


    }
    IEnumerator WaitForLocaiton()
    {
        while (location == false && maxWait > 0)
        {
            loadingScreen.SetActive(true);
            errorText.gameObject.SetActive(true);
            errorText.text = "Konum Bilgisi Bekleniyor";
            print("no location");

            if (Input.location.status == LocationServiceStatus.Running)
            {
                location = true;
            }
            maxWait--;
            yield return new WaitForSeconds(1f);
            if (maxWait < 1) yield break;

        }
        while (Input.location.isEnabledByUser)
        {
            loadingScreen.SetActive(false);
            errorText.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);
        }
        yield break;
      
    }
    private void LateUpdate()
    {
        //CheckLocation();
        //if (!isRunning)
        //{
        //    StartCoroutine(WaitForNetwork("google.com"));
        //}
    }
    IEnumerator Wait()
    {

        yield return new WaitForSeconds(3f);
        loadingScreen.SetActive(false);
        //while (map.MapVisualizer.State!=ModuleState.Finished)
        //{
        //    yield return new WaitForSeconds(2f);
        //    map.MapVisualizer.OnMapVisualizerStateChanged += (ModuleState s) =>
        //      {
        //          if (s==ModuleState.Finished)
        //          {
        //              return;
        //          }
        //      };
        //}
    }
}
