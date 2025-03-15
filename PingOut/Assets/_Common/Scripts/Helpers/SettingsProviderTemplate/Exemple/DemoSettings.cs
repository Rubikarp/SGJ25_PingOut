using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoSettings : SingletonSCO<DemoSettings>
{
    public string myString = "Hello World!";
    public int myInt = 42;
    public float myFloat = 3.14f;
    public bool myBool = true;
    public Color myColor = Color.red;
    public Vector3 myVector3 = Vector3.one;
}