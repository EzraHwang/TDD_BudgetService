using System.Collections.Generic;

namespace BudgetSystem.Model
{
    public interface IBudgetRepo
    {
        List<Budget> GetAll();
    }
}