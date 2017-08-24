// JavaScript Document
#pragma strict

//var forwardRate : float = 1;
//var turnRate : float = 1;

var speed = 1;

function Update () {
/*
// tank's forward speed in action
var forwardMoveAmount = Input.GetAxis("Vertical") * forwardRate;

// force of the tank's turn
var turnForce = Input.GetAxis("Horizontal") * turnRate;

// rotate tank in action
transform.Rotate(0,turnForce,0);

transform.position += transform.right * forwardMoveAmount * Time.deltaTime;
*/
	
  if (Input.GetKey(KeyCode.D))
      transform.Rotate(Vector3.up * speed * Time.deltaTime);
      
  if (Input.GetKey(KeyCode.Q))
      transform.Rotate(-Vector3.up * speed * Time.deltaTime);

  if (Input.GetKey(KeyCode.Z))
      transform.position += transform.forward * speed/20 * Time.deltaTime;
  
  if (Input.GetKey(KeyCode.S))
      transform.position += -transform.forward * speed/20 * Time.deltaTime;
      
  if (Input.GetKey(KeyCode.Space))
	  transform.position += transform.up * speed/20 * Time.deltaTime;

  if (Input.GetKey(KeyCode.C))
	  transform.position += -transform.up * speed/20 * Time.deltaTime;


}