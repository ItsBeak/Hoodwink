using Mirror;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEditor.PackageManager;
using UnityEngine;

public class TripwireGadgetScript : H_GadgetBase
{
    [Header("Tripewire Locations")]
    [SerializeField] Transform startPoint; // The starting point of the raycast
    [SerializeField] Transform endPoint;   // The ending point of the raycast

    [Header("Tripwire Layers")]
    [SerializeField] LayerMask hitLayer;   // Layer mask to filter what the raycast can hit
    [SerializeField] LayerMask tripwireLayer; // Layer mask for objects on the tripwire layer

    [Header("Tripwire Player (this is an issue bc multiplayer)")]
    
    private GameObject playerObject; // Reference to the player prefab

    [Header("Tripwire Materials")]
    [SerializeField] Material armedMaterial;    // Material to use when the tripwire is armed
    [SerializeField] Material disarmedMaterial; // Material to use when the tripwire is disarmed
    [SerializeField] Material explodedMaterial; // Material to use when the tripwire is exploded

    //[SerializeField] LineRenderer lineRenderer; // Reference to the LineRenderer component
    [Header("Tripwire Objects")]
    [SerializeField] GameObject[] targetObjects; // Array of target objects to change the material/color
    [SerializeField] GameObject TestPrefab;

    private bool isArmed = false;  // Flag to track whether the tripwire is armed
    private bool hasExploded = false; // Flag to trach whether the tripwire has exploded

    [Header("Tripwire Explosion")]
    [SerializeField] float maxExplosionDamage = 100f; // Maximum damage the explosion can deal
    [SerializeField] float maxExplosionRadius = 5f;   // Radius beyond which the damage falls off completely

    // public float health = 100; // for testing before implementation

    public float waitTime; // time before explosion

    // public GameObject debug_hitPoint;


    private void Start()
    {
        // Initialize with disarmed material
        foreach (GameObject targetObject in targetObjects)
        {
            ChangeMaterial(targetObject, disarmedMaterial);
            isArmed = false;
        }
    }

    public override void UseGadget()
    {
        Vector3 newPos = gameObject.transform.position;
        Instantiate(TestPrefab, newPos, Quaternion.identity);
        startPoint = GameObject.FindGameObjectWithTag("StartPoint").transform;
        endPoint = GameObject.FindGameObjectWithTag("EndPoint").transform;
    }

    private void Update()
    {
        // Update the raycast
        RaycastHit hitInfo;
        bool raycastHit = Physics.Raycast(startPoint.position, endPoint.position - startPoint.position, out hitInfo, Vector3.Distance(startPoint.position, endPoint.position) + 0.5f, hitLayer);

        // Debug.DrawRay(startPoint.position, endPoint.position - startPoint.position, Color.blue, 0.1f);


        // put red sphere where the ray hit
        // debug_hitPoint.transform.position = hitInfo.point;

        if (hitInfo.collider.gameObject.name == "Player")
        {
            //if the trap is armed, explode
            if (isArmed && !hasExploded)
            {
                Debug.Log("Tick, Tick, Tick");
                Explode();
            }
        }
        if (hitInfo.collider.gameObject.name == "TripwireTarget" && !hasExploded)
        {
            Debug.Log("Tripwire touching target: Activate!");
            // Arm the tripwire
            isArmed = true;
            foreach (GameObject targetObject in targetObjects)
            {
                ChangeMaterial(targetObject, armedMaterial);
            }
        }
        else
        {

            // Raycast did not hit the endpoint or objects on the tripwire layer, disarm the tripwire
            if (isArmed && !hasExploded)
            {
                Debug.Log("Tripwire hit obstacle: " + hitInfo.collider.gameObject.name);
                isArmed = false;
                foreach (GameObject targetObject in targetObjects)
                {
                    ChangeMaterial(targetObject, disarmedMaterial);
                }
            }
        }

    }

    private void ChangeMaterial(GameObject target, Material newMaterial)
    {
        // Change the material of the target object
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = newMaterial;
        }
    }

    private void Explode()
    {
        // Trigger explosion logic here
        Debug.Log("Kaboom");
        // Change color or perform other explosion effects
        foreach (GameObject targetObject in targetObjects)
        {
            ChangeMaterial(targetObject, explodedMaterial);
            Debug.Log("ColourChange");
            hasExploded = true;
            playerObject = GameObject.FindGameObjectWithTag("Player");
        }
        StartCoroutine(TriggerDelay(waitTime));
    }

    public IEnumerator TriggerDelay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Debug.Log("Waiting");
        // Calculate damage based on distance from the player object and apply it
        float distance = Vector3.Distance(startPoint.position, playerObject.transform.position);
        float normalizedDistance = Mathf.Clamp01(distance / maxExplosionRadius);
        float damage = Mathf.Lerp(maxExplosionDamage, 0f, normalizedDistance);

        Debug.Log("Calculated");

        //Put damage taking here (refere to xanders sticky notes for explanation)
        //  health = health - damage;
        Debug.Log("Damage" + damage);
    }
}
