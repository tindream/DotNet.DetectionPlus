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
using DetectionPlus.Win.Properties;

namespace DetectionPlus.Win
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

        #region Init
        public void Init()
        {
            new Action(() =>
            {
                DataService.Default.ExecuteCommand(cmd => { });
            }).BeginInvoke(null, null);
        }

        #endregion

        #region Admin.Update
        public void Update(string name, DbCommand arg = null)
        {
            var value = Config.Admin.GetValue(name);
            base.ExecuteCommand(cmd =>
            {
                string find = string.Format("Name = '{0}'", name);
                List<AdminBaseInfo> list = Find<AdminBaseInfo>(find, cmd);
                if (list.Count == 0)
                {
                    AdminBaseInfo info = new AdminBaseInfo() { Name = name, Value = value.ToStrs(), DateTime = DateTime.Now };
                    Insert(info, cmd);
                }
                else
                {
                    list[0].Value = value.ToString();
                    list[0].DateTime = DateTime.Now;
                    Update(list[0], cmd);
                }
            }, arg);
        }

        #endregion
    }
}
