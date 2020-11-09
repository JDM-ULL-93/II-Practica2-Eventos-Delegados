using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;


/// <summary>
/// Read Only attribute.
/// Attribute is use only to mark ReadOnly properties.
/// </summary>
public class ReadOnlyAttribute : PropertyAttribute { }

/// <summary>
/// This class contain custom drawer for ReadOnly attribute.
/// </summary>
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    /// <summary>
    /// Unity method for drawing GUI in Editor
    /// </summary>
    /// <param name="position">Position.</param>
    /// <param name="property">Property.</param>
    /// <param name="label">Label.</param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Disabling edit for property
        GUI.enabled = false;
        // Drawing Property
        EditorGUI.PropertyField(position, property, label);
        // Setting old GUI enabled value
        GUI.enabled = true;
    }
}


public delegate void fOnPlayerGameObjectCollision(PlayerHandler handler, GameObject other);

public class PlayerHandler : MonoBehaviour
{
    private static Dictionary<int, PlayerHandler> playerDataBase = new Dictionary<int, PlayerHandler>();
    public static PlayerHandler GetPlayerInstance(int id)
    {
        PlayerHandler player;
        if (PlayerHandler.playerDataBase.TryGetValue(id, out player))
            return player;
        else
            return null;
    }

    private static int mainPlayerId = -1;
    public static int MainPlayerId { get => mainPlayerId; }

    private PlayerHandler() { }

    [Range(0.0f, 20.0f)]
    public float distanciaMedia = 5.0f;
    [Range(0.0f, 20.0f)]
    public float distanciaCerca = 3.0f;
    [Range(0.0f, 20.0f)]
    public float scale = 1.0f;
    [ReadOnly]
    public int id = -1;
    public bool isMainPlayer = true;
    public bool active = true;
    public enum KeyCodeFilter
    {
        W = KeyCode.W,
        S = KeyCode.S,
        A = KeyCode.A,
        D = KeyCode.D,
        Up = KeyCode.UpArrow,
        Down = KeyCode.DownArrow,
        Left = KeyCode.LeftArrow,
        Right = KeyCode.RightArrow,

        I = KeyCode.I,
        L = KeyCode.L,
        J = KeyCode.J,
        M = KeyCode.M
    }

    public KeyCodeFilter forward = (KeyCodeFilter)KeyCode.W;
    public KeyCodeFilter backward = (KeyCodeFilter)KeyCode.S;
    public KeyCodeFilter left = (KeyCodeFilter)KeyCode.A;
    public KeyCodeFilter right = (KeyCodeFilter)KeyCode.D;

    public event fOnPlayerGameObjectCollision eventOnPlayerGameObjectCollision;

    private GameObject mainCamera = null;
    private Rigidbody rb = null;
    private void Awake()
    {
        this.id = this.GetInstanceID();
        playerDataBase.Add(id, this);
        if (isMainPlayer) mainPlayerId = this.id;

        //rb = this.gameObject.AddComponent<Rigidbody>();
        //rb.isKinematic = true;
        Color random = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        this.gameObject.GetComponent<MeshRenderer>().material.color = random;

        this.GetComponent<CapsuleCollider>().isTrigger = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        TerrainHandler.Instance.eventOnAfterCreation += onAfterCreation;
        InterceptorHandler.eventOnInterceptorCollision += onInterceptorCollision;
    }
    void onAfterCreation(TerrainHandler.ReservedSpace[] rspace)
    {
        Vector3 tpPos = new Vector3((rspace[0].Origin.x + rspace[0].End.x) / 2, 1, (rspace[0].Origin.y + rspace[0].End.y) / 2);
        transform.position = tpPos;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!active)
            return;
        if (Input.GetKey((KeyCode)forward))
        {
            var mov = (Vector3.forward * Time.deltaTime) * scale;
            this.transform.Translate(mov);//==this.gameObject.transform.position = this.gameObject.transform.position + (Vector3.forward*Time.deltaTime)* scale;
        }

        if (Input.GetKey((KeyCode)backward))
        {
            var mov = (Vector3.back * Time.deltaTime) * scale;
            this.transform.Translate(mov);
        }

        if (Input.GetKey((KeyCode)left))
        {
            var mov = (Vector3.left * Time.deltaTime) * scale;
            this.transform.Translate(mov);
        }

        if (Input.GetKey((KeyCode)right))
        {
            var mov = (Vector3.right * Time.deltaTime) * scale;
            this.transform.Translate(mov);
        }

        if (Input.GetKey(KeyCode.Space)) //Disparo
        {
            Collider[] colliders = Physics.OverlapSphere(this.transform.position, distanciaCerca);
            foreach (Collider collider in colliders)
                if (collider.attachedRigidbody != null)
                    if (collider.gameObject.GetInstanceID() != TerrainHandler.Instance.gameObject.GetInstanceID())
                        GameObject.Destroy(collider.gameObject);

            colliders = Physics.OverlapSphere(this.transform.position, distanciaMedia);
            foreach (Collider collider in colliders)
                if (collider.attachedRigidbody != null)
                    collider.attachedRigidbody.AddExplosionForce(1.0f, this.transform.position, 10.0f, 3.0f, ForceMode.VelocityChange);
                   
        }

    }

    void onInterceptorCollision(InterceptorHandler handler)
    {
        if(this.distanciaCerca > 10)
            this.distanciaCerca -= 10;
        if (this.distanciaMedia > 10)
            this.distanciaMedia -= 5;
    }

    private void OnTriggerEnter(Collider other)
    {
        eventOnPlayerGameObjectCollision(this,other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        var coin = other.gameObject.GetComponent<CoinHandler>();
        if (coin != null && Input.GetKey(KeyCode.E))
        {
            this.distanciaCerca += (this.distanciaCerca * (coin.value/2))/100;
            this.distanciaMedia += (this.distanciaCerca * coin.value)/100;
            GameObject.Destroy(other.gameObject);
            return;
        }

        var flashlight = other.gameObject.GetComponent<FlashLightHandler>();
        if (flashlight != null && Input.GetKey(KeyCode.E))
        {
            if (!flashlight.on)
            {
                flashlight.spotlight.intensity = 3;
                flashlight.on = true;
            }
            else
            {
                flashlight.spotlight.intensity = 0;
                flashlight.on = false;
            }
            return;
        }




    }
}
