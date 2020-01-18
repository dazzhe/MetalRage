using System;

[Flags]
public enum ActionTag {
    None = 0,
    Any = -1,
    Movement = 1 << 0,
}
