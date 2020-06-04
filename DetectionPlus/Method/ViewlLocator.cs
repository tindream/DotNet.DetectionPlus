using System;
using System.Collections.Generic;

namespace DetectionPlus
{
    /// <summary>
    /// 单实例
    /// </summary>
    public class ViewlLocator
    {
        /// <summary>
        /// 单实例列表
        /// </summary>
        private static readonly Dictionary<string, object> instanceDic = new Dictionary<string, object>();

        public static T GetViewInstance<T>(object tag = null)
        {
            var type = typeof(T);
            var name = $"{type.Name}-{tag}";
            if (instanceDic.ContainsKey(name)) return (T)instanceDic[name];
            var obj = Activator.CreateInstance(type);
            instanceDic.Add(name, obj);
            return (T)obj;
        }
    }
}