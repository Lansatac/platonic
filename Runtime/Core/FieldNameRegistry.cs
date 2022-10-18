using System;
using System.Collections.Generic;
using System.Linq;

namespace Platonic.Core
{
    public class FieldNameRegistry
    {
        private readonly Dictionary<ulong, IFieldName> _namesByID = new();
        private readonly Dictionary<string, IFieldName> _namesByName = new();

        public FieldName<T> Register<T>(string name, ulong id = 0)
        {
            if (id == 0)
            {
                id = (ulong)name.GetHashCode();
            }

            if (_namesByID.ContainsKey(id))
            {
                throw new Exception(
                    $"Attempted to register {name}({typeof(T).Name}) with id {id}. But {_namesByID[id].Name}({_namesByID[id].FieldType.Name}) was already registered.");
            }

            var fieldName = new FieldName<T>(id, name);
            _namesByID.Add(id, fieldName);
            _namesByName.Add(name, fieldName);
            return fieldName;
        }

        public IEnumerable<IFieldName> GetAllNames()
        {
            return _namesByID.Values;
        }

        public IEnumerable<IFieldName> GetNamesOfType(Type type)
        {
            return _namesByID.Values.Where(name => name.FieldType == type);
        }

        public IFieldName GetName(ulong id)
        {
            if (!TryGetName(id, out var name))
            {
                throw new Exception($"No field with id {id} was previously registered!");
            }
            return name;
        }

        public bool TryGetName(ulong id, out IFieldName name)
        {
            return _namesByID.TryGetValue(id, out name);
        }
        
        public IFieldName GetName(string fieldName)
        {
            if (!TryGetName(fieldName, out var name))
            {
                throw new Exception($"No field with name '{fieldName}' was previously registered!");
            }
            return name;
        }
        
        public bool TryGetName(string fieldName, out IFieldName name)
        {
            return _namesByName.TryGetValue(fieldName, out name);
        }
        
        public FieldName<T> GetName<T>(ulong id)
        {
            if (!_namesByID.TryGetValue(id, out var name))
            {
                throw new Exception($"No field with id {id} was previously registered!");
            }

            if (name is not FieldName<T> typedName)
            {
                throw new Exception($"Field name {name.ID}:{name.Name} was registered as {name.FieldType.Name} but requested as {typeof(T).Name}!");
            }
            return typedName;
        }
        
        public FieldName<T> GetName<T>(string fieldName)
        {
            if (!_namesByName.TryGetValue(fieldName, out var name))
            {
                throw new Exception($"No field with name '{fieldName}' was previously registered!");
            }

            if (name is not FieldName<T> typedName)
            {
                throw new Exception($"Field name {name.ID}:{name.Name} was registered as {name.FieldType.Name} but requested as {typeof(T).Name}!");
            }
            
            return typedName;
        }
    }

    public static class Names
    {
        static Names()
        {
            Instance = new FieldNameRegistry();
        }

        public static readonly FieldNameRegistry Instance;

        public static FieldName<T> Register<T>(string name, ulong id = 0)
        {
            return Instance.Register<T>(name, id);
        }
    }
}