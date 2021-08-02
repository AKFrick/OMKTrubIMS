using System;
using System.Configuration;
using Opc.UaFx.Client;
using Opc.UaFx;
using System.Windows;

namespace Tasker.Model
{
    /// <summary>
    /// Связь с ПЛК через OPC UA
    /// </summary>
    public class Opc
    {
        readonly string endpoint = ConfigurationManager.AppSettings.Get("OpcUaEndpoint");
        /// <summary>
        /// отправить в ПЛК задание
        /// </summary>
        public void SendTask(ProductionTaskExtended task)
        {
            Log.logThis($"SendTask: {task.Task.TaskNumber}");
            using (OpcClient client = new OpcClient(endpoint))
            {
                try
                {
                    client.Connect();
                    object[] result = client.CallMethod(
                                            "ns=3;s=\"OpcUaMethodSendNewTask\"",
                                            "ns=3;s=\"OpcUaMethodSendNewTask\".Method",
                                            (Int32)task.Task.ID,
                                            (string)task.Task.TaskNumber,
                                            (Int16)task.Task.Diameter,
                                            (Int16)task.Task.Thickness,
                                            (float)task.Task.PieceLength1,
                                            (Int16)task.Task.PieceQuantity1,
                                            (Int16)task.serialLabel.StartSerial,
                                            (string)task.Task.Labeling1Piece1,
                                            (string)task.Task.Labeling2Piece1,
                                            (string)task.serialLabel.EndLabel
                                            );
                }
                catch (Exception e)
                {
                    Log.logThis(e.Message);
                }
            }
        }
        public void SendItemLenSet(ProductionTaskExtended task)
        {
            Log.logThis($"SendTask item len set: {task.Task.TaskNumber}");

            ItemLenSet itemLenSet = new ItemLenSet(task);

            using (OpcClient client = new OpcClient(endpoint))
            {
                try
                {
                    client.Connect();
                    for (int i = 0; i <= 14 ; i++ )
                    {                        
                        object[] result = client.CallMethod(
                                                "ns=3;s=\"OpcUaMethodSendItemLenSet\"",
                                                "ns=3;s=\"OpcUaMethodSendItemLenSet\".Method",
                                                (float)itemLenSet.itemLenData[i].ItemLen,
                                                (Int16)itemLenSet.itemLenData[i].ItemAmount,                                                
                                                (string)itemLenSet.itemLenData[i].Labeling1,
                                                (string)itemLenSet.itemLenData[i].Labeling2
                                                );
                    }
                }
                catch (Exception e)
                {
                    Log.logThis(e.Message);
                }
            }
        }


        public ProductionTask GetCurrentTaskResult()
        {
            bool success;
            string message;
            
            ProductionTask taskResult = new ProductionTask();
            using (OpcClient client = new OpcClient(endpoint))
            {
                client.Connect();
                object[] result = client.CallMethod(
                                        "ns=3;s=\"OpcUaMethodGetTaskResult\"",
                                        "ns=3;s=\"OpcUaMethodGetTaskResult\".Method"
                                        );
                success = Convert.ToBoolean(result[0]);
                message = Convert.ToString(result[1]);
                if (success)
                {                    
                    taskResult.ID = Convert.ToInt32(result[2]);
                    taskResult.PiceAmount = Convert.ToInt16(result[3]);
                    taskResult.Operator = Convert.ToString(result[4]);

                    taskResult.FinishDate = DateTime.Now;
                }
                else
                    throw new Exception(message);
            }

            return taskResult;
        }
        
    }
}
