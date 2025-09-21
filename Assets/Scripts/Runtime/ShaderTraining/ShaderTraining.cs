using System.Collections.Generic;

using UnityEngine;

[ExecuteInEditMode, AddComponentMenu("Shader Training")]
public class ShaderTraining : MarkdownComponent
{
    public Shader practiceShader;
    public Shader solutionShader;

    public MeshRenderer practice;
    public MeshRenderer solution;

    private MaterialPropertyBlock mpb;
    public Object[] objects = new Object[0];
    public FloatProperties[] floats = new FloatProperties[0];

    public override bool requireSceneRepaint => floats.Length > 0;

    //private void OnValidate()
    //{
    //    practiceShader = practice?.sharedMaterial?.shader;
    //    solutionShader = solution?.sharedMaterial?.shader;
    //}

    private void Update()
    {
        SyncShader(Time.deltaTime);
    }

    public void SyncShader(float deltaTime)
    {
        if (floats.Length == 0)
            return;

        if (mpb == null)
            mpb = new MaterialPropertyBlock();

        for (int i = 0; i < floats.Length; i++)
        {
            floats[i].Execute(deltaTime, mpb);
        }

        practice?.SetPropertyBlock(mpb);
        solution?.SetPropertyBlock(mpb);
    }

    [System.Serializable]
    public struct FloatProperties
    {
        public string name;
        public AnimationCurve curve;
        public float duration;
        private float lifeTime;

        public void Execute(float deltaTime, MaterialPropertyBlock mpb)
        {
            if (string.IsNullOrEmpty(name) || curve == null)
                return;

            lifeTime = (lifeTime + deltaTime / duration) % 1.0f;

            float value = curve.Evaluate(lifeTime);
            mpb.SetFloat(name, value);
        }

    }

}
