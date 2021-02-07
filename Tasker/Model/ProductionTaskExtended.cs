using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasker.Model
{
    public class ProductionTaskExtended
    {
        public ProductionTask Task { get; set; }
        public ProductionTaskExtended(ProductionTask task)
        {
            Task = task;
            serialLabel = new ParsedSerialLabel(Task.StartSerialNumber);
        }
        public ProductionTaskExtended(Task task)
        {
            Task = new ProductionTask()
            {
                ID = task.ID,
                TaskNumber = task.TaskNumber,
                TaskPosition = task.TaskPosition,
                Product = task.Product,
                ProductBatchNumber = task.ProductBatchNumber,
                StartSerialNumber = task.StartSerialNumber,
                ProductsAmount = task.ProductsAmount,
                PipeBatchNumber = task.PipeBatchNumber,
                PipeNumber = task.PipeNumber,
                Heat = task.Heat,
                SteelType = task.SteelType,
                Diameter = task.Diameter,
                Thickness = task.Thickness,
                PieceLength = task.PieceLength,
                PieceQuantity = task.PieceQuantity,
                CreationDate = task.CreationDate,
                StartDate = task.StartDate,
                FinishDate = task.FinishDate,
                Source = task.Source,
                PiceAmount = task.PiceAmount,
                Operator = task.Operator,
                Status = task.Status,
                PieceLength1 = task.PieceLength1,
                PieceQuantity1 = task.PieceQuantity1,
                Labeling1Piece1 = task.Labeling1Piece1,
                Labeling2Piece1 = task.Labeling2Piece1                               
            };
            serialLabel = new ParsedSerialLabel(Task.StartSerialNumber);
        }
        public ParsedSerialLabel serialLabel { get; set; }

    }
}
