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
                                        (Int16)task.Id,
                                        (String)task.Number,
                                        (Int16)task.Position,
                                        (String)task.Item,
                                        (String)task.ItemBatch,
                                        (String)task.ItemBeginSerial,
                                        (String)task.RecipeNumber,
                                        (String)task.PipeBatch,
                                        (String)task.PipeNumber,
                                        (String)task.PipeHeat,
                                        (String)task.PipeSteel,
                                        (Int16)task.PipeDiameter,
                                        (Int16)task.PipeThickness
                                        );
            }
        }
        
    }
}
