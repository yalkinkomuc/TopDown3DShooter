using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mission : ScriptableObject
{
    public string MissionName;

    [TextArea]
    public string MissionDescription;

    public abstract void StartMission();
    public abstract bool MissionCompleted();

    public virtual void UpdateMission()
    {
        // mono behavior olmadığı  için update çalışmaz o yüzden başka yerden çağırıcaz.
    }

}
