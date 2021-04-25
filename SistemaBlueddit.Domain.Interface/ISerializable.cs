using System;

namespace SistemaBlueddit.Domain.Interface
{
    public interface ISerializable<T>
    {
        public string SerializeObejct();

        public T DeserializeObject(string objectToDeserialize);

        public string Print();
    }
}
