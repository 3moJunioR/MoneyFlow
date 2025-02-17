using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expense_Tracker.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        //CategoryID
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public int Ammount { get; set; }

        [Column(TypeName = "nvarchar(75)")]
        public string? Note { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        [NotMapped]
        public string? CategoryTitleWithIcon
        {
            get
            {
                return Category == null ? "" : Category.Icon + " " + Category.Title;
            }
        }
        [NotMapped]
        public string? FormattedAmount
        {
            get
            {
               return ((Category == null || Category.Type=="Expense" )? "- " : "+") + Ammount.ToString("C0");
            }
        }


    }
}
