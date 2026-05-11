using System.Collections.Generic;
using UnityEngine;

/*
체인 공격 비주얼 line 관리
- line renderer 업데이트 및 랜더링
*/


public class ChainLineRendererView
{
    private readonly Transform _owner;
    private readonly LineRenderer _template;
    private readonly List<LineRenderer> _lineRenderers = new List<LineRenderer>();

    public ChainLineRendererView(Transform owner, LineRenderer template)
    {
        _owner = owner;
        _template = template;
        _lineRenderers.Add(template);
        _template.positionCount = 0;
    }

    public void Draw(Vector3 origin, IReadOnlyList<List<Enemy>> branches, int fallbackRadius)
    {
        Activate(branches);

        if (branches.Count == 0)
        {
            _template.SetPosition(0, origin);
            _template.SetPosition(1, origin + Vector3.right * fallbackRadius);
            return;
        }

        for (int branchIndex = 0; branchIndex < branches.Count; branchIndex++)
        {
            LineRenderer lineRenderer = GetLineRenderer(branchIndex);
            List<Enemy> branch = branches[branchIndex];

            lineRenderer.SetPosition(0, origin);
            for (int i = 0; i < branch.Count; i++)
            {
                lineRenderer.SetPosition(i + 1, branch[i].transform.position);
            }
        }
    }

    public void Clear()
    {
        for (int i = 0; i < _lineRenderers.Count; i++)
        {
            _lineRenderers[i].positionCount = 0;
        }
    }

    private void Activate(IReadOnlyList<List<Enemy>> branches)
    {
        if (branches.Count == 0)
        {
            _template.positionCount = 2;
            for (int i = 1; i < _lineRenderers.Count; i++)
            {
                _lineRenderers[i].positionCount = 0;
            }

            return;
        }

        for (int i = 0; i < branches.Count; i++)
        {
            LineRenderer lineRenderer = GetLineRenderer(i);
            lineRenderer.positionCount = Mathf.Max(branches[i].Count + 1, 2);
        }

        for (int i = branches.Count; i < _lineRenderers.Count; i++)
        {
            _lineRenderers[i].positionCount = 0;
        }
    }

    private LineRenderer GetLineRenderer(int index)
    {
        while (_lineRenderers.Count <= index)
        {
            LineRenderer lineRenderer = CreateBranchLineRenderer(_lineRenderers.Count);
            _lineRenderers.Add(lineRenderer);
        }

        return _lineRenderers[index];
    }

    private LineRenderer CreateBranchLineRenderer(int index)
    {
        GameObject lineObject = new GameObject($"ChainBranchLine_{index}");
        lineObject.transform.SetParent(_owner);

        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.useWorldSpace = _template.useWorldSpace;
        lineRenderer.material = _template.material;
        lineRenderer.startWidth = _template.startWidth;
        lineRenderer.endWidth = _template.endWidth;
        lineRenderer.startColor = _template.startColor;
        lineRenderer.endColor = _template.endColor;
        lineRenderer.sortingLayerID = _template.sortingLayerID;
        lineRenderer.sortingOrder = _template.sortingOrder;
        lineRenderer.textureMode = _template.textureMode;
        lineRenderer.alignment = _template.alignment;
        lineRenderer.numCapVertices = _template.numCapVertices;
        lineRenderer.numCornerVertices = _template.numCornerVertices;
        lineRenderer.positionCount = 0;

        return lineRenderer;
    }
}
