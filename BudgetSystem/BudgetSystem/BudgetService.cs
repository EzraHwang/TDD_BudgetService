using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetSystem
{
    public class BudgetService
    {
        private readonly IBudgetRepo _budgetRepo;

        public BudgetService(IBudgetRepo budgetRepo)
        {
            _budgetRepo = budgetRepo;
        }

        public decimal Query(DateTime start, DateTime end)
        {
            if (IsValidInput(start, end))
            {
                var allAmount = _budgetRepo.GetAll();
                
                if (IsNotSameMonth(start, end))
                {
                    var amount=0;
                    var lastDayOfStartMonth = new DateTime(start.Year, start.Month, DateTime.DaysInMonth(start.Year, start.Month));
                    var days = (lastDayOfStartMonth - start).Days+1;
                    amount+= days * GetAmountForOneDay(start, allAmount);

                    var firstDayOfEndMonth = new DateTime(end.Year, end.Month, 1);
                    days = (end - firstDayOfEndMonth).Days + 1;
                    amount += days * GetAmountForOneDay(end, allAmount);
                
                

                    var secondYearMonth = new DateTime(start.Year,start.Month+1,1);
                    var lastSecondEndYearMonth = new DateTime(end.Year,end.Month-1,1);
                
                    while (secondYearMonth<=lastSecondEndYearMonth)
                    {
                        var yearMonth = secondYearMonth.ToString("yyyyMM");
                        amount+=GetAmountForAllMonth(allAmount, yearMonth);

                        secondYearMonth=secondYearMonth.AddMonths(1);
                    }
                }
                else
                {
                    return ((end - start).Days + 1) * GetAmountForOneDay(start, allAmount);
                }
                
                return amount;
            }
            return 0;
        }

        private static bool IsNotSameMonth(DateTime start, DateTime end)
        {
            var startYearMonth = new DateTime(start.Year,start.Month,1);
            var lendYearMonth = new DateTime(end.Year,end.Month,1);

            return startYearMonth != lendYearMonth;
        }

        private static bool IsValidInput(DateTime start, DateTime end)
        {
            return start <= end;
        }

        private int GetAmountForOneDay(DateTime start, List<Budget> allAmount)
        {
            var budget = allAmount.FirstOrDefault(x => x.YearMonth.Equals(start.ToString("yyyyMM")));
            return budget == null ? 0 : budget.Amount /
                   DateTime.DaysInMonth(start.Year, start.Month);
        }

        private static int GetAmountForAllMonth(List<Budget> allAmount, string yearMonth)
        {
            var budget = allAmount.FirstOrDefault(x => x.YearMonth.Equals(yearMonth));
            return budget?.Amount ?? 0;
        }
    }

    public interface IBudgetRepo
    {
        List<Budget> GetAll();
    }

    public class Budget
    {
        public string YearMonth { get; set; }
        public int Amount { get; set; }
    }
}