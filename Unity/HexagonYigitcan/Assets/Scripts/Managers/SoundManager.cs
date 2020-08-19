using System.Collections;
using System.Collections.Generic;
using Toolbox;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
     [SerializeField]
     Dictionary<SoundTypes, AudioClip> sounds;

     private AudioSource audioSource;
     private GridManager gridManager;

     private void Start()
     {
          //adding our audio clips to dictionary
          sounds = new Dictionary<SoundTypes, AudioClip>();
          sounds.Add(SoundTypes.Explosion, Resources.Load<AudioClip>("Sounds/explosion"));
          sounds.Add(SoundTypes.Selection, Resources.Load<AudioClip>("Sounds/selection"));
          audioSource = gameObject.GetComponent<AudioSource>();


          gridManager = GridManager.Instance;
          //subscribe to sound event
          gridManager.PlaySound += PlayClip;
     }

     public void PlayClip(SoundTypes key)
     {
          AudioClip clip;

          if (sounds.TryGetValue(key, out clip))
          {
               audioSource.PlayOneShot(clip);
          }
     }
}
