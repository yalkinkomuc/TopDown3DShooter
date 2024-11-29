using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MissionObjectHuntTarget : MonoBehaviour
{
    public static event Action onTargetKilled;

    public void InvokeOnTargetKilled() => onTargetKilled?.Invoke();
}
