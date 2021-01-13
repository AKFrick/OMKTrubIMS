using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasker.Model
{
    public class ProductTaskExtended : ProductionTask
    {
        public ProductTaskExtended(Task task)
        {
            ID = task.ID;
            TaskNumber = task.TaskNumber;
            TaskPosition = task.TaskPosition;
            Product = task.Product;
            ProductBatchNumber = task.ProductBatchNumber;
            StartSerialNumber = task.StartSerialNumber;
            ProductsAmount = task.ProductsAmount;
            PipeBatchNumber = task.PipeBatchNumber;
            PipeNumber = task.PipeNumber;
            Heat = task.Heat;
            SteelType = task.SteelType;
            Diameter = task.Diameter;
            Thickness = task.Thickness;
            PieceLength = task.PieceLength;
            PieceQuantity = task.PieceQuantity;
            CreationDate = task.CreationDate;
            StartDate = task.StartDate;
            FinishDate = task.FinishDate;
            Source = task.Source;
            PiceAmount = task.PiceAmount;
            Operator = task.Operator;
            Status = task.Status;
        }
    }
}
