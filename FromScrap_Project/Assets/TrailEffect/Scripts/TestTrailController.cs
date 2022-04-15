using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTrailController : MonoBehaviour
{
    [SerializeField]
    private GameObject grassTrailPrefab;
    
    private TrailController grassTrailControlled;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grassTrailPrefab = GameObject.Instantiate(grassTrailPrefab, Vector3.zero, Quaternion.identity);
        grassTrailControlled = grassTrailPrefab.GetComponent<TrailController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forwardVector = (Vector3.Reflect(rb.velocity, Vector3.up) + rb.velocity);
        grassTrailControlled.AddNewPosition(transform.position, Vector3.up, forwardVector.normalized, rb.velocity.magnitude, transform.localScale.x);
    }
}
