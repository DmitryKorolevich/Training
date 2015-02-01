using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VitalChoice.Validation.Models
{
    [Serializable]
    public sealed class LinkSettings: IDictionary<string, LinkSetting>
    {
        [NonSerialized]
        private readonly Dictionary<string, LinkSetting> _settings;

        public LinkSettings()
        {
            _settings = new Dictionary<string, LinkSetting>();
        }

        public IEnumerator<KeyValuePair<string, LinkSetting>> GetEnumerator()
        {
            return _settings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _settings.GetEnumerator();
        }

        public void Add(KeyValuePair<string, LinkSetting> item)
        {
            _settings.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _settings.Clear();
        }

        public bool Contains(KeyValuePair<string, LinkSetting> item)
        {
            return _settings.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, LinkSetting>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, LinkSetting>>)_settings).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, LinkSetting> item)
        {
            return _settings.Remove(item.Key);
        }

        public int Count
        {
            get { return _settings.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool ContainsKey(string key)
        {
            return _settings.ContainsKey(key);
        }

        public void Add(string key, LinkSetting value)
        {
            _settings.Add(key, value);
        }

        public bool Remove(string key)
        {
            return _settings.Remove(key);
        }

        public bool TryGetValue(string key, out LinkSetting value)
        {
            return _settings.TryGetValue(key, out value);
        }

        public LinkSetting this[string key]
        {
            get { return _settings[key]; }
            set { _settings[key] = value; }
        }

        public ICollection<string> Keys
        {
            get { return _settings.Keys; }
        }

        public ICollection<LinkSetting> Values
        {
            get { return _settings.Values; }
        }
    }
}