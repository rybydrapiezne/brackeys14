using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FogOfWarManager : MonoBehaviour
{

    [SerializeField] private int _initialDepth;
    [SerializeField] private float _moveDuration = 1.0f;
    private float _scale;
    private int _currentLevel = 0;

    private Coroutine _currentMove;

    void Start()
    {
        _scale = transform.localScale.x;
        MoveFog(_initialDepth);
    }


    public void MoveFogForward(int count)
    {
        MoveFog(_currentLevel + count);
    }

    public void MoveFog(int depth)
    {
        float targetX = GetLeftmostNodeX(depth);

        if (depth <= _currentLevel)

            if (_currentMove != null)
            {
                StopCoroutine(_currentMove);
            }

        _currentMove = StartCoroutine(ScaleFogOverTime(targetX));
        _currentLevel = depth;
    }

    private IEnumerator ScaleFogOverTime(float targetX)
    {
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = new Vector3(_scale - targetX, startScale.y, startScale.z);

        float elapsed = 0f;

        while (elapsed < _moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _moveDuration);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transform.localScale = targetScale;
        _currentMove = null;
    }


    private static float GetLeftmostNodeX(int targetLevel)
    {
        List<Node> allNodes = FindObjectsOfType<Node>(includeInactive: true).ToList();

        var filteredNodes = allNodes.Where(n => n.level == targetLevel);

        if (!filteredNodes.Any())
        {
            Debug.LogWarning("No nodes found at level " + targetLevel);
            return 0f;
        }

        Node leftmost = filteredNodes.Aggregate((minNode, nextNode) =>
            nextNode.transform.position.x < minNode.transform.position.x ? nextNode : minNode
        );


        Debug.Log("Result: " + leftmost.transform.position.x);


        return leftmost.transform.position.x;
    }


}
