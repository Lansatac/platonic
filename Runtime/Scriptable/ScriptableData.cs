using System;
using System.Collections.Generic;
using System.Linq;
using Platonic.Core;
using UnityEngine;

namespace Platonic.Scriptable
{
    [CreateAssetMenu(fileName = "Data", menuName = "Data/Scriptable Data")]
    public class ScriptableData : ScriptableObject, IData
    {
        [SerializeField] List<ScriptableField> fields = new();
        public IEnumerable<IField> Fields => fields!;
        public IField GetField(IFieldName name)
        {
            foreach (var field in fields)
            {
                if (((IField)field).Name.ID == name.ID)
                {
                    return field;
                }
            }
            
            throw new Exception($"Data did not contain field with name {name.ID}:{name.Name}!");
        }

        public IField<T> GetField<T>(FieldName<T> name)
        {
            return (IField<T>)GetField((IFieldName)name);
        }
    }
}