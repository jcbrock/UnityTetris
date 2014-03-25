#pragma strict

var moveUp : KeyCode;
var moveDown : KeyCode;
var moveRight : KeyCode;
var moveLeft : KeyCode;

//var speed : float - 10;
var speed = -.5;

var counter = 0;
function Update ()
{

	if (Input.GetKey(moveUp))
	{
		//rigidbody2D.velocity.y = speed;
		rigidbody2D.transform.position.y += 1;
		//Debug.Log("MoveUp pressed");
	}
	else if (Input.GetKey(moveDown))
	{
		//rigidbody2D.velocity.y = speed *-1;
		rigidbody2D.transform.position.y -= 1;
		//Debug.Log("MoveDown pressed");
	}
	
	if (Input.GetKey(moveRight))
	{
		//rigidbody2D.velocity.y = speed;
		if (rigidbody2D.transform.position.x < 10){
		rigidbody2D.transform.position.x += 1;
		}
		//Debug.Log("MoveUp pressed");
	}
	else if (Input.GetKey(moveLeft))
	{
		//rigidbody2D.velocity.y = speed *-1;
		if (rigidbody2D.transform.position.x > 0){
		rigidbody2D.transform.position.x -= 1;
		}
		//Debug.Log("MoveDown pressed");
	}
}