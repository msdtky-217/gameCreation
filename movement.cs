using UnityEngine;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using WebSocketSharp;
using UniRx;
using System;
public class movement : MonoBehaviour
{
    [SerializeField] private string _serverAddress;
    [SerializeField] private int _port;
    [SerializeField] private Vector3 pos2;
    [SerializeField] private Transform _syncObjTransform;   //Share transform
    [SerializeField] private GameObject cube;
    [SerializeField] private SyncPhase _nowPhase;
    [SerializeField] private string[] sArray;
    private WebSocket ws;

    bool go;
    bool rightmove;
    int  rightwait = 0;
    bool leftmove;
    int  leftwait = 0;
    int mode = 0;
    public enum SyncPhase
    {
        Idling,
        Syncing
    }

    private void Awake()
    {
        cube = GameObject.Find("Cube");
        pos2 = new Vector3(-9.0f, 0.4737244f, 0.0f);
        var ca = "ws://" + _serverAddress + ":" + _port.ToString();
        Debug.Log("Connect to " + ca);
        ws = new WebSocket(ca);

        //Add Events
        //On catch message event
        ws.OnMessage += (object sender, MessageEventArgs e) => {
            print(e.Data);
            var sVector = e.Data;
            sVector = sVector.Substring(1, sVector.Length - 2);


            // split the items
            sArray = sVector.Split(',');
            // store as a Vector3

            try
            {
                pos2 = new Vector3(

            Convert.ToSingle(sArray[0]),
            Convert.ToSingle(sArray[1]),
            Convert.ToSingle(sArray[2])
                );
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }


        };

        //On error event
        ws.OnError += (sender, e) => {
            Debug.Log("WebSocket Error Message: " + e.Message);
            _nowPhase = SyncPhase.Idling;
        };

        //On WebSocket close event
        ws.OnClose += (sender, e) => {
            Debug.Log("Disconnected Server");
        };

        ws.Connect();


        _nowPhase = SyncPhase.Syncing;
    }
    // Use this for initialization
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        //移動方向判別
        if (go == true)
        {
            if (mode % 6 == 0)
            {
                transform.position += new Vector3((float)17.32, 0, 0);
            }
            else if (mode % 6 == 1)
            {
                transform.position += new Vector3((float)7.5, 0, 15);
            }
            else if(mode % 6 == 2)
            {
                transform.position += new Vector3((float)-7.5, 0, 15);
            }
            else if (mode % 6 == 3)
            {
                transform.position += new Vector3((float)-17.32, 0, 0);
            }
            else if (mode % 6 == 4)
            {
                transform.position += new Vector3((float)-7.5, 0, -15);
            }
            else if (mode % 6 == 5)
            {
                transform.position += new Vector3((float)7.5, 0, -15);
            }
            OnChangedTargetTransformValue(transform.position);
            go = false;
            
        }
        //方向転換
        if (rightmove == true)
        {
            Vector3 localTrans = transform.localEulerAngles;
            if (localTrans.y==360)
            {
                localTrans.y = 60;
            } else {
                localTrans.y += 60;
            }
            
            transform.localEulerAngles = localTrans;
            rightmove = false;
            
            

        }
        if (leftmove == true)
        {
            Vector3 localTrans = transform.localEulerAngles;
            if (localTrans.y == 0)
            {
                localTrans.y = 300;
            }
            else
            {
                localTrans.y -= 60;
            }
            transform.localEulerAngles = localTrans;
            leftmove = false;
        }



        cube.transform.position = pos2;
    }
    public void goOn()
    {
        go = true;
    }
    public void rightOn()
    {
        rightmove = true;
        if (mode==0)
        {
            mode = 5;
        } else
        {
            mode--;
        }
        
    }
    public void leftOn()
    {
        leftmove = true;
        if (mode == 5)
        {
            mode = 0;
        }
        else
        {
            mode++;
        }
        
    }
    public void OnChangedTargetTransformValue(Vector3 pos)
    {
        if (_nowPhase == SyncPhase.Syncing)
        {
            Debug.Log(pos);
            ws.Send(pos.ToString());
        }
    }
}
