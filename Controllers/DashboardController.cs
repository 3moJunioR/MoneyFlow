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

            //Last 30 Days
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

            //Spline Chart - Income vs Expense
            //Income
            List<SplineChartData> IncomeSummary = SelectedTransactions
                .Where(i=> i.Category.Type=="Income")
                .GroupBy(j=>j.Date)
                .Select(k=> new SplineChartData()
                {
                    day=k.First().Date.ToString("dd-MMM"),
                    income=k.Sum(l => l.Ammount),

                })
                .ToList();

            //Expense
            List<SplineChartData> ExpenseSummary = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    expense = k.Sum(l => l.Ammount),

                })
                .ToList();
            //Combine Income & Expense
            string[] last30Day = Enumerable.Range(0, 30)
                .Select(i => StartDate.AddDays(i).ToString("dd-MMM"))
                .ToArray();
            ViewBag.SplineChartData = from day in last30Day
                                      join income in IncomeSummary on day equals income.day into dayIncomeJoined
                                      from income in dayIncomeJoined.DefaultIfEmpty()
                                      join expense in ExpenseSummary on day equals expense.day into expenseJoined
                                      from expense in expenseJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          day = day,
                                          income = income == null ? 0 : income.income,
                                          expense = expense == null ? 0 : expense.expense,
                                      };


            return View();
        }
    }
    public class SplineChartData
    {
        public string day;
        public int income;
        public int expense;
    }
}
