public struct BlittableBool {
    public readonly byte Value;

    public BlittableBool(byte value) {
        this.Value = value;
    }

    public BlittableBool(bool value) {
        this.Value = value ? (byte)1 : (byte)0;
    }

    public static implicit operator bool(BlittableBool bb) {
        return bb.Value != 0;
    }

    public static implicit operator BlittableBool(bool b) {
        return new BlittableBool(b);
    }
}
