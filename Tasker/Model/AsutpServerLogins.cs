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
        bool connectionEstablished = false;
        bool firstAttempt = true;
        public void GetLoginList()
        {
            bool loginsLoaded = false;
            while (!loginsLoaded)
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
                        loginsLoaded = true;
                        connectionEstablished = true;
                        firstAttempt = false;
                    }
                    catch (Exception e)
                    {
                        if (firstAttempt || connectionEstablished)
                        {
                            OutputLog.That($"Не удалось считать список операторов: {e.Message}");
                            connectionEstablished = false;
                            firstAttempt = false;
                        }
                    }
                }
                Thread.Sleep(20000);
            }
        }

        ObservableCollection<Login> logins;
        public ReadOnlyObservableCollection<Login> Logins { get; private set; }

    }
}
