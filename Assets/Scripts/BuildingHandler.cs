using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHandler : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject door;
    bool open = false;
    void Start()
    {
        TerrainHandler.Instance.eventOnAfterCreation += onAfterCreation;
        CoinHandler.eventOnCoinDestroy += onCoinDestroy;
        //var collider = this.gameObject.AddComponent<MeshCollider>();
        //collider.sharedMesh = GetComponent<MeshFilter>().sharedMesh;
        door = this.gameObject.GetComponentInChildren<DoorHandler>().gameObject;
    }

    void onAfterCreation(TerrainHandler.ReservedSpace[] rspace)
    {
        Vector3 tpPos = new Vector3((rspace[0].Origin.x + rspace[0].End.x) / 2 - 1, 3, ((rspace[0].Origin.y + rspace[0].End.y) / 2) + 2);
        transform.position = tpPos;
    }

    void onCoinDestroy(CoinHandler handler)
    {
        this.open = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (open)
        {
            door.transform.rotation = Quaternion.Euler(door.transform.rotation.eulerAngles.x - 1.0f,0 , 0);
            if (Mathf.Cos(door.transform.rotation.eulerAngles.x) == 0) open = false;
        }
           
    }
}
