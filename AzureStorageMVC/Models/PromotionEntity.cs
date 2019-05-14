using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace AzureStorageMVC.Models
{
    public class PromotionEntity : TableEntity
    {
        public int ProductId { get; set; }
        public double Discount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public PromotionEntity()
        {
            this.PartitionKey = Guid.NewGuid().ToString();
            this.RowKey = Guid.NewGuid().ToString();
        }

        public PromotionEntity(int productId, double discount, DateTime startDate, DateTime endDate) : this()
        {
            this.ProductId = productId;
            this.Discount = discount;
            this.StartDate = startDate;
            this.EndDate = endDate;
        }
    }
}
