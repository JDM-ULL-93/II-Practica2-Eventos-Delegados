using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void fOnInterceptorCollision(InterceptorHandler handler);
public class InterceptorHandler : MonoBehaviour
{
    public static event fOnInterceptorCollision eventOnInterceptorCollision;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rb = this.gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        TerrainHandler.Instance.eventOnAfterCreation += onAfterCreation;
        CoinHandler.eventOnCoinCollision += onCoinCollision;
        PlayerHandler.GetPlayerInstance(PlayerHandler.MainPlayerId).eventOnPlayerGameObjectCollision += OnPlayerGameObjectCollision;
    }

    void onAfterCreation(TerrainHandler.ReservedSpace[] rspace)
    {
        Vector3 tpPos = new Vector3((rspace[0].Origin.x + rspace[0].End.x) / 2 + 10, 0, ((rspace[0].Origin.y + rspace[0].End.y) / 2) + 7);
        transform.position = tpPos;
    }

    void onCoinCollision(CoinHandler handler)
    {
        Debug.Log("¡¡¡Alto policia!!!");
    }

    void OnPlayerGameObjectCollision(PlayerHandler handler, GameObject other)
    {
        if(other.GetInstanceID() == this.gameObject.GetInstanceID())
        {
            InterceptorHandler.eventOnInterceptorCollision(this);
            Color random = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            this.GetComponent<MeshRenderer>().material.color = random;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
