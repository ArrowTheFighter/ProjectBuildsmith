using System;
using System.Collections;
using UnityEngine;

public class ConnectToPlatformsScript : MonoBehaviour
{
    public Material lineMaterial;
    [SerializeField] Connections[] lineConnections;

    bool RenderingLines;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int count = 0;
        foreach (var connection in lineConnections)
        {
            GameObject lineObject = new GameObject($"LineRenderer-{count}");
            count++;

            lineObject.transform.SetParent(transform);

            connection.lineRenderer = lineObject.AddComponent<LineRenderer>();
            connection.lineRenderer.SetPosition(0, transform.position);
            connection.lineRenderer.SetPosition(1, connection.target.position);
            connection.lineRenderer.textureMode = LineTextureMode.Tile;
            connection.lineRenderer.material = lineMaterial;
        }
        SetRenderingLines(false);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var connection in lineConnections)
        {
            connection.lineRenderer.SetPosition(1, connection.target.position);
        }
    }

    public void SetRenderingLines(bool rendering)
    {
        if (rendering)
        {
            RenderingLines = true;
            foreach (var connection in lineConnections)
            {
                connection.lineRenderer.gameObject.SetActive(true);
                StartCoroutine(DisableLineRenderer(connection.LineDuration, connection.lineRenderer.gameObject));
            }
        }
        else
        {
            RenderingLines = true;
            foreach (var connection in lineConnections)
            {
                connection.lineRenderer.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator DisableLineRenderer(float delay,GameObject gameObject)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

}

[Serializable]
class Connections
{
    public LineRenderer lineRenderer;
    public Transform target;
    public float LineDuration;
}
