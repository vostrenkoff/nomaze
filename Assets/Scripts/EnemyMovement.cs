/*using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Debug;

public class EnemyMovement : MonoBehaviour
{
	public NavMeshAgent agent;
	public Transform player;
	public LayerMask whatIsGround, whatIsPlayer;

	public Vector3 walkpoint;
	bool walkPointSet;
	public float walkPointRange;

	public float timeBetweenAttacks;
	bool alreadyAttacked;

	public float sightRange, attackRange;
	public bool playerInSightRange, playerInAttackRange;

	public GameObject projectile;
	private void Awake()
	{
		agent = GetComponent<NavMeshAgent > ();
	}
	private void Update()
	{
		playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
		playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

		if(!playerInAttackRange && !playerInSightRange)
		{
			Idling();
		}
		if (!playerInAttackRange && playerInSightRange)
		{
			Chase();
		}
		if (playerInAttackRange && playerInSightRange)
		{
			Attack();
		}
	}

	private void Idling()
	{
		if(!walkPointSet)
		{
			SearchWalkpoint();
		}
		if(walkPointSet)
		{
			agent.SetDestination(walkpoint);
		}
		Vector3 distanceToWalkPoint = transform.position - walkpoint;
		if(distanceToWalkPoint.magnitude < 1f)
		{
			walkPointSet = false;
		}
	}
	private void Chase()
	{
		agent.SetDestination(player.position);
	}
	private void Attack()
	{
		agent.SetDestination(transform.position);
		transform.LookAt(player);

		if(!alreadyAttacked)
		{

			//attack code
			Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
			rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
			rb.AddForce(transform.up * 8f, ForceMode.Impulse);



			alreadyAttacked = true;
			Invoke("ResetAttack", timeBetweenAttacks);
		}
	}
	private void SearchWalkpoint()
	{
		float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
		float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

		walkpoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
		if (Physics.Raycast(walkpoint, -transform.up,2f,whatIsGround))
		{
			walkPointSet = true;
		}



	}
	private void ResetAttack()
	{
		alreadyAttacked = false;
	}
}
*/
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player, Dulo;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private Rigidbody myRigidBody;

    private void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
    }
    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();


        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLenght;



        
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        
        //agent.SetDestination(transform.position);

        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
        //DrawLine(gameObject.origin, player, Color.blue);

        if (!alreadyAttacked)
        {
            
            GameObject CurrentBullet = Instantiate(projectile, Dulo.position, Dulo.rotation);
            CurrentBullet.GetComponent<BulletController>().BulletMovement(myRigidBody.velocity.magnitude);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    
}
