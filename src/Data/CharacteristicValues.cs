using System.Collections.Generic;

namespace ThirdRun.Data
{
    public class CharacteristicValues
    {
        private readonly Dictionary<Characteristic, int> _values;

        public CharacteristicValues()
        {
            _values = new Dictionary<Characteristic, int>();
        }

        public int GetValue(Characteristic characteristic)
        {
            return _values.TryGetValue(characteristic, out var value) ? value : 0;
        }

        public void SetValue(Characteristic characteristic, int value)
        {
            _values[characteristic] = value;
        }

        public void AddValue(Characteristic characteristic, int value)
        {
            _values[characteristic] = GetValue(characteristic) + value;
        }

        public void RemoveValue(Characteristic characteristic, int value)
        {
            _values[characteristic] = GetValue(characteristic) - value;
        }

        public Dictionary<Characteristic, int> GetAllValues()
        {
            return new Dictionary<Characteristic, int>(_values);
        }

        public void Clear()
        {
            _values.Clear();
        }
    }
}