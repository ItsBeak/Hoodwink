using UnityEngine;
using Mirror;

public class H_DestroyAfterTime : MonoBehaviour
{
    public bool destroyLocal = false;
    public float lifetime = 1;

    void Start()
    {
        if (destroyLocal)
        {
            Invoke("DestroyObjectLocal", lifetime);

        }
        else
        {
            Invoke("DestroyObject", lifetime);
        }

    }

    public void DestroyObject()
    {
        NetworkServer.Destroy(gameObject);
    }

    public void DestroyObjectLocal()
    {
        Destroy(gameObject);
    }
}
