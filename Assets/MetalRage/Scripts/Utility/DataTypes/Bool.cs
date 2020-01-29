public struct Bool {
    public readonly byte Value;

    public Bool(byte value) {
        this.Value = value;
    }

    public Bool(bool value) {
        this.Value = value ? (byte)1 : (byte)0;
    }

    public static implicit operator bool(Bool bb) {
        return bb.Value != 0;
    }

    public static implicit operator Bool(bool b) {
        return new Bool(b);
    }
}
