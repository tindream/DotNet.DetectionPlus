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
using System.Reflection;
using System.Threading;
using System.Collections;
using DetectionPlus.Sign.Properties;

namespace DetectionPlus.Sign
{
    public class DataService : SQLiteHelper
    {
        private const string dbName = "DetectionSign.db";
        public static DataService Default { get; } = new DataService();
        public DataService()
        {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbName);
            base.InitConnect(file);
            if (base.InitCreate(Resources.script))
            {
            }
        }

        #region 初始化
        public void Init()
        {
            new Action(() =>
            {
                ExecuteCommand(cmd => { });
            }).BeginInvoke(null, null);
        }
        public void Load()
        {
            base.ExecuteCommand(cmd =>
            {
                Config.Admin = FindAdmin(cmd);
            });
        }
        private AdminInfo FindAdmin(DbCommand cmd = null)
        {
            List<AdminBaseInfo> temp = Find<AdminBaseInfo>(cmd);
            List<IInfo> list = new List<IInfo>();
            list.AddRange(temp);
            return TMethod.Conversion<AdminInfo, IInfo>(list);
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
                    list[0].Value = value.ToStrs();
                    list[0].DateTime = DateTime.Now;
                    Update(list[0], cmd);
                }
            }, arg);
        }

        #endregion
    }
}
