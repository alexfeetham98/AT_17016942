using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MultiThreadedMeshDestroy : MonoBehaviour
{
    List<Action> functionsToRunInMainThread;

    private void Start()
    {
        functionsToRunInMainThread = new List<Action>();
    }

    private void Update()
    {
        while(functionsToRunInMainThread.Count > 0)
        {
            //Grab the first/oldest function in the list
            Action someFunc = functionsToRunInMainThread[0];
            functionsToRunInMainThread.RemoveAt(0);

            someFunc();
        }
    }

    public void StartThreadedFunction(Action someFunction)
    {
        Thread t = new Thread(new ThreadStart(someFunction));
        t.Start();
    }

    public void QueueMainThreadFunction(Action someFunction)
    {
        //someFunction(); //not okay if still in a child thread

        functionsToRunInMainThread.Add(someFunction);
    }

    void SlowFunctionThatDoesAUnityThing()
    {
        Thread.Sleep(2000);

        Action aFunction = () =>
        {
            this.transform.position = new Vector3(1, 1, 1); //Not allowed from a child thread
        };

        aFunction();

        QueueMainThreadFunction(aFunction);
        
    }
}