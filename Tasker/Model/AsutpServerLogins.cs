using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.Entity.Core;
using Tasker.ModelView;
using System.Threading;
using System.Data.SqlClient;
using System.Configuration;

namespace Tasker.Model
{    
    public class AsutpServerLogins
    {
        Thread thread;
        public AsutpServerLogins()
        {
            logins = new ObservableCollection<Login>();
            Logins = new ReadOnlyObservableCollection<Login>(logins);

            thread = new Thread(GetLoginList) { IsBackground = true };
            thread.Start();
        }

        public void GetLoginList()
        {
            using (ASUTPEntities db = new ASUTPEntities())
            {
                try
                {
                    IQueryable<Login> query = from b in db.Logins
                                              select b;
                    foreach (Login login in query)
                    {
                        logins.Add(login);
                    }
                }
                catch (Exception e)
                {
                    OutputLog.That($"Не удалось считать список операторов: {e.Message}");
                }
            }
        }

        ObservableCollection<Login> logins;
        public ReadOnlyObservableCollection<Login> Logins { get; private set; }

    }
}
