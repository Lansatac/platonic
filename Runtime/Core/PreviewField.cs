#nullable enable
using System;
using System.Linq;
using Platonic.Version;
using UnityEngine;

namespace Platonic.Core
{
    
    [Serializable]
    public class PreviewField : IField<int>, IField<string>, IField<bool>, IField<float>, ISerializationCallbackReceiver
    {
        private ulong _verison = Versions.Initial;
        public ulong Version => _verison;

        [SerializeField] private string _fieldName = string.Empty;

        public IFieldName Name
        {
            get
            {
                if (Names.Instance.TryGetName(_fieldName, out var name))
                {
                    return name;
                }
                return Names.Instance.GetAllNames().First();
            }
        }

        [SerializeField] private string _stringValue = string.Empty;
        [SerializeField] private int _intValue = 0;
        [SerializeField] private float _floatValue = 0f;
        [SerializeField] private bool _boolValue = false;
        
        public object Value
        {
            get
            {
                switch (Name.FieldType)
                {
                    case not null when Name.FieldType == typeof(int):
                        return _intValue;
                    case not null when Name.FieldType == typeof(float):
                        return _floatValue;
                    case not null when Name.FieldType == typeof(string):
                        return _stringValue;
                    case not null when Name.FieldType == typeof(bool):
                        return _boolValue;
                    default:
                        return "Cannot be previewed.";
                }
            }
        }

        public bool IsBasicType
        {
            get
            {
                switch (Name.FieldType)
                {
                    case not null when Name.FieldType == typeof(int):
                    case not null when Name.FieldType == typeof(float):
                    case not null when Name.FieldType == typeof(string):
                    case not null when Name.FieldType == typeof(bool):
                        return true;
                    default:
                        return false;
                }
            }
        }
        
        int IField<int>.Value => _intValue;
        IFieldName<int> IField<int>.Name => (IFieldName<int>)Name;
        
        float IField<float>.Value => _floatValue;
        IFieldName<float> IField<float>.Name => (IFieldName<float>)Name;

        bool IField<bool>.Value => _boolValue;
        IFieldName<bool> IField<bool>.Name => (IFieldName<bool>)Name;

        string IField<string>.Value => _stringValue;
        IFieldName<string> IField<string>.Name => (IFieldName<string>)Name;
        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            _verison += 1;
        }
    }
}