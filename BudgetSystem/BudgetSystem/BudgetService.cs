using System;
using System.Collections.Generic;
using System.Linq;
using BudgetSystem.Model;

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
                if (IsSameMonth(start, end))
                {   
                    return ((end - start).Days + 1) * GetAmountForOneDay(start, allAmount);
                }

                amount += GetStartMonthAmount(start, allAmount);

                amount += GetEndMonthAmount(end, allAmount);

                amount += GetMiddleMonthsAmount(start, end, allAmount);

                return amount;
            }
            return 0;
        }

        private static int GetMiddleMonthsAmount(DateTime start, DateTime end, List<Budget> allAmount)
        {
            var amount = 0;
            var secondYearMonth = new DateTime(start.Year, start.Month + 1, 1);
            var lastSecondEndYearMonth = new DateTime(end.Year, end.Month - 1, 1);

            while (secondYearMonth <= lastSecondEndYearMonth)
            {
                amount += GetAmountForAllMonth(allAmount, secondYearMonth);

                secondYearMonth = secondYearMonth.AddMonths(1);
            }

            return amount;
        }

        private static int GetEndMonthAmount(DateTime end, List<Budget> allAmount)
        {
            var firstDayOfEndMonth = new DateTime(end.Year, end.Month, 1);
            var days = (end - firstDayOfEndMonth).Days + 1;
            return days * GetAmountForOneDay(end, allAmount);
        }

        private static int GetStartMonthAmount(DateTime start, List<Budget> allAmount)
        {
            var lastDayOfStartMonth = new DateTime(start.Year, start.Month, DateTime.DaysInMonth(start.Year, start.Month));
            var days = (lastDayOfStartMonth - start).Days + 1;
            return days * GetAmountForOneDay(start, allAmount);
        }

        private static bool IsSameMonth(DateTime start, DateTime end)
        {
            var startYearMonth = new DateTime(start.Year,start.Month,1);
            var endYearMonth = new DateTime(end.Year,end.Month,1);

            return startYearMonth == endYearMonth;
        }

        private static bool IsValidInput(DateTime start, DateTime end)
        {
            return start <= end;
        }

        private static int GetAmountForOneDay(DateTime start, IEnumerable<Budget> allAmount)
        {
            var amount= GetAmountForAllMonth(allAmount, start);
            return amount / DateTime.DaysInMonth(start.Year, start.Month);
        }

        private static int GetAmountForAllMonth(IEnumerable<Budget> allAmount, DateTime date)
        {
            var budget = allAmount.FirstOrDefault(x => x.YearMonth.Equals(date.ToString("yyyyMM")));
            return budget?.Amount ?? 0;
        }
    }
}