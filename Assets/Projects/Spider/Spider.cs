using UnityEngine;

public class Spider : MonoBehaviour
{
    public SpiderLeg[] legs;

    public float lengthA;
    public float lengthB;

    private void OnDrawGizmos()
    {
        for (int i = 0; i < legs.Length; i++)
        {
            Vector2 A = transform.position;
            Vector2 B = legs[i].result;
            Vector2 C = FindKnee(A, B, lengthA, lengthB);
            Gizmos.DrawLine(A, C);
            Gizmos.DrawLine(C, B);
        }
    }

    private static Vector2 FindKnee(Vector2 A, Vector2 B, float lengthA, float lengthB)
    {
        float a = lengthA;
        float b = lengthB;
        float c = Vector2.Distance(A, B);

        if (A.x < B.x)
        {
            (A, B) = (B, A);
            (a, b) = (b, a);
        }

        Vector2 AB = (A - B).normalized;

        float a2 = a * a;
        float b2 = b * b;
        float c2 = c * c;

        float baseAngle = Mathf.Atan2(AB.x, AB.y);
        float angleA = baseAngle - Mathf.Acos((b2 + c2 - a2) / (2 * b * c));

        return B + new Vector2(Mathf.Sin(angleA), Mathf.Cos(angleA)) * b;
    }

}
