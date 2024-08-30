using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    [SerializeField] private GameObject _floorPrefab;

    private void Awake()
    {
        GameObject floor = Instantiate(_floorPrefab, new Vector3(0.4f, -0.3f, 0), Quaternion.identity);
        BoxCollider2D collider = floor.GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = floor.AddComponent<BoxCollider2D>();
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
