using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperClass
{
    public static bool CheckIfObjectIsBehind(Vector2 gameObjectPos,Vector2 gameObjectToCheckPos,GlobalEnums.HorizontalDirections gameObjectLookingDirection)
    {
        // sub result - <0 means gameObjectToCheckPos is on right, else its on left. Mult result - <0 gameObjectToCheckPos is in front, else gameObjectToCheckPos is behind
        if ((gameObjectPos.x - gameObjectToCheckPos.x) * ((int)gameObjectLookingDirection) <= 0) return false;
        else return true;
    }
}
