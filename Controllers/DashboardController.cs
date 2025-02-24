using Expense_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using SQLitePCL;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Expense_Tracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task <ActionResult> Index()
        {

            //Last 7 Days
            DateTime StartDate = DateTime.Today.AddDays(-30);
            DateTime EndtDate = DateTime.Today;

            List<Transaction> SelectedTransactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date >= StartDate && y.Date <= EndtDate)
                .ToListAsync();

            //Total Income
            int TotalIncome = SelectedTransactions
                .Where(i => i.Category.Type == "Income")
                .Sum(j => j.Ammount);
            ViewBag.TotalIncome = TotalIncome.ToString("C0");

            //Total Expense
            int TotalExpense = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .Sum(j => j.Ammount);
            ViewBag.TotalExpense = TotalExpense.ToString("C0");

            //Balance 
            int Balance = TotalIncome - TotalExpense;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.Balance = String.Format(culture,"{0:C0}",Balance);

            //RoundedCorner chart - Expense By Category
            ViewBag.RoundedChartData = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Category.CategoryId)
                .Select(k => new
                {
                    categoryTitleWithIcon= k.First().Category.Icon+ " "+ k.First().Category.Title,
                    amount = k.Sum(j => j.Ammount),
                    formattedAmount= k.Sum(j => j.Ammount).ToString("C0"),
                })
                .OrderByDescending(l=>l.amount)
                .ToList();
                    


            return View();
        }
    }
}
