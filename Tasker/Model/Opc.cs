using System;
using System.Configuration;
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
        public string URI { get; set; } // = "opc.tcp://192.168.10.2:4840/";
        ApplicationInstance application;
        ApplicationConfiguration configuration;
        SessionReconnectHandler reconnectHandler;
        Browser browser;
        Session session;

        private string sendTaskID = "ns=3;s=\"OpcUaMethodSendNewTask\"";
        private string sendTaskMethodID = "ns=3;s=\"OpcUaMethodSendNewTask\".Method";
        private NodeId sendTaskNodeID;
        private NodeId sendTaskMethodNodeID;

        public Opc()
        {
            URI = ConfigurationManager.AppSettings.Get("OpcUaEndpoint");
            application = new ApplicationInstance()
            {
                ApplicationName = "OPUA",
                ApplicationType = ApplicationType.Client,
                ConfigSectionName = "Client"
            };
            sendTaskNodeID = new NodeId(sendTaskID);
            sendTaskMethodNodeID = new NodeId(sendTaskMethodID);
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
                    (Int16)(task.serialLabel.StartSerial),
                    (string)task.Task.Labeling1Piece1 ?? "",
                    (string)task.Task.Labeling2Piece1 ?? "",
                    (string)task.serialLabel.EndLabel
                };

                CallMethod(sendTaskNodeID, sendTaskMethodNodeID, inputs);
            }




            //    using (OpcClient client = new OpcClient(endpoint))
            //{
            //    try
            //    {
            //        client.Connect();
            //        object[] result = client.CallMethod(
            //                                "ns=3;s=\"OpcUaMethodSendNewTask\"",
            //                                "ns=3;s=\"OpcUaMethodSendNewTask\".Method",
            //                                (Int32)(task.Task.ID),
            //                                (string)(task.Task.TaskNumber ?? "БЕЗ НОМЕРА") ,
            //                                (Int16)(task.Task.Diameter ?? 0),
            //                                (Int16)(task.Task.Thickness ?? 0),
            //                                (float)(task.Task.PieceLength1 ?? 0),
            //                                (Int16)(task.Task.PieceQuantity1 ?? 0),
            //                                (Int16)(task.serialLabel.StartSerial),
            //                                (string)task.Task.Labeling1Piece1 ?? "",
            //                                (string)task.Task.Labeling2Piece1 ?? "",
            //                                (string)task.serialLabel.EndLabel
            //                                );
            //    }
            //    catch (Exception e)
            //    {
            //        Log.logThis(e.Message);
            //        throw;
            //    }
            //}
        }

        public void CallMethod(NodeId nodeId, NodeId methodId, object[] inputArgument)
        {
            session.Call(nodeId, methodId, inputArgument);
        }
        //public void SendItemLenSet(ProductionTaskExtended task)
        //{
        //    Log.logThis($"SendTask item len set: {task.Task.TaskNumber}");

        //    ItemLenSet itemLenSet = new ItemLenSet(task);

        //    using (OpcClient client = new OpcClient(endpoint))
        //    {
        //        try
        //        {
        //            client.Connect();
        //            for (int i = 0; i <= 14 ; i++ )
        //            {                        
        //                object[] result = client.CallMethod(
        //                                        "ns=3;s=\"OpcUaMethodSendItemLenSet\"",
        //                                        "ns=3;s=\"OpcUaMethodSendItemLenSet\".Method",
        //                                        (float)itemLenSet.itemLenData[i].ItemLen,
        //                                        (Int16)itemLenSet.itemLenData[i].ItemAmount,                                                
        //                                        (string)itemLenSet.itemLenData[i].Labeling1 ?? "",
        //                                        (string)itemLenSet.itemLenData[i].Labeling2 ?? ""
        //                                        );
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Log.logThis(e.Message);
        //            throw;
        //        }
        //    }
        //}


        //public ProductionTask GetCurrentTaskResult()
        //{
        //    bool success;
        //    string message;
            
        //    ProductionTask taskResult = new ProductionTask();
        //    using (OpcClient client = new OpcClient(endpoint))
        //    {
        //        client.Connect();
        //        object[] result = client.CallMethod(
        //                                "ns=3;s=\"OpcUaMethodGetTaskResult\"",
        //                                "ns=3;s=\"OpcUaMethodGetTaskResult\".Method"
        //                                );
        //        success = Convert.ToBoolean(result[0]);
        //        message = Convert.ToString(result[1]);
        //        if (success)
        //        {                    
        //            taskResult.ID = Convert.ToInt32(result[2]);
        //            taskResult.PiceAmount = Convert.ToInt16(result[3]);
        //            //taskResult.Operator = Convert.ToString(result[4]);
        //            taskResult.BandType = Convert.ToString(result[8]);
        //            taskResult.BandBrand = Convert.ToString(result[9]);
        //            taskResult.BandSpeed = Convert.ToSingle(result[10]);
        //            taskResult.SawDownSpeed = Convert.ToSingle(result[11]);


        //            taskResult.FinishDate = DateTime.Now;
        //        }
        //        else
        //            throw new Exception(message);
        //    }

        //    return taskResult;
        //}
        
    }
}
