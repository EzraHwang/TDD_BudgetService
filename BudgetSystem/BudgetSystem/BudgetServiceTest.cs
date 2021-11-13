using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;

namespace BudgetSystem
{
    public class BudgetServiceTest
    {
        private IBudgetRepo _budgetRepo;
        private BudgetService _budgetService;

        [SetUp]
        public void Setup()
        {
            _budgetRepo = Substitute.For<IBudgetRepo>();
            _budgetService = new BudgetService(_budgetRepo);
        }

        [Test]
        public void StartDateBiggerThanEndDate()
        {
            var query = _budgetService.Query(new DateTime(2021, 12, 01), new DateTime(2021, 11, 01));
            Assert.AreEqual(0, query);
        }
        
        [Test]
        public void WholeMonth()
        {
            GenerateFakeData();
            var query = _budgetService.Query(new DateTime(2021, 11, 01), new DateTime(2021, 11, 30));
            Assert.AreEqual(3000, query);
        }
        
        [Test]
        public void TwoWholeMonth()
        {
            GenerateFakeData();
            var query = _budgetService.Query(new DateTime(2021, 11, 01), new DateTime(2021, 12, 31));
            Assert.AreEqual(34000, query);
        }
        [Test]
        public void SameDate()
        {
            GenerateFakeData();
            var query = _budgetService.Query(new DateTime(2021, 11, 01), new DateTime(2021, 11, 01));
            Assert.AreEqual(100, query);
        }
        
        [Test]
        public void CrossPartialMonth()
        {
            GenerateFakeData();
            var query = _budgetService.Query(new DateTime(2021, 11, 30), new DateTime(2021, 12, 02));
            Assert.AreEqual(2100, query);
        }
        
        [Test]
        public void PartialMonth()
        {
            GenerateFakeData();
            var query = _budgetService.Query(new DateTime(2021, 11, 01), new DateTime(2021, 11, 10));
            Assert.AreEqual(1000, query);
        }
        
        [Test]
        public void EmptyData()
        {
            GenerateFakeData();
            var query = _budgetService.Query(new DateTime(2021, 09, 01), new DateTime(2021, 09, 30));
            Assert.AreEqual(0, query);
        }
        
        [Test]
        public void TestCase()
        {
            GenerateFakeData();
            var query = _budgetService.Query(new DateTime(2021, 10, 02), new DateTime(2021, 12, 01));
            Assert.AreEqual(7000, query);
        }


        private void GenerateFakeData()
        {
            _budgetRepo.GetAll().Returns(new List<Budget>
            {
                new Budget()
                {
                    YearMonth = "202110",
                    Amount = 3100
                },
                new Budget()
                {
                    YearMonth = "202111",
                    Amount = 3000
                },
                new Budget()
                {
                    YearMonth = "202112",
                    Amount = 31000
                }
            });
        }
    }
}