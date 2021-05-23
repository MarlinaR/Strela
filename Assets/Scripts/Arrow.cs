using System;
using Blocks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Arrow : MonoBehaviour
{
    public float speed = 5.0f;
    public float selfDestructionTime;
    public AudioClip audioClip;

    private Rigidbody2D myRigidbody;
    private AudioSource myAudioSource;
    private float myTime;

    private void Awake()
    {
        myAudioSource = gameObject.AddComponent<AudioSource>();
        myAudioSource.clip = audioClip;
        myAudioSource.playOnAwake = false;
        myAudioSource.loop = false;
        myAudioSource.volume = 0.3f;
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        myRigidbody.velocity = transform.right * speed;
    }

    private void Update()
    {
        myTime += Time.deltaTime;
        if (myTime >= selfDestructionTime || transform.position.magnitude > 15)
        {
            Destroy(gameObject);
            return;
        }
        
        RotateToVelocity(myRigidbody.velocity);
    }

    private void RotateToVelocity(Vector2 velocity)
    {
        if (velocity.magnitude < float.Epsilon)
        {
            return;
        }
        
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var obj = other.gameObject;
        IInteractable interactable;
        if (obj.TryGetComponent(out interactable))
        {
            interactable.Interact();
        }
    }
}
