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
                                        (Int16)task.PipeDiameter,
                                        (Int16)task.PipeThickness,
                                        (float)task.ItemLength,
                                        (Int16)task.ItemTargetAmount    
                                        );
            }
        }
        
    }
}
