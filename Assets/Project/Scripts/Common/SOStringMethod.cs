using System;
using UnityEngine;

[Serializable]
public class SOStringMethod
{
    [SerializeField] private UnityEngine.Object obj;
    [SerializeField] private string method;

    public UnityEngine.Object Obj => obj;
    public string Method => method;
}