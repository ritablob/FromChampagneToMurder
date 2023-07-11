using System.Linq;
using UnityEngine;

public class DestroyPinsInArea : MonoBehaviour
{
    public float checkRadius = 5f;

    private void Start()
    {
        // Get all RectTransforms with the tag "Pin" within the circular area
        RectTransform[] pins = GameObject.FindGameObjectsWithTag("Pin")
            .Select(obj => obj.GetComponent<RectTransform>())
            .Where(rectTransform => rectTransform != null && IsWithinRadius(rectTransform))
            .ToArray();

        // Destroy the found pins
        foreach (RectTransform pin in pins)
        {
            Destroy(pin.gameObject);
        }

        // Destroy self
        Destroy(gameObject);
    }

    private bool IsWithinRadius(RectTransform rectTransform)
    {
        Vector3 center = transform.position;
        Vector3 pinPos = rectTransform.position;

        float distance = Vector3.Distance(center, pinPos);
        return distance <= checkRadius;
    }
}