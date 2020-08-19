using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    // Start is called before the first frame update

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag=="Building")
        {
            var mr = collision.gameObject.GetComponent<MeshRenderer>();
            var collider = collision.gameObject.GetComponent<BoxCollider>();
            collision.gameObject.layer = 2;
            mr.enabled = false;
          
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        print(collision.gameObject.tag+"exit");
        if (collision.gameObject.tag=="Building")
        {
            var mr = collision.gameObject.GetComponent<MeshRenderer>();
            collision.gameObject.layer = 0;
            mr.enabled = true;
        }
    }
}
