using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using static UnityEngine.Debug;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
	[SerializeField] public float speed = 0.3f;
	[SerializeField] private Rigidbody myRigidBody;

	[SerializeField] public Text Score;

	[SerializeField] private Vector3 moveInput;
	[SerializeField] private Vector3 moveVelocity;
	[SerializeField] Vector3 pointToLook;

	[SerializeField] private Camera mainCamera;

	[SerializeField] public GameObject Bullet, planee, score, Attention, Congrats, RestartB;
	[SerializeField] public Transform Dulo;

	[SerializeField] public float TimeBetweenShots = 0;
	[SerializeField] public float shotCounter;
	[SerializeField] public float Health = 1400;
	[SerializeField] public float MaxHealth = 1400;

	[SerializeField] private bool CanFire = false;

	[SerializeField] public static int num = 0;

	[SerializeField] public Image HealthStatus;
	[SerializeField] public ParticleSystem particleDamage, particleDeath;
	[SerializeField] public Collider myCollider;

	void Start()
	{
		//Congrats.GetComponent<Animation>();
		myCollider = GetComponent<Collider>();
		myRigidBody = GetComponent<Rigidbody>();
		mainCamera = FindObjectOfType<Camera>();
		//HealthStatus = GetComponent<UnityEngine.UI.Image>();
	}
	void Update()
	{
		/*    float xDirection = Input.GetAxis("Horizontal");
			float yDirection = Input.GetAxis("Vertical");

			Vector3 moveDirection = new Vector3(xDirection, 0.0f, yDirection);

			transform.position += moveDirection * speed;
			*/
		moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
		moveVelocity = moveInput * speed;

		Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
		Plane groundPlane = new Plane(Vector3.down, 30);
		
		float rayLenght;



		if (groundPlane.Raycast(cameraRay, out rayLenght))
		{
			pointToLook = cameraRay.GetPoint(rayLenght);
			DrawLine(cameraRay.origin, pointToLook, Color.blue);
			transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
		}
		if (Input.GetMouseButtonDown(0))
		{
			CanFire = true;
		}
		if (Input.GetMouseButtonUp(0))
		{
			CanFire = false;
		}
		if(CanFire)
		{
			//Shoot();
			
			TimeBetweenShots -= Time.deltaTime;
			if (TimeBetweenShots <= 0)
			{
				TimeBetweenShots = 0.2f;
				GameObject CurrentBullet = Instantiate(Bullet, Dulo.position, Dulo.rotation);
				CurrentBullet.GetComponent<BulletController>().BulletMovement(myRigidBody.velocity.magnitude);
				
			}
			else
			{
				//TimeBetweenShots = 0.1f;
			}
			

		}
		UnityEngine.Debug.Log(TimeBetweenShots);

	}
	void FixedUpdate()
	{
		myRigidBody.velocity = moveVelocity;
		mainCamera.transform.position = myRigidBody.transform.position + new Vector3(0f, 4f, -3f);
		if(gameObject.tag == "Player")
		{
			HealthStatus.fillAmount = Health / MaxHealth;
		}
		Score.text = "" + Movement.num;
	}
	void OnTriggerStay(Collider target)
	{
		
		if (target.tag == "Bullet")
		{
			if (gameObject.tag == "Player")
			{
				particleDamage.Play();
				Health = Health - 12f;


				if (Health <= 0)
				{
					Destroy(gameObject);
					RestartB.SetActive(true);
					num = 0;
				}
			}
		}
		if (target.tag == "BulletF")
		{
			
			Health -= 5f;
			if (Health <= 0)
			{
				myCollider.enabled = false;
				gameObject.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
				particleDeath.Play();

				Invoke("DestroyEnemy", 1.5f);
			}
			
		}
		
	}

	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Wall")
		{
			
			if (gameObject.tag == "Player")
			{
				if (num >= 10)
				{

					Congrats.SetActive(true);
					Congrats.GetComponent<Animation>().Play("congrats");
					TImer.timerIsRunning = false;
					gameObject.GetComponent<Collider>().enabled = false;
				}
				else
				{
					Attention.SetActive (true);
					Invoke("TurnOff", 3f);
				}
			}
		}
	}
	public void TurnOff()
	{
		Attention.SetActive(false);
	}
	
	public void DestroyEnemy()
	{
		Destroy(gameObject);
		Vector3 pos = gameObject.transform.position;
		GameObject CurrentScore = Instantiate(score);
		CurrentScore.transform.position = new Vector3(pos.x, 30.46f, pos.z);
		
	}
	public void Shoot()
	{

		TimeBetweenShots -= Time.deltaTime * 0.00001f;
		if (TimeBetweenShots <= 0)
		{
			TimeBetweenShots = 100010000;
			GameObject CurrentBullet = Instantiate(Bullet, Dulo.position, Dulo.rotation);
			CurrentBullet.GetComponent<BulletController>().BulletMovement(myRigidBody.velocity.magnitude);
		}
		

	}
	
}
