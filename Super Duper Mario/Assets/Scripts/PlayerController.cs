using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{

    [SerializeField] private GameObject _playerPrefab;
    private Vector3 _initialPosition = new Vector3(0, 0, 0);
    private Rigidbody2D _rb;


    private void Awake()
    {
        GameObject player = Instantiate(_playerPrefab, _initialPosition , Quaternion.identity);
        _rb = player.GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            _rb = player.AddComponent<Rigidbody2D>();
        }

        BoxCollider2D collider = player.GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = player.AddComponent<BoxCollider2D>();
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            Debug.Log("Mario has landed on the floor.");
            // Here, ensure that velocity in the Y direction is set to zero
            _rb.velocity = new Vector2(_rb.velocity.x, 0);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
