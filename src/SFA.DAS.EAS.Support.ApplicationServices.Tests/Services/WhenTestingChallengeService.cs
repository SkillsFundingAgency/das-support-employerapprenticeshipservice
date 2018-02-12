using System;
using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Support.ApplicationServices.Tests.Services
{
    [TestFixture]
    public class WhenTestingChallengeService
    {
        [SetUp]
        public void Setup()
        {
            _unit = new ChallengeService();
        }

        private ChallengeService _unit;

        [Test]
        public void ItShouldObtainAnIndexlistFromTheListOfPayeSchemDetails()
        {
            var payeSchemeModel = new List<PayeSchemeModel>
            {
                new PayeSchemeModel
                {
                    AddedDate = DateTime.Today.AddMonths(-12),
                    Name = "Account 123",
                    DasAccountId = "123",
                    Ref = "123/123456",
                    RemovedDate = null
                }
            };

            var actual = _unit.GetPayeSchemesCharacters(payeSchemeModel);

            Assert.IsNotNull(actual);
            CollectionAssert.IsNotEmpty(actual);
        }
    }
}