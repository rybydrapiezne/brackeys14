using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerCaravanController : MonoBehaviour
{
    public bool isMoving=false;
    
    [SerializeField] private GameObject nomadPrefab;
    [SerializeField] private GameObject camelPrefab;
    [SerializeField] private float xOffset = 2f;
    [SerializeField] private float yOffset = 2f;
    [SerializeField] private float minSpawnDistance = 0.5f;
    [SerializeField] private int maxSpawnAttempts = 10;
    [SerializeField] private float moveSpeed = 50f;
    private int _currentCaravanSize = 0;
    private int _numberOfCamels = 0;
    private List<GameObject> _caravan;

    private void Start()
    {
        _caravan = new List<GameObject>();
        ResourceSystem.OnResourceChanged += CaravanSizeChanged;
        for (int i = 0; i < ResourceSystem.getResource(ResourceSystem.ResourceType.People); i++)
        {
            InstantiateNomad();
        }
    }

    private void CaravanSizeChanged(object sender, OnResourceChangedArgs e)
    {
        if (e.Resource == ResourceSystem.ResourceType.People && e.AmountChanged > 0)
        {
            InstantiateNomad();
        }
        else if (e.Resource == ResourceSystem.ResourceType.People && e.AmountChanged < 0)
        {
            initiateDeath(e.AmountChanged);
        }
    }

    private void InstantiateNomad()
    {
        Vector3 spawnPos = GetValidSpawnPosition();
        GameObject character = Instantiate(nomadPrefab, spawnPos, Quaternion.identity);
        character.transform.parent = transform;
        _caravan.Add(character);
        _currentCaravanSize++;
        
        if (_currentCaravanSize % 5 == 0)
        {
            spawnPos = GetValidSpawnPosition();
            GameObject camel = Instantiate(camelPrefab, spawnPos, Quaternion.identity);
            camel.transform.parent = transform;
            _caravan.Add(camel);
            _numberOfCamels++;
        }
    }

    private Vector3 GetValidSpawnPosition()
    {
        int attempts = 0;
        Vector3 spawnPos;

        do
        {
            spawnPos = new Vector3(
                transform.position.x + Random.Range(-xOffset, xOffset),
                transform.position.y + Random.Range(-yOffset, yOffset),
                transform.position.z
            );
            attempts++;

            if (IsPositionValid(spawnPos) || attempts >= maxSpawnAttempts)
            {
                return spawnPos;
            }
        } while (attempts < maxSpawnAttempts);
        
        Debug.LogWarning("Could not find valid spawn position after max attempts.");
        return spawnPos;
    }

    private bool IsPositionValid(Vector3 position)
    {
        foreach (GameObject member in _caravan)
        {
            if ( Vector3.Distance(position, member.transform.position) < minSpawnDistance)
            {
                return false;
            }
        }
        return true;
    }

    public void startCaravanMovement(Vector3 destPoint)
    {
        StartCoroutine(moveCaravan(destPoint));
    }

    public void initiateDeath(int amount)
    {
        int removed = 0;
        List<GameObject> deadNomads = new List<GameObject>();
        foreach (GameObject member in _caravan)
        {
            if (member.CompareTag("Nomad"))
            {
                deadNomads.Add(member);
                removed--;
                if (removed <= amount)
                    break;
            }
        }

        for (int i = 0; i < deadNomads.Count; i++)
        {
            _caravan.Remove(deadNomads[i]);
            StartCoroutine(killMember(deadNomads[i]));
        }

        if (removed == 0)
            Debug.LogWarning("Could not find Nomad");
        
        
    }

    private IEnumerator killMember(GameObject member)
    {
        member.transform.parent = null;
        member.GetComponent<Animator>().SetBool("Dying", true);
        _currentCaravanSize--;
        if (_currentCaravanSize % 5 != 0 && _currentCaravanSize/5!=_numberOfCamels)
        {
            StartCoroutine(killCamel());
        }
        while (member.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime <= 3.2)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        Destroy(member);
    }

    private IEnumerator killCamel()
    {
        foreach (GameObject member in _caravan)
        {
            if (member.CompareTag("Camel"))
            {
                member.transform.parent = null;
                member.GetComponent<Animator>().SetBool("Dying", true);
                _numberOfCamels--;
                while (member.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime <= 2.2)
                {
                    yield return null;
                }
                _caravan.Remove(member);
                yield return new WaitForSeconds(0.5f);
                Destroy(member);
                break;
            }
        }
    }


    private IEnumerator moveCaravan(Vector3 destination)
    { 
        isMoving = true;
        foreach (GameObject member in _caravan)
        {
            member.GetComponent<Animator>().SetBool("Move",true);
        }
        while (Vector3.Distance(transform.position, destination) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                destination,
                moveSpeed * Time.deltaTime
            );
            yield return null; 
        }
        
        transform.position = destination;
        foreach (GameObject member in _caravan)
        {
            member.GetComponent<Animator>().SetBool("Move",false);
        }
        isMoving = false;
    }
}