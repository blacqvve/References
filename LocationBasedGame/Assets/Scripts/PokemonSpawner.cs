using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PokemonSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject loadingScreen;

    static public PokemonSpawner instance;
     void Awake()
    {
        instance = this;  
    }
    void Start()
    {
        string t = "PAKET";
        GameObject prefab = Resources.Load("CatchPokemon/" + t, typeof(GameObject)) as GameObject;
        GameObject pokemon = Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
        pokemon.transform.SetParent(transform);
        pokemon.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        PlayerPrefs.DeleteKey("POKEMON_KEY");
    }
    
    public static void Run()
    {
    
        instance.StartCoroutine("LoadScene");
    }
    IEnumerator LoadScene()
    {
        loadingScreen.SetActive(true);

        yield return new WaitForSeconds(3);


        AsyncOperation async = SceneManager.LoadSceneAsync("Test");

        while (!async.isDone)
        {
            print(async.progress);
            yield return null;
        }
    }
}

