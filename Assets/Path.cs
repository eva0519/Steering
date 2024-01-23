using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Path : MonoBehaviour
{
    public List<GameObject> nodes;
    public List<PathSegment> segments;

    // Start is called before the first frame update
    void Start()
    {
        segments = GetSegments();
    }

    public List<PathSegment> GetSegments()
    {
        List<PathSegment> segments = new List<PathSegment>();
        for (int i = 0; i < nodes.Count - 1; i++)
        {
            Vector3 src = nodes[i].transform.position;
            Vector3 dst = nodes[i+1].transform.position;
            PathSegment segment = new PathSegment(src, dst);
            segments.Add(segment);
        }
        
        return segments; 
    }

    public float GetParam(Vector3 position, float lastParam) {
        float param = 0f;
        PathSegment currentSegment = null;
        float tempParam = 0f;
        foreach (PathSegment ps in segments)
        {
            tempParam += Vector3.Distance(ps.a, ps.b);
            if (lastParam <= tempParam)
            {
                currentSegment = ps;
                break;
            }
        }
        if (currentSegment == null)
            return 0f;
        
        Vector3 currPos = position - currentSegment.a;
        Vector3 segmentDirection = currentSegment.b - currentSegment.a;
        segmentDirection.Normalize();

        Vector3 pointInSegment = Vector3.Project(currPos, segmentDirection);
        param = tempParam - Vector3.Distance(currentSegment.a, currentSegment.b);
        param += pointInSegment.magnitude;

        return param;
    }

    public Vector3 GetPosition(float param) {
        Vector3 position = Vector3.zero;
        PathSegment currentSegment = null;
        float tempParam = 0f;

        foreach (PathSegment ps in segments)
        {
            tempParam += Vector3.Distance(ps.a, ps.b);
            if (param <= tempParam)
            {
                currentSegment = ps;
                break;
            }
        }
        if (currentSegment == null)
            return Vector3.zero;
        
        Vector3 segmentDirection = currentSegment.b - currentSegment.a;
        segmentDirection.Normalize();
        tempParam -= Vector3.Distance(currentSegment.a, currentSegment.b);
        tempParam = param - tempParam;
        position = currentSegment.a + segmentDirection * tempParam;

        return position;
    }

    public void OnDrawGizmos() {
        Vector3 direction;
        Color tmp = Gizmos.color;
        Gizmos.color = Color.magenta;
        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 src = nodes[i].transform.position;
            Vector3 dst = nodes[i+1].transform.position;
            direction = dst - src;
            Gizmos.DrawRay(src, direction);
        }
        Gizmos.color = tmp;
    }
}
