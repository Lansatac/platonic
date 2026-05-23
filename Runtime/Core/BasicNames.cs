#nullable enable
using System.Collections.Generic;
using UnityEngine;

namespace Platonic.Core
{
#if UNITY_EDITOR
    using UnityEditor;

    [InitializeOnLoad]
#endif
    public static class BasicNames
    {
        static BasicNames()
        {
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
        }
        
        public static readonly FieldName<IData> Data =
            Names.Register<IData>("Platonic.Core." + nameof(Data));

        public static readonly FieldName<IEnumerable<IData>> DataList =
            Names.Register<IEnumerable<IData>>("Platonic.Core." + nameof(DataList));
    }


    public static class BasicNameExtensions
    {
        public static IData Get_Data(this IData data)
        {
            return data.GetField(BasicNames.Data).Value;
        }

        public static IEnumerable<IData> Get_DataList(this IData data)
        {
            return data.GetField(BasicNames.DataList).Value;
        }
    }
}