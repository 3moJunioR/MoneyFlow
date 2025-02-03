using System.ComponentModel.DataAnnotations.Schema;

namespace Expense_Tracker.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        //CategoryID
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int Ammount { get; set; }

        [Column(TypeName = "nvarchar(75)")]
        public string? Note { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;


    }
}
