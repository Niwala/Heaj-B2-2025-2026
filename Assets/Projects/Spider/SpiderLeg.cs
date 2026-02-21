using UnityEngine;

public class SpiderLeg : MonoBehaviour
{
    public float offset;
    public AnimationCurve snapCurve;
    public AnimationCurve heightCurve;
    public Vector2 result;

    private void OnDrawGizmos()
    {

        float x = transform.position.x + offset;

        float previous = Mathf.Floor(x);
        float next = previous + 1;
        float t = x - previous;


        //Evaluation position
        float height = heightCurve.Evaluate(t);
        t = snapCurve.Evaluate(t);
        float newX = previous + t - offset;


        //Draw gizmos
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Vector3.right * (previous - offset), 0.1f);
        Gizmos.DrawSphere(Vector3.right * (next - offset), 0.1f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector3(newX, height, 0), 0.1f);
        result = new Vector3(newX, height, 0);
    }

}
