using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowReaderTest : MonoBehaviour {

    private Renderer rend;
    // Use this for initialization
    void Start()
    {
        rend = this.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (rend.IsVisibleFrom(Camera.main))
        //{
        //    Debug.Log("visible by main camera");
        //}
        //else
        //{
        //    Debug.Log("not visible by any camera");
        //}

        //if (rend.IsVisibleFrom(Camera.main)) Debug.Log("Visible");
        //else Debug.Log("Not visible");

        if (rend.isVisible)
        {
          //  Debug.LogError("red cube is Visible");
            gameObject.name = "Visible";
        }
        else
        {
            //Debug.LogError("red cube not Visible"); ;
            gameObject.name = "No Visible";
        }

        //if (Camera.main.useOcclusionCulling)
        //{
        //    Debug.Log("currrent occluson is using");
        //}
    }

    void OnWillRenderObject()
    {
        //Debug.Log("will render ");
    }


    void OnBecameVisible()
    {
        //Debug.Log("became Visible");
    }

    void OnBecameInvisible()
    {
        //Debug.Log("became InVisible");
    }
}
