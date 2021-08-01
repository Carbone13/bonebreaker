﻿using Godot;

public struct sfloat2
{
    public sfloat X { get; set; }
    public sfloat Y { get; set; }

    public override string ToString () => "x: " + X + "; y: " + Y;

    #region Constants

    public static readonly sfloat2 Zero = new sfloat2(sfloat.Zero, sfloat.Zero);
    public static readonly sfloat2 One = new sfloat2(sfloat.One, sfloat.One);
    
    public static readonly sfloat2 Right = new sfloat2(sfloat.One, sfloat.Zero);
    public static readonly sfloat2 Left = new sfloat2(sfloat.MinusOne, sfloat.Zero);
    public static readonly sfloat2 Up = new sfloat2(sfloat.Zero, sfloat.One);
    public static readonly sfloat2 Down = new sfloat2(sfloat.Zero, sfloat.MinusOne);
    
    #endregion
    
    #region Basic Constructor
    
    public sfloat2 (sfloat _x, sfloat _y)
    {
        X = _x;
        Y = _y;
    }

    public sfloat2 (float _x, float _y)
    {
        X = (sfloat) _x;
        Y = (sfloat) _y;
    }
    
    public sfloat2 (int _x, int _y)
    {
        X = (sfloat) _x;
        Y = (sfloat) _y;
    }
    
    public sfloat2 (Godot.Vector2 vector)
    {
        X = (sfloat) vector.x;
        Y = (sfloat) vector.y;
    }
    
    #endregion

    #region Static Constructor
    
    public static sfloat2 FromRaw (uint _x, uint _y)
    {
        return new sfloat2
        (
            sfloat.FromRaw(_x),
            sfloat.FromRaw(_y)
        );
    }
    
    #endregion
    
    #region Serialization
    // So Godot don't really like sending custom types, but we can send string, so these functions
    // allow you the serialize and deserialize this struct as string
    public string Serialize ()
    {
        return X.RawValue + ";" + Y.RawValue;
    }

    public static sfloat2 FromString (string input)
    {
        return new sfloat2
        (
            sfloat.FromRaw( uint.Parse(input.Split(';')[0]) ),
            sfloat.FromRaw( uint.Parse(input.Split(';')[1]) )
        );
    }
    
    #endregion
   
    #region Setter
    public void SetX (sfloat _x)
    {
        X = _x;
    }

    public void SetY (sfloat _y)
    {
        Y = _y;
    }
    
    public void SetX (int _x)
    {
        X = (sfloat)_x;
    }

    public void SetY (int _y)
    {
        Y = (sfloat)_y;
    }
    
    public void SetX (float _x)
    {
        X = (sfloat)_x;
    }

    public void SetY (float _y)
    {
        Y = (sfloat) _y;
    }

    #endregion
    
    #region Functions

    public sfloat2 Clone ()
    {
        return new sfloat2(X, Y);
    }
    
    public sfloat SquaredLength => X * X + Y * Y;

    public sfloat Length
    {
        get => libm.sqrtf(SquaredLength);
        set
        {
            sfloat eps = sfloat.Epsilon;
            sfloat angle = libm.atan2f(Y, X);

            X = libm.cosf(angle) * value;
            Y = libm.sinf(angle) * value;

            if (sfloat.Abs(X) < eps) X = sfloat.Zero;
            if (sfloat.Abs(Y) < eps) Y = sfloat.Zero;
        }
    }
    
    public sfloat2 normalized => new sfloat2(X / Length, Y / Length);

    public sfloat2 Normalize ()
    {
        var len = Length;
        
        if (len == sfloat.Zero)
        {
            X = sfloat.One;
            return this;
        }

        X /= len;
        Y /= len;

        return this;
    }

    public sfloat2 Truncate (sfloat max)
    {
        Length = (sfloat) sfloat.Min(max, Length);
        return this;
    }

    public sfloat2 Invert ()
    {
        X = -X;
        Y = -Y;

        return this;
    }

    public sfloat Dot (sfloat2 other)
    {
        return X * other.X + Y * other.Y;
    }
    
    public sfloat Cross (sfloat2 other)
    {
        return X * other.X - Y * other.Y;
    }

    public sfloat2 Add (sfloat2 other)
    {
        X += other.X;
        Y += other.Y;

        return this;
    }
    
    public sfloat2 Substract (sfloat2 other)
    {
        X -= other.X;
        Y -= other.Y;

        return this;
    }

    public sfloat2 Sign ()
    {
        return new sfloat2(X.Sign(), Y.Sign());
    }
    
    #endregion

    #region Operators

    public static implicit operator Vector2 (sfloat2 input)
    {
        return new Vector2((float)input.X, (float)input.Y);
    }

    public static implicit operator sfloat2 (Vector2 input)
    {
        return new sfloat2(input.x, input.y);
    }
    
    public static sfloat2 operator *(sfloat2 input, sfloat factor)
    {
        return new sfloat2(input.X * factor, input.Y * factor);
    }
    
    public static sfloat2 operator *(sfloat2 input, sfloat2 factor)
    {
        return new sfloat2(input.X * factor.X, input.Y * factor.Y);
    }
    
    public static sfloat2 operator /(sfloat2 input, sfloat factor)
    {
        return new sfloat2(input.X / factor, input.Y / factor);
    }
    
    public static sfloat2 operator /(sfloat2 input, sfloat2 factor)
    {
        return new sfloat2(input.X / factor.X, input.Y / factor.Y);
    }

    public static sfloat2 operator - (sfloat2 one, sfloat2 two)
    {
        return new sfloat2(one.X - two.X, one.Y - two.Y);
    }
    
    public static sfloat2 operator + (sfloat2 one, sfloat2 two)
    {
        return new sfloat2(one.X + two.X, one.Y + two.Y);
    }

    public static sfloat2 operator -(sfloat2 input)
    {
        return new sfloat2(-input.X, -input.Y);
    }

    #endregion
}
