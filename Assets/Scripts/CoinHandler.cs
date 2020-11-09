using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void fOnCoinCollision(CoinHandler handler);
public delegate void fOnCoinDestroy(CoinHandler handler);
public class CoinHandler : MonoBehaviour
{
    public static event fOnCoinCollision eventOnCoinCollision;
    public static event fOnCoinDestroy eventOnCoinDestroy;
    // Start is called before the first frame update

    [ReadOnly]
    public int value = 100;
    void Start()
    {
        Rigidbody rb = this.gameObject.AddComponent<Rigidbody>();
        //rb.isKinematic = true;
        this.GetComponent<MeshRenderer>().material.color = Color.yellow;
        TerrainHandler.Instance.eventOnAfterCreation += onAfterCreation;
        InterceptorHandler.eventOnInterceptorCollision += onInterceptorCollision;
        PlayerHandler.GetPlayerInstance(PlayerHandler.MainPlayerId).eventOnPlayerGameObjectCollision += OnPlayerGameObjectCollision;
    }

    void onAfterCreation(TerrainHandler.ReservedSpace[] rspace)
    {
        Vector3 tpPos = new Vector3((rspace[0].Origin.x + rspace[0].End.x)/2+5, 1, ((rspace[0].Origin.y + rspace[0].End.y) / 2)+12 );
        transform.position = tpPos;
    }

    void OnPlayerGameObjectCollision(PlayerHandler handler, GameObject other)
    {
        if (other.GetInstanceID() == this.gameObject.GetInstanceID())
            CoinHandler.eventOnCoinCollision(this);
    }

    void onInterceptorCollision(InterceptorHandler handler)
    {
        this.value -= 10;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.rotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y - 1.0f, 0.0f);
    }

    private void OnDestroy()
    {
        CoinHandler.eventOnCoinDestroy(this);
    }
}

