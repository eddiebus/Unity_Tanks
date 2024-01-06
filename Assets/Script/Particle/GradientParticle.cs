using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientParticlle : Particle
{
    public Color StartColour = Color.white;
    public Color EndColour = Color.white;

    public MaterialColour matColour;
    // Start is called before the first frame update
    void Start()
    {
        _ParticleInit();
        matColour = GetComponentInChildren<MaterialColour>();
        _UpdateColour();
    }

    // Update is called once per frame
    void Update()
    {
        _UpdateBody();
        _Tick();
        _UpdateColour();
    }

    private void _UpdateColour(){
        var LifeTimeFloat = _CurrentLifeTime / LifeTime;

        if (matColour){
            var TargetColour = Color.Lerp(EndColour,StartColour,LifeTimeFloat);
            matColour.Colour = TargetColour;
        }
    }
}
