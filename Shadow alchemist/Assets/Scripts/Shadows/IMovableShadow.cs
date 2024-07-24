using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovableShadow
{
    public void MoveShadow(float moveSpeed, Vector2 direction);

    public void RevertMove();
}
