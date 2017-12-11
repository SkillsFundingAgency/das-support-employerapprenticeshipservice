﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.ApplicationServices;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Infrastructure.Services;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests
{
    [TestFixture]
    public class WhenTestingChallengeRepository
    {
        private ChallengeRepository _unit;
        private Mock<IAccountRepository> _accountRepository;
        [SetUp]
        public void Setup()
        {
            _accountRepository = new Mock<IAccountRepository>();
            _unit = new ChallengeRepository(_accountRepository.Object);
        }
        [Test]
        public async Task ItShouldReturnTrueWhenCheckDataHasValidData()
        {
            var account = new Core.Models.Account()
            {
                Transactions = new List<TransactionViewModel>()
                {
                    new TransactionViewModel(){ Balance = 300m, },
                    new TransactionViewModel(){ Balance = 700m},

                },
                PayeSchemes = new List<PayeSchemeViewModel>()
                {
                    new PayeSchemeViewModel()
                    {
                        AddedDate = DateTime.Today.AddMonths(-12),
                        Ref = "123/456789"
                    },
                    new PayeSchemeViewModel()
                    {
                        AddedDate = DateTime.Today.AddMonths(-12),
                        Ref = "124/456789"
                    }

                },

            };
            var challengePermissionQuery = new ChallengePermissionQuery()
            {
                Id = "123",
                Balance= "£1000",
                ChallengeElement1 = "1",
                ChallengeElement2 = "2",
                FirstCharacterPosition = "0",
                SecondCharacterPosition = "1"
            };

            var balance = 1000m;

            _accountRepository.Setup(x => x.GetAccountBalance(challengePermissionQuery.Id))
                .ReturnsAsync(balance);

            var actual = await _unit.CheckData(account, challengePermissionQuery);

            Assert.IsTrue(actual);

        }
        [Test]
        public async Task ItShouldReturnFalseWhenCheckDataHasInvalidCharacterData()
        {
            var account = new Core.Models.Account()
            {
                Transactions = new List<TransactionViewModel>()
                {
                    new TransactionViewModel(){ Balance = 300m, },
                    new TransactionViewModel(){ Balance = 700m},

                },
                PayeSchemes = new List<PayeSchemeViewModel>()
                {
                    new PayeSchemeViewModel()
                    {
                        AddedDate = DateTime.Today.AddMonths(-12),
                        Ref = "123/456789"
                    },
                    new PayeSchemeViewModel()
                    {
                        AddedDate = DateTime.Today.AddMonths(-12),
                        Ref = "124/456789"
                    }

                },

            };
            var challengePermissionQuery = new ChallengePermissionQuery()
            {
                Id = "123",
                Balance = "£1000",
                ChallengeElement1 = "1",
                ChallengeElement2 = "2",
                FirstCharacterPosition = "1",
                SecondCharacterPosition = "2"
            };

            var balance = 1000m;

            _accountRepository.Setup(x => x.GetAccountBalance(challengePermissionQuery.Id))
                .ReturnsAsync(balance);

            var actual = await _unit.CheckData(account, challengePermissionQuery);

            Assert.IsFalse(actual);

        }

        [Test]
        public async Task ItShouldReturnFalseWhenCheckDataHasInvalidBalance()
        {
            var account = new Core.Models.Account()
            {
                Transactions = new List<TransactionViewModel>()
                {
                    new TransactionViewModel(){ Balance = 300m, },
                    new TransactionViewModel(){ Balance = 700m},

                },
                PayeSchemes = new List<PayeSchemeViewModel>()
                {
                    new PayeSchemeViewModel()
                    {
                        AddedDate = DateTime.Today.AddMonths(-12),
                        Ref = "123/456789"
                    },
                    new PayeSchemeViewModel()
                    {
                        AddedDate = DateTime.Today.AddMonths(-12),
                        Ref = "124/456789"
                    }

                },

            };
            var challengePermissionQuery = new ChallengePermissionQuery()
            {
                Id = "123",
                Balance = "£1000",
                ChallengeElement1 = "1",
                ChallengeElement2 = "2",
                FirstCharacterPosition = "0",
                SecondCharacterPosition = "1"
            };

            var balance = 999m;

            _accountRepository.Setup(x => x.GetAccountBalance(challengePermissionQuery.Id))
                .ReturnsAsync(balance);

            var actual = await _unit.CheckData(account, challengePermissionQuery);

            Assert.IsFalse(actual);

        }

    }
}