using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasker.Model
{
    public class ItemLenSet
    {

        public ItemLenData[] itemLenData = new ItemLenData[15];
        public ItemLenSet(ProductionTaskExtended task)
        {

            itemLenData[0] = new ItemLenData()
            {
                ItemLen = task.Task.PieceLength1 ?? 0,
                ItemAmount = task.Task.PieceQuantity1 ?? 0,
                Labeling1 = task.Task.Labeling1Piece1 ?? "",
                Labeling2 = task.Task.Labeling2Piece1 ?? ""
            };

            itemLenData[1] = new ItemLenData()
            {
            ItemLen = task.Task.PieceLength2 ?? 0,
            ItemAmount = task.Task.PieceQuantity2 ?? 0,
            Labeling1 = task.Task.Labeling1Piece2 ?? "",
            Labeling2 = task.Task.Labeling2Piece2 ?? ""
        };
        itemLenData[2] = new ItemLenData()
        {
            ItemLen = task.Task.PieceLength3 ?? 0,
            ItemAmount = task.Task.PieceQuantity3 ?? 0,
            Labeling1 = task.Task.Labeling1Piece3 ?? "",
            Labeling2 = task.Task.Labeling2Piece3 ?? ""
        };
        itemLenData[3] = new ItemLenData()
        {
            ItemLen = task.Task.PieceLength4 ?? 0,
            ItemAmount = task.Task.PieceQuantity4 ?? 0,
            Labeling1 = task.Task.Labeling1Piece4 ?? "",
            Labeling2 = task.Task.Labeling2Piece4 ?? ""
        };
        itemLenData[4] = new ItemLenData()
        {
            ItemLen = task.Task.PieceLength5 ?? 0,
            ItemAmount = task.Task.PieceQuantity5 ?? 0,
            Labeling1 = task.Task.Labeling1Piece5 ?? "",
            Labeling2 = task.Task.Labeling2Piece5 ?? ""
        };
        itemLenData[5] = new ItemLenData()
        {
            ItemLen = task.Task.PieceLength6 ?? 0,
            ItemAmount = task.Task.PieceQuantity6 ?? 0,
            Labeling1 = task.Task.Labeling1Piece6 ?? "",
            Labeling2 = task.Task.Labeling2Piece6 ?? ""
        };
        itemLenData[6] = new ItemLenData()
        {
            ItemLen = task.Task.PieceLength7 ?? 0,
            ItemAmount = task.Task.PieceQuantity7 ?? 0,
            Labeling1 = task.Task.Labeling1Piece7 ?? "",
            Labeling2 = task.Task.Labeling2Piece7 ?? ""
        };
        itemLenData[7] = new ItemLenData()
        {
            ItemLen = task.Task.PieceLength8 ?? 0,
            ItemAmount = task.Task.PieceQuantity8 ?? 0,
            Labeling1 = task.Task.Labeling1Piece8 ?? "",
            Labeling2 = task.Task.Labeling2Piece8 ?? ""
        };
        itemLenData[8] = new ItemLenData()
        {
            ItemLen = task.Task.PieceLength9 ?? 0,
            ItemAmount = task.Task.PieceQuantity9 ?? 0,
            Labeling1 = task.Task.Labeling1Piece9 ?? "",
            Labeling2 = task.Task.Labeling2Piece9 ?? ""
        };
        itemLenData[9] = new ItemLenData()
        {
            ItemLen = task.Task.PieceLength10 ?? 0,
            ItemAmount = task.Task.PieceQuantity10 ?? 0,
            Labeling1 = task.Task.Labeling1Piece10 ?? "",
            Labeling2 = task.Task.Labeling2Piece10 ?? ""
        };
        itemLenData[10] = new ItemLenData()
        {
            ItemLen = task.Task.PieceLength11 ?? 0,
            ItemAmount = task.Task.PieceQuantity11 ?? 0,
            Labeling1 = task.Task.Labeling1Piece11 ?? "",
            Labeling2 = task.Task.Labeling2Piece11 ?? ""
        };
        itemLenData[11] = new ItemLenData()
        {
            ItemLen = task.Task.PieceLength12 ?? 0,
            ItemAmount = task.Task.PieceQuantity12 ?? 0,
            Labeling1 = task.Task.Labeling1Piece12 ?? "",
            Labeling2 = task.Task.Labeling2Piece12 ?? ""
        };
            itemLenData[12] = new ItemLenData()
            {
                ItemLen = task.Task.PieceLength13 ?? 0,
                ItemAmount = task.Task.PieceQuantity13 ?? 0,
                Labeling1 = task.Task.Labeling1Piece13 ?? "",
                Labeling2 = task.Task.Labeling2Piece13 ?? ""
            };
            itemLenData[13] = new ItemLenData()
            {
                ItemLen = task.Task.PieceLength14 ?? 0,
                ItemAmount = task.Task.PieceQuantity14 ?? 0,
                Labeling1 = task.Task.Labeling1Piece14 ?? "",
                Labeling2 = task.Task.Labeling2Piece14 ?? ""
            };
            itemLenData[14] = new ItemLenData()
            {
                ItemLen = task.Task.PieceLength15 ?? 0,
                ItemAmount = task.Task.PieceQuantity15 ?? 0,
                Labeling1 = task.Task.Labeling1Piece15 ?? "",
                Labeling2 = task.Task.Labeling2Piece15 ?? ""
            };
        }
        
    }
    public class ItemLenData
    {
        public int ItemAmount;
        public double ItemLen;
        public string Labeling1;
        public string Labeling2;

    }
        
}

