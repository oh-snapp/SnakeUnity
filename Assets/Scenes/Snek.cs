using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snek : MonoBehaviour
{
    [SerializeField] BoardProperties boardProperties;

    [SerializeField] GameObject blockPrefab;
    [SerializeField] GameObject applePrefab;

    [SerializeField] float moveInterval;

    [SerializeField] float speedIncrement = 0.2f;
    [SerializeField] int nApplesPerSpeedIncrement = 3;
    [SerializeField] float sprintSpeedMult = 1.5f;

    [SerializeField] Vector2Int startBlockPosition;
    [SerializeField] Vector2Int startApplePosition;

    public System.Action GameOverEvent;

    List<Vector2Int> blockCoords = new();
    List<GameObject> blocks = new();

    Vector2Int appleCoord = new();
    GameObject apple = null;

    Vector2Int curMoveDirection = new();
    Vector2Int lastInputDirection = new();
    bool hasMoved = false;
    float nextMoveTime = 0;

    bool gameOver = false;

    int nApplesSinceLastSpeedIncrement = 0;
    float speed;

    void Awake()
    {
        AddBlock(startBlockPosition);
        speed = boardProperties.Speed;
    }

    void Start()
    {
        apple = Instantiate(applePrefab, transform);
        apple.transform.position = (Vector2)startApplePosition;

        appleCoord = startApplePosition;
    }

    void Update()
    {
        if (gameOver) return;
        var inputDirection = new Vector2Int(
            (int)Input.GetAxisRaw("Horizontal"),
            (int)Input.GetAxisRaw("Vertical")
        );
        if(inputDirection.x != 0 && inputDirection.y != 0)
        {
            inputDirection.y = 0;
        }
        
        if(
            inputDirection != Vector2Int.zero
            && !Mathf.Approximately(Vector2.Dot(curMoveDirection, inputDirection), -1f) // dont allow moving in opposite direction of movement
            // (instant death)
        )
        {
            lastInputDirection = inputDirection;
            if (!hasMoved)
            {
                nextMoveTime = Time.time;
            }
            hasMoved = true;
        }

        if(lastInputDirection != Vector2Int.zero)
        {
            if(Time.time > nextMoveTime)
            {
                float speedMult = 1.0f;
                if (Input.GetButton("Sprint"))
                {
                    speedMult = sprintSpeedMult;
                }
                Move(lastInputDirection);
                curMoveDirection = lastInputDirection;
                nextMoveTime += moveInterval / (speed * speedMult);
            }
        }
    }

    void AddBlock(Vector2Int position)
    {
        blockCoords.Add(position);

        var block = Instantiate(blockPrefab, transform);
        block.transform.localPosition = (Vector2)position;

        blocks.Add(block);
    }

    void Move(Vector2Int direction)
    {
        var newCoord = blockCoords[^1] + direction;
        if (!CanEnterCoordinate(newCoord))
        {
            gameOver = true;
            GameOverEvent?.Invoke();
        }
        else if(appleCoord == newCoord)
        {
            AddBlock(newCoord);
            RandomizeApple();

            nApplesSinceLastSpeedIncrement += 1;
            if(nApplesSinceLastSpeedIncrement >= nApplesPerSpeedIncrement)
            {
                speed += speedIncrement;
                nApplesPerSpeedIncrement = 0;
            }
        }
        else
        {
            // when we move, add a new head to the end of the list
            // and remove the tail
            blockCoords.Add(newCoord);
            blockCoords.RemoveAt(0);

            // instead of deleting and readding GameObjects (slow)
            // treat the tail block as the new head
            var backBlock = blocks[0];
            blocks.RemoveAt(0);
            backBlock.transform.position = (Vector2)newCoord;
            blocks.Add(backBlock);
        }
    }

    bool CanEnterCoordinate(Vector2Int coord)
    {
        return
            System.Math.Abs(coord.x) <= boardProperties.HalfExtent.x
            && System.Math.Abs(coord.y) <= boardProperties.HalfExtent.y
            && !blockCoords.Contains(coord) /* if running into self */;
    }

    void RandomizeApple()
    {
        Vector2Int newCoord;
        do
        {
            newCoord = new Vector2Int(
                Random.Range(-boardProperties.HalfExtent.x, boardProperties.HalfExtent.x),
                Random.Range(-boardProperties.HalfExtent.y, boardProperties.HalfExtent.y)
            );
        } while(newCoord == appleCoord || blockCoords.Contains(newCoord));
        appleCoord = newCoord;
        apple.transform.position = (Vector2)newCoord;
    }
}
