using System;

using UnityEngine;

public class SparksEffect : MonoBehaviour
{
    //Exposed
    [SerializeField]
    private float duration = 1.0f;

    [SerializeField]
    private AnimationCurve lightIntensityOverLifetime;

    [SerializeField]
    private new Light light;


    //Hidden
    [NonSerialized]
    public float lifetime;


    private void Update()
    {
        //Update lifetime
        lifetime += Time.deltaTime / duration;


        //Update light
        light.intensity = lightIntensityOverLifetime.Evaluate(lifetime);


        //Destroy
        if (lifetime >= 1.0f)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
