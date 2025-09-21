using System.Collections.Generic;

using UnityEngine;

public struct NoteHierarchy
{
    public ShaderTraining note;
    public List<ShaderTraining> childs;

    public ShaderTraining this[int index]
    {
        get
        {
            return childs[index];
        }
    }

    public int Count => childs.Count;
    public Transform transform => note.transform;
    public string name => note.name;

    public NoteHierarchy(ShaderTraining note)
    {
        this.note = note;
        childs = new List<ShaderTraining>();

        int childCount = note.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            ShaderTraining child = note.transform.GetChild(i).GetComponent<ShaderTraining>();
            if (child != null)
            {
                childs.Add(child);
                string name = $"{note.name} - {childs.Count}";
                if (child.gameObject.name != name)
                    child.gameObject.name = name;
            }
        }
    }
}