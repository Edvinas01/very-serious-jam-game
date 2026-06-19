using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace InSun.GameCore.Utilities
{
    /// <summary>
    /// See https://github.com/dbrizov/NaughtyAttributes/blob/master/Assets/NaughtyAttributes/Scripts/Editor/Utility/ReflectionUtility.cs
    /// </summary>
    public static class ReflectionUtilities
    {
        public static bool TryGetValue<T>(object target, MemberInfo member, out T value)
        {
            value = default;

            if (target == null || member == null)
            {
                return false;
            }

            object result;

            switch (member)
            {
                case FieldInfo field:
                {
                    if (!typeof(T).IsAssignableFrom(field.FieldType))
                    {
                        return false;
                    }

                    result = field.GetValue(target);
                    break;
                }

                case PropertyInfo property:
                {
                    if (!property.CanRead)
                    {
                        return false;
                    }

                    if (!typeof(T).IsAssignableFrom(property.PropertyType))
                    {
                        return false;
                    }

                    result = property.GetValue(target);
                    break;
                }

                case MethodInfo method:
                {
                    if (method.GetParameters().Length != 0)
                    {
                        return false;
                    }

                    if (!typeof(T).IsAssignableFrom(method.ReturnType))
                    {
                        return false;
                    }

                    result = method.Invoke(target, null);
                    break;
                }

                default:
                {
                    return false;
                }
            }

            if (result == null)
            {
                return false;
            }

            value = (T)result;
            return true;
        }

        public static bool TryGetMember(object target, string memberName, out MemberInfo member)
        {
            if (target == null || string.IsNullOrWhiteSpace(memberName))
            {
                member = null;
                return false;
            }

            var types = GetSelfAndBaseTypes(target);
            for (var index = types.Count - 1; index >= 0; index--)
            {
                var members = types[index].GetMember(memberName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
                if (members.Length > 0)
                {
                    member = members[0];
                    return true;
                }
            }

            member = null;
            return false;
        }

        public static IEnumerable<FieldInfo> GetAllFields(object target, Func<FieldInfo, bool> predicate)
        {
            if (target == null)
            {
                yield break;
            }

            List<Type> types = GetSelfAndBaseTypes(target);

            for (int i = types.Count - 1; i >= 0; i--)
            {
                IEnumerable<FieldInfo> fieldInfos = types[i]
                    .GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(predicate);

                foreach (var fieldInfo in fieldInfos)
                {
                    yield return fieldInfo;
                }
            }
        }

        public static IEnumerable<PropertyInfo> GetAllProperties(object target, Func<PropertyInfo, bool> predicate)
        {
            if (target == null)
            {
                Debug.LogError("The target object is null. Check for missing scripts.");
                yield break;
            }

            List<Type> types = GetSelfAndBaseTypes(target);

            for (int i = types.Count - 1; i >= 0; i--)
            {
                IEnumerable<PropertyInfo> propertyInfos = types[i]
                    .GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(predicate);

                foreach (var propertyInfo in propertyInfos)
                {
                    yield return propertyInfo;
                }
            }
        }

        public static IEnumerable<MethodInfo> GetAllMethods(object target, Func<MethodInfo, bool> predicate)
        {
            if (target == null)
            {
                Debug.LogError("The target object is null. Check for missing scripts.");
                yield break;
            }

            List<Type> types = GetSelfAndBaseTypes(target);

            for (int i = types.Count - 1; i >= 0; i--)
            {
                IEnumerable<MethodInfo> methodInfos = types[i]
                    .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(predicate);

                foreach (var methodInfo in methodInfos)
                {
                    yield return methodInfo;
                }
            }
        }

        public static FieldInfo GetField(object target, string fieldName)
        {
            return GetAllFields(target, f => f.Name.Equals(fieldName, StringComparison.Ordinal)).FirstOrDefault();
        }

        public static PropertyInfo GetProperty(object target, string propertyName)
        {
            return GetAllProperties(target, p => p.Name.Equals(propertyName, StringComparison.Ordinal)).FirstOrDefault();
        }

        public static MethodInfo GetMethod(object target, string methodName)
        {
            return GetAllMethods(target, m => m.Name.Equals(methodName, StringComparison.Ordinal)).FirstOrDefault();
        }

        public static Type GetListElementType(Type listType)
        {
            if (listType.IsGenericType)
            {
                return listType.GetGenericArguments()[0];
            }
            else
            {
                return listType.GetElementType();
            }
        }

        /// <summary>
        ///		Get type and all base types of target, sorted as following:
        ///		<para />[target's type, base type, base's base type, ...]
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private static List<Type> GetSelfAndBaseTypes(object target)
        {
            List<Type> types = new List<Type>()
            {
                target.GetType()
            };

            while (types.Last().BaseType != null)
            {
                types.Add(types.Last().BaseType);
            }

            return types;
        }
    }
}
