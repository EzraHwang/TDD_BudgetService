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
                var amount=0;
                if (IsNotSameMonth(start, end))
                {
                    
                    amount = GetStartMonthAmount(start, amount, allAmount);

                    amount = GetEndMonthAmount(end, amount, allAmount);


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

        private int GetEndMonthAmount(DateTime end, int amount, List<Budget> allAmount)
        {
            var firstDayOfEndMonth = new DateTime(end.Year, end.Month, 1);
            var days = (end - firstDayOfEndMonth).Days + 1;
            amount += days * GetAmountForOneDay(end, allAmount);
            return amount;
        }

        private int GetStartMonthAmount(DateTime start, int amount, List<Budget> allAmount)
        {
            var lastDayOfStartMonth = new DateTime(start.Year, start.Month, DateTime.DaysInMonth(start.Year, start.Month));
            var days = (lastDayOfStartMonth - start).Days + 1;
            amount += days * GetAmountForOneDay(start, allAmount);
            return amount;
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