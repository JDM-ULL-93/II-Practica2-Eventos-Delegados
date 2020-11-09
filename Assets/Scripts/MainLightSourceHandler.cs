using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLightSourceHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TerrainHandler.Instance.eventOnAfterCreation += onAfterCreation;
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
