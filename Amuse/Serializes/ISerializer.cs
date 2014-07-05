using System;

namespace Amuse.Serializes
{
    public interface ISerializer
    {
        string Serialize(object obj);
        T Deserialize<T>(string text);
        object Deserialize(string text, Type type);
    }
}
