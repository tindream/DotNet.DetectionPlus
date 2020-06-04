using Paway.Helper;
using Paway.Utils;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using log4net;
using System.Reflection;
using System.Threading;
using System.Collections;
using DetectionPlus.Properties;

namespace DetectionPlus
{
    public class DataService : SQLiteHelper
    {
        private const string dbName = "DetectionPlus.db";
        public static DataService Default { get; } = new DataService();
        public DataService()
        {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbName);
            base.InitConnect(file);
            if (base.InitCreate(Resources.script))
            {
            }
        }

        #region Admin.Update
        public void Update(string name, object value, DbCommand arg = null)
        {
            ExecuteTransaction(cmd =>
            {
                //string find = "Name = @name";
                //List<AdminBaseInfo> list = Find<AdminBaseInfo>(find, new { name }, cmd);
                //if (list.Count == 0)
                //{
                //    AdminBaseInfo info = new AdminBaseInfo() { Name = name, Value = value.ToString() };
                //    Insert(info, cmd);
                //}
                //else
                //{
                //    list[0].Value = value.ToString();
                //    Update(list[0], cmd);
                //}
            }, arg);
        }

        #endregion
    }
}
