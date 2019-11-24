using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
	private Rigidbody2D body;
	private Vector2 posVector = new Vector2();
    private Vector2 camPos;

    public float speedEffect;

    public void init() {
        body.velocity = new Vector2(speedEffect, 0);
        generatePosition();
    }

	void Start() {
        camPos = Camera.main.ViewportToWorldPoint(new Vector2(-0.15f, 0.5f));
        body = GetComponent<Rigidbody2D>();
		body.velocity = new Vector2(speedEffect, 0);
        generatePosition();
       }


	void Update(){ }

	private void FixedUpdate()
	{

		if (body.transform.position.x <= camPos.x)
		{
            //generatePosition();
            GameEventManager.instance.ObstacleRecycle(this);

		}


	}

    private float generateRandomYpos() {
        return Random.Range(-4.7f, 5.2f);
    }

    private void generatePosition() {
        posVector.x = 2.1f;
        //posVector.y = body.position.y;
        posVector.y = generateRandomYpos();
        body.transform.position = posVector;
    }
}
