using UnityEngine;

public class Fireworks_Noise : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField]
    private int pointCount = 12;

    [SerializeField]
    private float spacing = 1.0f; //TODO : Fix this shit

    //Noise
    [SerializeField, Header("Noise")]
    private float noiseFrequency = 1.0f;
    [SerializeField]
    private float noiseAmplitude = 1.0f;
    [SerializeField]
    private float noiseSpeed = 1.0f;

    private void Update()
    {
        //Check line renderer
        if (lineRenderer == null)
            return;

        lineRenderer.positionCount = pointCount;

        Vector3 origin = transform.position;
        origin.y *= spacing;

        for (int i = 0; i < pointCount; i++)
        {
            //Progress
            float t = i / (pointCount - 1.0f);

            //Noise
            float noise = Mathf.PerlinNoise1D((transform.position.y - i) * noiseFrequency + Time.time * noiseSpeed);
            noise -= 0.5f;
            noise *= noiseAmplitude * t;

            //Compute new position
            Vector3 p1 = origin + new Vector3(noise, i * -spacing, 0);

            //Set
            lineRenderer.SetPosition(i, p1);
        }
    }
}
