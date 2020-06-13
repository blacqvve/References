using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Android;

public class PokemonManager : MonoBehaviour
{
    private TileManager tileManager;
    [SerializeField]
    private float waitSpawnTime, minIntervalTime, maxIntervalTime;
    [SerializeField]
    private GameObject loadingScreen;
    GameObject dialog;
    private List<Pokemon> pokemons = new List<Pokemon>();

    void Start()
    {
        tileManager = GameObject.FindGameObjectWithTag("TileManager").GetComponent<TileManager>();

    }

    void Update()
    {
        if (waitSpawnTime < Time.time)
        {
            waitSpawnTime = Time.time + UnityEngine.Random.Range(minIntervalTime, maxIntervalTime);
            SpawnPokemon();
        }

        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Stationary)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Debug.LogError(hit.transform.tag);
                if (hit.transform.tag == "Pokemon")
                {
                    Debug.LogWarning(hit.transform.tag);
                    Pokemon pokemon = hit.transform.GetComponent<Pokemon>();
                    PokemonBattle(pokemon.pokeType);
                }
            }
        }
    }
//    private void OnGUI()
//    {
//#if PLATFORM_ANDROID
//        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
//        {

//            dialog.AddComponent<CameraDialog>();
//            return;
//        }
//        else if (dialog != null)
//        {
//            Destroy(dialog);
//        }
//#endif
//    }

    void PokemonBattle(PokemonType type)
    {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
            dialog = new GameObject();

        }
#endif

        string t = type.ToString();
        PlayerPrefs.SetString("POKEMON_KEY", t);
        Invoke("ChangeScene", 1);



    }
    void ChangeScene()
    {


        StartCoroutine(LaodScene());


    }
    IEnumerator LaodScene()
    {
        if (Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            print(Permission.HasUserAuthorizedPermission(Permission.Camera));

            loadingScreen.SetActive(true);

            yield return new WaitForSeconds(2);
            


            AsyncOperation async = SceneManager.LoadSceneAsync("Catch");

            while (!async.isDone)
            {
                print(async.progress);
                yield return null;
            }
        }
        else
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
    }


    void SpawnPokemon()
    {
        PokemonType type = (PokemonType)(int)UnityEngine.Random.Range(0, Enum.GetValues(typeof(PokemonType)).Length);
        float newLat = tileManager.getLat + UnityEngine.Random.Range(-0.0001f, 0.0001f);
        float newLon = tileManager.getLon + UnityEngine.Random.Range(-0.0001f, 0.0001f);

        Pokemon prefab = Resources.Load("MapPokemon/" + type.ToString(), typeof(Pokemon)) as Pokemon;
        Pokemon pokemon = Instantiate(prefab, Vector3.zero, Quaternion.identity) as Pokemon;
        pokemon.tileManager = tileManager;
        pokemon.Init(newLat, newLon);

        pokemons.Add(pokemon);
    }

    public void UpdatePokemonPosition()
    {
        if (pokemons.Count == 0)
            return;

        Pokemon[] pokemon = pokemons.ToArray();
        for (int i = 0; i < pokemon.Length; i++)
        {
            pokemon[i].UpdatePosition();
        }
    }
}