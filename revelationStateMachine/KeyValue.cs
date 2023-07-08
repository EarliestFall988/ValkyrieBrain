

/// <summary>
/// A generic key value pair
/// </summary>
/// <typeparam name="T"></typeparam>
public class KeyValue<T> where T : IComparable<T>, IEquatable<T>
{

    private string _key = "";

    public string Key
    {
        get
        {
            return _key;
        }

        set
        {
            if (_key == string.Empty)
                _key = value;
        }
    }

    public T Value { get; set; }

    public KeyValue(string key, T value)
    {
        Key = key;
        Value = value;
    }


    public void UpdateValue(T value)
    {
        Console.WriteLine($"Updating {Key} to {value}");
        Value = value;
    }

    public int Compare(KeyValue<T> other)
    {
        return Value.CompareTo(other.Value);
    }

    public bool Equals(KeyValue<T> other)
    {
        return Value.Equals(other.Value);
    }

    public override string ToString()
    {
        return $"{Key} -> {Value}";
    }
}