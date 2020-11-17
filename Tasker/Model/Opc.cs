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
                                        (Int32)task.Id,
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
            bool success;
            string message;

            TaskResult taskResult = new TaskResult();
            using (OpcClient client = new OpcClient(endpoint))
            {
                client.Connect();
                object[] result = client.CallMethod(
                                        "ns=3;s=\"OpcUaMethodGetTaskResult\"",
                                        "ns=3;s=\"OpcUaMethodGetTaskResult\".Method"
                                        );
                success = Convert.ToBoolean(result[0]);
                message = Convert.ToString(result[1]);
                taskResult.ProductionTask_Id = Convert.ToInt32(result[2]);
                taskResult.ItemAmount = Convert.ToInt16(result[3]);
                taskResult.Interrupt = Convert.ToBoolean(result[4]);
                taskResult.RecipeNumber = Convert.ToString(result[5]);
                taskResult.MachineBrand = Convert.ToString(result[6]);
                taskResult.BandType = Convert.ToInt16(result[7]);
                taskResult.BandSpeed = Convert.ToSingle(result[8]);
                taskResult.SawDownSpeed = Convert.ToSingle(result[9]);
                taskResult.Operator = Convert.ToString(result[10]);


                taskResult.FinishDate = DateTime.Now;
            }

                return taskResult;
        }
        
    }
}
