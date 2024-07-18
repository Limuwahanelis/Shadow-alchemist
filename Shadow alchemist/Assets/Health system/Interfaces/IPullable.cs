using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPullable
{
    public void Pull(Vector2 direction, float value);
}
