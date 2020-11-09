using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TerrainHandler.Instance.eventOnBeforeCreation += onBeforeCreation;
        TerrainHandler.Instance.eventOnAfterCreation += onAfterCreation;
    }

    void onBeforeCreation(TerrainHandler handler)
    {
        handler.addReservedSpace(new TerrainHandler.ReservedSpace(50, 50, 80, 80));
    }
    void onAfterCreation(TerrainHandler.ReservedSpace[] rspace)
    {
        Vector3 tpPos = new Vector3((rspace[0].Origin.x + rspace[0].End.x) / 2, 10, ((rspace[0].Origin.y + rspace[0].End.y) / 2) - 10);
        transform.position = tpPos;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
