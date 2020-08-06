using System;
using System.Configuration;
using Opc.UaFx.Client;

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
                                        "ns=3;s=\"SendNewTask\"",
                                        "ns=3;s=\"SendNewTask\".Method",
                                        (Int16)task.Id,
                                        (String)task.Number.Trim(),
                                        (String)task.PipeBatch,
                                        (String)task.ProductBatchNumber,
                                        (Int16)task.ItemAmount1);                    
            }
        }
        
    }
}
