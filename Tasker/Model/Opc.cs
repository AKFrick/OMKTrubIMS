using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

namespace Tasker.Model
{
    /// <summary>
    /// Связь с ПЛК через OPC UA
    /// </summary>
    public class Opc
    {

        readonly bool multilen = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("multilen"));
        public string URI { get; set; } // = "opc.tcp://192.168.10.2:4840/";
        ApplicationInstance application;
        ApplicationConfiguration configuration;
        SessionReconnectHandler reconnectHandler;
        Browser browser;
        Session session;

        private string sendTaskID = "ns=3;s=\"OpcUaMethodSendNewTask\"";
        private string sendTaskMethodID = "ns=3;s=\"OpcUaMethodSendNewTask\".Method";

        private string getTaskResultID = "ns=3;s=\"OpcUaMethodGetTaskResult\"";
        private string getTaskResultMethodID = "ns=3;s=\"OpcUaMethodGetTaskResult\".Method";

        private string sendItemLenID = "ns=3;s=\"OpcUaMethodSendItemLenSet\"";
        private string sendItemLenMethodID = "ns=3;s=\"OpcUaMethodSendItemLenSet\".Method";

        public Opc()
        {
            URI = ConfigurationManager.AppSettings.Get("OpcUaEndpoint");
            application = new ApplicationInstance()
            {
                ApplicationName = "OPUA",
                ApplicationType = ApplicationType.Client,
                ConfigSectionName = "Client"
            };
        }

        public async void SendTask(ProductionTaskExtended task)
        {
                OutputLog.That($"Отправляем задание {task.Task.TaskNumber}");
                configuration = await application.LoadApplicationConfiguration(false);
                var selectedEndpoint = CoreClientUtils.SelectEndpoint(URI, false, 15000);
                var endpointConfiguration = EndpointConfiguration.Create(configuration);
                var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);

            using (session = await Session.Create(configuration,
                endpoint,
                false,
                "OPC UA Client",
                60000,
                new UserIdentity(new AnonymousIdentityToken()),
                null))
            {
                object[] inputs = new object[10]
                {
                    (Int32)(task.Task.ID),
                    (string)(task.Task.TaskNumber ?? "БЕЗ НОМЕРА"),
                    (Int16)(task.Task.Diameter ?? 0),
                    (Int16)(task.Task.Thickness ?? 0),
                    (float)(task.Task.PieceLength1 ?? 0),
                    (Int16)(task.Task.PieceQuantity1 ?? 0),
                    (Int32)(task.serialLabel.StartSerial),
                    (string)task.Task.Labeling1Piece1 ?? "",
                    (string)task.Task.Labeling2Piece1 ?? "",
                    (string)task.serialLabel.EndLabel
                };

                IList<object> result = session.Call(new NodeId(sendTaskID), new NodeId(sendTaskMethodID), inputs);
                OutputLog.That($"Задание отправлено: {result[0]} {result[1]}");

                if (multilen)
                {
                    ItemLenSet itemLenSet = new ItemLenSet(task);
                    for (int i = 0; i <= 14; i++)
                    {
                        object[] inputsLen = new object[4]
                        {
                        (float)itemLenSet.itemLenData[i].ItemLen,
                        (Int16)itemLenSet.itemLenData[i].ItemAmount,
                        (string)itemLenSet.itemLenData[i].Labeling1 ?? "",
                        (string)itemLenSet.itemLenData[i].Labeling2 ?? ""
                        };

                        session.Call(new NodeId(sendItemLenID), new NodeId(sendItemLenMethodID), inputsLen);                        
                    }
                    OutputLog.That($"Массив длин отправлен");
                }
            }            
        }

        public async System.Threading.Tasks.Task<ProductionTask> GetCurrentTaskResult()
        {
            bool success;
            string message;
            configuration = await application.LoadApplicationConfiguration(false);
            var selectedEndpoint = CoreClientUtils.SelectEndpoint(URI, false, 15000);
            var endpointConfiguration = EndpointConfiguration.Create(configuration);
            var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);

            using (session = await Session.Create(configuration,
                endpoint,
                false,
                "OPC UA Client",
                60000,
                new UserIdentity(new AnonymousIdentityToken()),
                null))
            {
                ProductionTask taskResult = new ProductionTask();

                IList<object> result = session.Call(new NodeId(getTaskResultID), new NodeId(getTaskResultMethodID));                                
               
                success = Convert.ToBoolean(result[0]);
                message = Convert.ToString(result[1]);

                if (success)
                {
                    taskResult.ID = Convert.ToInt32(result[2]);
                    taskResult.PiceAmount = Convert.ToInt16(result[3]);
                    taskResult.BandType = Convert.ToString(result[8]);
                    taskResult.BandBrand = Convert.ToString(result[9]);
                    taskResult.BandSpeed = Convert.ToSingle(result[10]);
                    taskResult.SawDownSpeed = Convert.ToSingle(result[11]);

                    taskResult.Diameter = Convert.ToInt32(result[12]);
                    taskResult.Thickness = Convert.ToInt32(result[13]);
                    taskResult.PieceLength1 = Convert.ToInt32(result[14]);
                    if (taskResult.ID != 0)
                    {
                        OutputLog.That($"Считано загруженное в панель ID: {taskResult.ID} Диаметр: {taskResult.Diameter} длина детали: {taskResult.PieceLength1} отрезано: {taskResult.PiceAmount} ");

                    }
                    else
                        OutputLog.That($"Считано задание созданное на панели: Диаметр: {taskResult.Diameter} длина детали: {taskResult.PieceLength1} отрезано: {taskResult.PiceAmount} ");

                    taskResult.FinishDate = DateTime.Now;
                }
                else
                    throw new TaskNotFoundException();                
                return taskResult;
            }
        }
        
    }
    public class TaskNotFoundException : Exception
    {           
    }
}
