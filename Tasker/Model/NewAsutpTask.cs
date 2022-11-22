using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasker.Model
{
    public class NewAsutpTask
    {
        public Task Task { get; set; }
        public NewAsutpTask(ProductionTask prodTask, string Line)
        {
            Task = new Task()
            {
                TaskNumber = prodTask.TaskNumber,
                TaskPosition = prodTask.TaskPosition,
                Product = prodTask.Product,
                ProductBatchNumber = prodTask.ProductBatchNumber,
                StartSerialNumber = prodTask.StartSerialNumber,
                ProductsAmount = prodTask.ProductsAmount,
                PipeBatchNumber = prodTask.PipeBatchNumber,
                PipeNumber = prodTask.PipeNumber,
                Heat = prodTask.Heat,
                SteelType = prodTask.SteelType,
                Diameter = prodTask.Diameter,
                Thickness = prodTask.Thickness,
                PieceLength = prodTask.PieceLength,
                PieceQuantity = prodTask.PieceQuantity,
                CreationDate = prodTask.CreationDate,
                StartDate = prodTask.StartDate,
                FinishDate = prodTask.FinishDate,
                Source = prodTask.Source,
                PiceAmount = prodTask.PiceAmount,
                Operator = prodTask.Operator,
                Status = prodTask.Status,
                PieceLength1 = prodTask.PieceLength1,
                PieceQuantity1 = prodTask.PieceQuantity1,
                Labeling1Piece1 = prodTask.Labeling1Piece1,
                Labeling2Piece1 = prodTask.Labeling2Piece1,

                BandBrand = prodTask.BandBrand,
                BandType = prodTask.BandType,
                BandSpeed = prodTask.BandSpeed,
                SawDownSpeed = prodTask.SawDownSpeed,
                LineNumber = Line                
            };
        }
    }
}
