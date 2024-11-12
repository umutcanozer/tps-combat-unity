using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetable 
{
    GameObject gameObject { get; }
    Transform transform { get; }
}
