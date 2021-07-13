
using System;
using Godot;

public struct int2
{
    public int X;
    public int Y;

    public static readonly int2 Zero = new int2(0, 0);
    public int2 (int x, int y)
    {
        X = x;
        Y = y;
    }

    public int2 (float x, float y)
    {
        X = (int) x;
        Y = (int) y;
    }

    public int2 (sfloat2 vector)
    {
        X = vector.X.Sign();
        Y = vector.Y.Sign();
    }
    
    public static implicit operator Vector2 (int2 input)
    {
        return new Vector2(input.X, input.Y);
    }
    
    public static implicit operator int2 (Vector2 input)
    {
        return new int2(input.x, input.y);
    }
    
    public static int2 operator+ (int2 a, int2 b)
    {
        return new int2(a.X + b.X, a.Y + b.Y);
    }
    
    public static int2 operator- (int2 a, int2 b)
    {
        return new int2(a.X - b.X, a.Y - b.Y);
    }

    public int2 Sign ()
    {
        return new int2(Math.Sign(X), Math.Sign(Y));
    }

    public override string ToString ()
    {
        return "x: " + X + " y: " + Y;
    }
}
