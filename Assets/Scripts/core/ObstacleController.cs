using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
	private Rigidbody2D body;
	private Vector2 posVector = new Vector2();
    private Vector2 camPos;
    private Random rand = new Random();

    public float speedEffect;

	void Start()
	{
        //imageWidth = GetComponentInChildren<SpriteRenderer>().bounds.size.x;
        camPos = Camera.main.ViewportToWorldPoint(new Vector2(-0.15f, 0.5f));
        body = GetComponent<Rigidbody2D>();
		body.velocity = new Vector2(speedEffect, 0);
        generatePosition();

    }


	void Update()
	{

	}

	private void FixedUpdate()
	{

		if (body.transform.position.x <= camPos.x)
		{
            generatePosition();
		}


	}

    private float generateRandomYpos() {
        return Random.Range(-6f, 4.4f);
    }

    private void generatePosition() {
        posVector.x = 1.7f;
        posVector.y = generateRandomYpos();
        body.transform.position = posVector;
    }
}
