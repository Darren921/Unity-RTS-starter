using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Monster : MonoBehaviour
{
    
    //3 states: Patrolling, Moving, Attacking
    //Moving overrides patrolling and attacking.
    
    private Animator myAnimator;
    private NavMeshAgent ai;

    private const int IdleAnims = 2;

    [SerializeField] private float maxHealth;
    private float health;
    private Coroutine stateRoutine;
    [SerializeField] private Rigidbody fireBall;
    [SerializeField] private Transform firePoint;
    private Vector3 lookAtPoint;
    [SerializeField] private float boredTimer = 4;

    private EAiState AiState = EAiState.Idle;
    private WaitForSeconds idleTimer;
    [Flags]
    private enum EAiState 
    { 
     Idle = 1,
     Wander = 2,
     ConmmandMove = 4,
     CommandAttack = 8
    }
    private void Start()
    {
        myAnimator = GetComponent<Animator>();
        ai = GetComponent<NavMeshAgent>();
        health = maxHealth;
        idleTimer = new WaitForSeconds(boredTimer);
        EnterIdle();
    }
    private IEnumerator Idle()
    {
        yield return idleTimer;
        if (AiState == EAiState.Idle)
        {
            RaycastHit hit;

            bool findLoc;
            do {
                Vector3 randomSphere = Random.insideUnitCircle * 10;
                randomSphere.z = randomSphere.y;
                randomSphere.y = 1000;
                findLoc = Physics.Raycast(randomSphere, -Vector3.up, out hit, 2000, StaticUtilities.GroundLayer);
            } while (! findLoc);
           


            MoveToTarget(hit.point);
            AiState = EAiState.Wander;
        }
    }

    private void EnterIdle()
    {
        AiState = EAiState.Idle;
        StopRotating();

        stateRoutine = StartCoroutine(Idle());
    }

    private void Update()
    {
        Vector3 velocity = transform.InverseTransformVector(ai.velocity);
        
        myAnimator.SetFloat(StaticUtilities.XSpeedAnimId, velocity.x);
        myAnimator.SetFloat(StaticUtilities.YSpeedAnimId, velocity.z);

        if ((AiState & (EAiState.Idle | EAiState.CommandAttack)) == 0 && ai.remainingDistance <= ai.stoppingDistance)
        {
            EnterIdle();
        }
    }

    public void MoveToTarget(Vector3 hitInfoPoint)
    {
        AiState = EAiState.ConmmandMove;
        ai.SetDestination(hitInfoPoint);
        ai.isStopped = false;
    }

    public void ChangeIdleState()
    {
        int rngIndex = Random.Range(0, 2);
        myAnimator.SetFloat(StaticUtilities.IdleAnimId, rngIndex);
    }

    public  void TryAttack(RaycastHit hit)
    {
        ai.isStopped = true;
        AiState = EAiState.CommandAttack;
        StopRotating();
        lookAtPoint = (hit.point - transform.position).normalized;
        //rotate to face then attack
        stateRoutine = StartCoroutine( RotateToTarget());

    }

    private  IEnumerator RotateToTarget()
    {
        myAnimator.SetBool(StaticUtilities.isTurnAnimId, true);
        float angle;
        do
        {
          angle = Vector3.Dot(transform.right, lookAtPoint);
            myAnimator.SetFloat(StaticUtilities.TurnAnimId, angle);
            yield return null;
        } while (Math.Abs(angle) >= 0.01f);
        myAnimator.SetBool(StaticUtilities.isTurnAnimId, false);
        stateRoutine = null;
        Attack();
    }

    private void StopRotating()
    {
        if (stateRoutine != null) StopCoroutine(stateRoutine);
        myAnimator.SetBool(StaticUtilities.isTurnAnimId, false);

    }
    private void Attack()
    {
        myAnimator.SetTrigger(StaticUtilities.AttackAnimId);
    }

    public void SpawnFireBall()
    {
       Rigidbody projectile = Instantiate(fireBall, firePoint.position, firePoint.rotation);
        projectile.AddForce (lookAtPoint * 100, ForceMode.Impulse);
        EnterIdle();

    }
}
