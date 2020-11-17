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
                                        (string)task.Number,
                                        (Int16)task.PipeDiameter,
                                        (Int16)task.PipeThickness,
                                        (float)task.ItemLength,
                                        (Int16)task.ItemTargetAmount    
                                        );
            }
        }
        public TaskResult GetCurrentTaskResult()
        {
            TaskResult taskResult = new TaskResult();
            using (OpcClient client = new OpcClient(endpoint))
            {
                client.Connect();
                object[] result = client.CallMethod(
                                        "ns=3;s=\"OpcUaMethodSendNewTask\"",
                                        "ns=3;s=\"OpcUaMethodSendNewTask\".Method"
                                        );
                taskResult.Id = Convert.ToInt16(result[0]);
                taskResult.ItemAmount = Convert.ToInt16(result[1]);
                taskResult.Interrupt = Convert.ToBoolean(result[2]);
                taskResult.RecipeNumber = Convert.ToString(result[3]);
                taskResult.MachineBrand = Convert.ToString(result[4]);
                taskResult.BandType = Convert.ToString(result[5]);
                taskResult.BandSpeed = Convert.ToSingle(result[6]);
                taskResult.SawDownSpeed = Convert.ToSingle(result[7]);
                taskResult.Operator = Convert.ToString(result[8]);


                taskResult.FinishDate = DateTime.Now;
            }

                return taskResult;
        }
        
    }
}
