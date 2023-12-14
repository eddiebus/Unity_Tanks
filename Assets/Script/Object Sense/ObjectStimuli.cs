using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectStimuliType {
    Vision,
    Sound
}

public enum StimuliTag{
    Character,
    Item,
    Threat,
    Ambiance
}

public class ObjectStimuli : MonoBehaviour
{
    public ObjectStimuliType Type;
    public StimuliTag Tag;
}
