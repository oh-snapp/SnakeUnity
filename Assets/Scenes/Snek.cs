using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The logic behind <c>Snek</c>'s movement (<c>Snek.Move</c>) is that we have 
/// a collection of blocks that represent the snake
/// where the first represents the tail and the last
/// represents the head.
/// 
/// When we move, we want to "shift every block forward one"
/// 
/// Eg, if this is our snake (with X being the head) and we are moving up,
/// the desired change is:
///
///            X
///  xxX ->  xxx
/// xx       x
/// 
/// However, this is the same as the following sequence of steps:
/// 
///        remove old tail   add new head
///                                  X
///  xxX    ->  xxx        ->      xxx
/// xx          x                  x
/// 
/// (It is also the same as moving the 
/// 
/// The way movement works with input is that every <c>Snek.moveInterval</c> number 
/// of seconds (scaled according to speed), we check what key the player last pressed and 
/// move in that direction.
/// This could be done using a coroutine, but in this it is done by storing
/// <c>Snek.nextMoveTime</c> and comparing it with <c>Time.time</c> in <c>Snek.Update</c>
/// </summary>
public class Snek : MonoBehaviour
{
    [SerializeField] BoardProperties boardProperties;

    [SerializeField] GameObject blockPrefab;
    [SerializeField] GameObject applePrefab;

    [SerializeField] float moveInterval;
    [SerializeField] Vector2Int startBlockPosition;
    [SerializeField] Vector2Int startApplePosition;

    public System.Action GameOverEvent;

    // we used LinkedList to make it fast to remove and insert to front and back
    LinkedList<GameObject> blocks = new();
    // we use a hashset to check if a block exists quickly
    HashSet<Vector2Int> blockCoords = new();

    Vector2Int appleCoord = new();
    GameObject apple = null;

    Vector2Int curMoveDirection = new();
    Vector2Int lastInputDirection = new();
    bool hasMoved = false;
    float nextMoveTime = 0;

    bool gameOver = false;

    void Awake()
    {
        AddBlock(startBlockPosition);
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
            && !Mathf.Approximately(Vector2.Dot(curMoveDirection, inputDirection), -1f) 
            // dont allow moving in opposite direction of movement
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
                Move(lastInputDirection);
            }
        }
    }

    void AddBlock(Vector2Int position)
    {
        var block = Instantiate(blockPrefab, transform);
        block.transform.localPosition = (Vector2)position;

        blocks.AddFirst(block);

        blockCoords.Add(position);
    }

    void Move(Vector2Int direction)
    {
        var newHeadCoord = HeadCoord + direction;

        if (!CanEnterCoordinate(newHeadCoord))
        {
            gameOver = true;
            // used to notify UI
            GameOverEvent?.Invoke();
        }
        else if(appleCoord == newHeadCoord)
        {
            AddBlock(appleCoord);
            EatApple();
        }
        // regular move
        else
        {
            var tailBlock = blocks.Last.Value;
            
            // remove tail
            blocks.RemoveLast();
            blockCoords.Remove(GetBlockCoord(tailBlock));

            // change tail to head
            tailBlock.transform.position = (Vector2)newHeadCoord;

            // add tail into head position
            blocks.AddFirst(tailBlock);
            blockCoords.Add(newHeadCoord);
        }

        curMoveDirection = direction;
        nextMoveTime += moveInterval / boardProperties.Speed;
    }

    bool CanEnterCoordinate(Vector2Int coord)
    {
        return
            // hitting horizontal wall
            System.Math.Abs(coord.x) <= boardProperties.HalfExtent.x
            // hitting vertical wall
            && System.Math.Abs(coord.y) <= boardProperties.HalfExtent.y
            // running into snake
            // (could use physics, but that's overkill)
            && !blockCoords.Contains(coord);
    }

    void EatApple()
    {
        Vector2Int newCoord;
        do
        {
            // pick a random coordiante
            newCoord = new Vector2Int(
                Random.Range(-boardProperties.HalfExtent.x, boardProperties.HalfExtent.x),
                Random.Range(-boardProperties.HalfExtent.y, boardProperties.HalfExtent.y)
            );
            // ensure the new coordinate is not the same as the old apple coordinate
            // and that the new coordinate in not inside the snake
        } while(newCoord == appleCoord || blockCoords.Contains(newCoord));
        // (note that this method of picking a new coord may be naive
        // and will get really slow as the snake gets bigger)

        appleCoord = newCoord;
        apple.transform.position = (Vector2)newCoord;
    }

    Vector2Int GetBlockCoord(GameObject block)
    {
        return Vector2Int.RoundToInt(block.transform.position);
    }

    // the => means that whenever we use <c>HeadCoord</c>
    // use the value <c>GetBlockCoord(blocks.First.Value)</c> instead
    // (its called an Expression body definition for a readonly property)
    // (you can also use it for one-line member functions like <c>int Square(int x) => x * x;</c>)
    // <see href="https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/expression-bodied-members#read-only-properties" />
    Vector2Int HeadCoord => GetBlockCoord(blocks.First.Value);
}
