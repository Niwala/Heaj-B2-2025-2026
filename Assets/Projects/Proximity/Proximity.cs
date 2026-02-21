using UnityEngine;

public class Proximity : MonoBehaviour
{
    //Exposed
    [SerializeField]
    private Transform target;

    [SerializeField]
    private MeshRenderer rend;


    //Hidden
    private MaterialPropertyBlock mpb;


    private void Start()
    {
        mpb = new MaterialPropertyBlock();
    }

    private void Update()
    {
        mpb.SetVector("_Target_Position", target.position);
        rend.SetPropertyBlock(mpb);
    }
}
