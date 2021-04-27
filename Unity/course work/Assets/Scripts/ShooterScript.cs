using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterScript : MonoBehaviour
{

    [SerializeField] private Transform SpawnPoint;
    [SerializeField] private GameObject Arrow;
    [SerializeField] private bool Enabled;
    private Animator Anime;
    // Start is called before the first frame update
    void Start()
    {
        Anime = GetComponent<Animator>();
        Anime.SetBool("Enabled", Enabled);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
    }
    private void Shoot()
    {
        Instantiate(Arrow, SpawnPoint.position, SpawnPoint.rotation);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Enabled = true;
            Anime.SetBool("Enabled", Enabled);
        }
    }
}
