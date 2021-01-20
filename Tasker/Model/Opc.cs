using System;
using System.Configuration;
using Opc.UaFx.Client;
using Opc.UaFx;

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
        public void SendTask(ProductionTask task)
        {            
            using (OpcClient client = new OpcClient(endpoint))
            {                
                client.Connect();
                object[] result = client.CallMethod(
                                        "ns=3;s=\"OpcUaMethodSendNewTask\"",
                                        "ns=3;s=\"OpcUaMethodSendNewTask\".Method",
                                        (Int32)task.ID,
                                        (string)task.TaskNumber,
                                        (Int16)task.Diameter,
                                        (Int16)task.Thickness,
                                        (float)task.PieceLength1,
                                        (Int16)task.PieceQuantity1,
                                        (string)task.StartSerialNumber,
                                        (string)task.Labeling1Piece1,
                                        (string)task.Labeling2Piece1
                                        );
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
                taskResult.ID = Convert.ToInt32(result[2]);
                taskResult.PieceQuantity = Convert.ToInt16(result[3]);
                taskResult.Operator = Convert.ToString(result[10]);

                //taskResult.StartDate = 
                //taskResult.FinishDate = 
            }

                return taskResult;
        }
        
    }
}
