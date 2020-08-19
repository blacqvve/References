using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    void FixedUpdate()
    {


        StartCoroutine(ChangeAnim());
    }

    IEnumerator ChangeAnim()
    {
        var changeRate = Random.Range(1, 10);
        yield return new WaitForSeconds(changeRate);
        var anim = gameObject.GetComponent<Animator>();
        anim.SetInteger("sneakPeak", 1);
        yield return new WaitForSeconds(changeRate);
        anim.SetInteger("sneakPeak", 0);
    }
}
