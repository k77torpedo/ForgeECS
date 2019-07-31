[System.Serializable]
public struct BlittableBool {
    //Fields
    public byte boolValue;


    //Functions
    public BlittableBool (bool value) {
        boolValue = (byte)(value ? 1 : 0);
    }

    public static implicit operator bool (BlittableBool value) {
        return value.boolValue == 1;
    }

    public static implicit operator BlittableBool (bool value) {
        return new BlittableBool(value);
    }

    public override string ToString () {
        if (boolValue == 1)
            return "true";

        return "false";
    }
}