using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    public float lifeTime;

    public void Init(Vector3 force ,float lt)
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(force,ForceMode.Impulse);
        lifeTime = lt;
    }
    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime < 0)
        {
            gameObject.SetActive(false);
        }
    }
}
