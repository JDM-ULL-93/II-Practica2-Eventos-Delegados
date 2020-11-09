using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rb = this.gameObject.AddComponent<Rigidbody>();
        
        TerrainHandler.Instance.eventOnAfterCreation += onAfterCreation;
    }

    void onAfterCreation(TerrainHandler.ReservedSpace[] rspace)
    {
        Vector3 tpPos = new Vector3((rspace[0].Origin.x + rspace[0].End.x) / 2 -10, 2, ((rspace[0].Origin.y + rspace[0].End.y) / 2) + 5);
        transform.position = tpPos;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
