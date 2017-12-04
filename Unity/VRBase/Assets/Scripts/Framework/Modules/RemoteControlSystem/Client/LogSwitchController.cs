using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogSwitchController : MonoBehaviour {
    [RuntimeInitializeOnLoadMethod]
    static void RunOnStartGame()
    {
        //GameObject obj = new GameObject("[LogSwitchController]");
        //obj.AddComponent<LogSwitchController>();
    }
    private string ipAdress = "127.0.0.1";
    private int port = 1100;
    NetWorkManager netManager;
    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(this);
        netManager = new NetWorkManager(ipAdress, port);

        Application.runInBackground = true;
        Debug.Log("Start");
    }
	
	// Update is called once per frame
	void Update () {

        netManager.Update();


    }


}
