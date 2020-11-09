using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLightHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public Light spotlight;
    public bool on = false;
    void Start()
    {
        Rigidbody rb = this.gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        spotlight = this.gameObject.GetComponentInChildren<Light>();
        spotlight.spotAngle = 65;
        spotlight.intensity = 0;
        TerrainHandler.Instance.eventOnAfterCreation += onAfterCreation;

        var collider = this.gameObject.AddComponent<MeshCollider>();
        collider.sharedMesh = GetComponent<MeshFilter>().sharedMesh;
    }

    void onAfterCreation(TerrainHandler.ReservedSpace[] rspace)
    {
        Vector3 tpPos = new Vector3((rspace[0].Origin.x + rspace[0].End.x) / 2 - 1, 1, ((rspace[0].Origin.y + rspace[0].End.y) / 2) + 2);
        transform.position = tpPos;
    }
    // Update is called once per frame
    void Update()
    {

    }
}

