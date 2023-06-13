using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class SimpleGradientUI : BaseMeshEffect
{
    [SerializeField] private Color topColor = Color.white;
    [SerializeField] private bool useTopColor = true;

    [SerializeField] private Color bottomColor = Color.black;
    [SerializeField] private bool useBottomColor = true;

    [SerializeField] private Color leftColor = Color.red;
    [SerializeField] private bool useLeftColor = true;

    [SerializeField] private Color rightColor = Color.blue;
    [SerializeField] private bool useRightColor = true;

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;

        var vertexList = new List<UIVertex>();
        vh.GetUIVertexStream(vertexList);

        ApplyGradient(vertexList);

        vh.Clear();
        vh.AddUIVertexTriangleStream(vertexList);
    }
    private void ApplyGradient(List<UIVertex> vertexList)
    {
        if (vertexList.Count == 0)
            return;

        float bottomY = vertexList[0].position.y;
        float topY = vertexList[0].position.y;
        float leftX = vertexList[0].position.x;
        float rightX = vertexList[0].position.x;

        for (int i = 1; i < vertexList.Count; i++)
        {
            float y = vertexList[i].position.y;
            if (y > topY)
                topY = y;
            else if (y < bottomY)
                bottomY = y;

            float x = vertexList[i].position.x;
            if (x > rightX)
                rightX = x;
            else if (x < leftX)
                leftX = x;
        }

        float uiElementHeight = topY - bottomY;
        float uiElementWidth = rightX - leftX;

        for (int i = 0; i < vertexList.Count; i++)
        {
            UIVertex vertex = vertexList[i];
            float normalizedY = (vertex.position.y - bottomY) / uiElementHeight;
            float normalizedX = (vertex.position.x - leftX) / uiElementWidth;

            Color color = vertex.color;

            if (useTopColor && normalizedY >= 0.5f)
                color = Color.Lerp(color, topColor, normalizedY - 0.5f);

            if (useBottomColor && normalizedY < 0.5f)
                color = Color.Lerp(color, bottomColor, 0.5f - normalizedY);

            if (useLeftColor && normalizedX < 0.5f)
                color = Color.Lerp(color, leftColor, 0.5f - normalizedX);

            if (useRightColor && normalizedX >= 0.5f)
                color = Color.Lerp(color, rightColor, normalizedX - 0.5f);

            vertex.color = color;
            vertexList[i] = vertex;
        }
    }
}
