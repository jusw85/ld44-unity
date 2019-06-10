using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TextReplacer : MonoBehaviour
{
    [SerializeField] private SOStringMethod textSource;
    [SerializeField] private Text text;
    [SerializeField] private bool AlwaysUpdate;

    public UnityEvent evt;

    // TODO: event based rather than always update
    // 2D, LWRP 2019.2

//    private delegate string StringReturner();
//    private StringReturner stringReturner;

    private Func<string> textSourceFunc;

    private void Awake()
    {
//        stringReturner =
//            (StringReturner) Delegate.CreateDelegate(typeof(StringReturner), textSource, "ToString", false);

        textSourceFunc =
            (Func<string>) Delegate.CreateDelegate(typeof(Func<string>), textSource.Obj, textSource.Method);
    }

    private void OnEnable()
    {
        text.text = textSourceFunc();
    }

    private void Update()
    {
        if (AlwaysUpdate)
        {
            text.text = textSourceFunc();
        }
    }
}