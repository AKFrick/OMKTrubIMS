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
                Labeling2Piece1 = task.Labeling2Piece1,

                PieceLength2 = task.PieceLength2,
                PieceQuantity2 = task.PieceQuantity2,
                Labeling1Piece2 = task.Labeling1Piece2,
                Labeling2Piece2 = task.Labeling2Piece2,

                PieceLength3 = task.PieceLength3,
                PieceQuantity3 = task.PieceQuantity3,
                Labeling1Piece3 = task.Labeling1Piece3,
                Labeling2Piece3 = task.Labeling2Piece3,

                PieceLength4 = task.PieceLength4,
                PieceQuantity4 = task.PieceQuantity4,
                Labeling1Piece4 = task.Labeling1Piece4,
                Labeling2Piece4 = task.Labeling2Piece4,

                PieceLength5 = task.PieceLength5,
                PieceQuantity5 = task.PieceQuantity5,
                Labeling1Piece5 = task.Labeling1Piece5,
                Labeling2Piece5 = task.Labeling2Piece5,

                PieceLength6 = task.PieceLength6,
                PieceQuantity6 = task.PieceQuantity6,
                Labeling1Piece6 = task.Labeling1Piece6,
                Labeling2Piece6 = task.Labeling2Piece6,

                PieceLength7 = task.PieceLength7,
                PieceQuantity7 = task.PieceQuantity7,
                Labeling1Piece7 = task.Labeling1Piece7,
                Labeling2Piece7 = task.Labeling2Piece7,

                PieceLength8 = task.PieceLength8,
                PieceQuantity8 = task.PieceQuantity8,
                Labeling1Piece8 = task.Labeling1Piece8,
                Labeling2Piece8 = task.Labeling2Piece8,

                PieceLength9 = task.PieceLength9,
                PieceQuantity9 = task.PieceQuantity9,
                Labeling1Piece9 = task.Labeling1Piece9,
                Labeling2Piece9 = task.Labeling2Piece9,

                PieceLength10 = task.PieceLength10,
                PieceQuantity10 = task.PieceQuantity10,
                Labeling1Piece10 = task.Labeling1Piece10,
                Labeling2Piece10 = task.Labeling2Piece10,

                PieceLength11 = task.PieceLength11,
                PieceQuantity11 = task.PieceQuantity11,
                Labeling1Piece11 = task.Labeling1Piece11,
                Labeling2Piece11 = task.Labeling2Piece11,

                PieceLength12 = task.PieceLength12,
                PieceQuantity12 = task.PieceQuantity12,
                Labeling1Piece12 = task.Labeling1Piece12,
                Labeling2Piece12 = task.Labeling2Piece12,

                PieceLength13 = task.PieceLength13,
                PieceQuantity13 = task.PieceQuantity13,
                Labeling1Piece13 = task.Labeling1Piece13,
                Labeling2Piece13 = task.Labeling2Piece13,

                PieceLength14 = task.PieceLength14,
                PieceQuantity14 = task.PieceQuantity14,
                Labeling1Piece14 = task.Labeling1Piece14,
                Labeling2Piece14 = task.Labeling2Piece14,

                PieceLength15 = task.PieceLength15,
                PieceQuantity15 = task.PieceQuantity15,
                Labeling1Piece15 = task.Labeling1Piece15,
                Labeling2Piece15 = task.Labeling2Piece15
                               
            };
            serialLabel = new ParsedSerialLabel(Task.StartSerialNumber);
        }
        public ParsedSerialLabel serialLabel { get; set; }

    }
}
