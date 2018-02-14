﻿using HMRC.ESFA.Levy.Api.Types;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EAS.Support.Core.Services;
using SFA.DAS.EAS.Support.Infrastructure.Services;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Support.ApplicationServices.Tests.PayeSchemeLevySubmissionsHandler
{
    [TestFixture]
    public class WhenCallingFindPayeLevySubmissions
    {

        private Mock<IAccountRepository> _accountRepository;
        private Mock<ILevySubmissionsRepository> _levySubmissionsRepository;
        private Mock<IPayeSchemeObfuscator> _payeSchemeObfuscator;
        private Mock<ILog> _log;
        private Mock<IHashingService> _hashingService;

        private const string _accountId = "45789456123";
        private const string _hashedPayeRef = "HASHPAYEID";
        private const string _actualPayeRef = "155/5555";

        [SetUp]
        public void SetUp()
        {
            _accountRepository = new Mock<IAccountRepository>();
            _levySubmissionsRepository = new Mock<ILevySubmissionsRepository>();
            _payeSchemeObfuscator = new Mock<IPayeSchemeObfuscator>();
            _log = new Mock<ILog>();
            _hashingService = new Mock<IHashingService>();

        }


        [Test]
        public async Task ShouldReturnLevyResponseAndCallRequiredMethods()
        {

            var levyDeclarations = new LevyDeclarations
            {
                Declarations = new List<Declaration>
                    {
                        new Declaration
                        {
                            Id = "002",
                             SubmissionTime = new DateTime(2017, 4, 1)
                        },
                        new Declaration
                        {
                             Id = "003",
                           SubmissionTime = new DateTime(2017, 4, 1)
                        }
                    }
            };

            var accountModel = new Core.Models.Account
            {
                HashedAccountId = _accountId,
                DasAccountName = "TEST",
                PayeSchemes = new List<PayeSchemeModel>
                {
                    new PayeSchemeModel
                    {
                        Ref = _actualPayeRef
                    }
               }
            };

            _accountRepository
                .Setup(x => x.Get(_accountId, AccountFieldsSelection.PayeSchemes))
                .Returns(Task.FromResult(accountModel));

            _hashingService
                .Setup(x => x.DecodeValueToString(_hashedPayeRef))
                .Returns(_actualPayeRef);

            _payeSchemeObfuscator
                .Setup(x => x.ObscurePayeScheme(_actualPayeRef))
                 .Returns("1*******5");

            _levySubmissionsRepository
                .Setup(x => x.Get(_actualPayeRef))
                .Returns(Task.FromResult(levyDeclarations));

            var _sut = new PayeLevySubmissionsHandler(_accountRepository.Object,
                                                    _levySubmissionsRepository.Object,
                                                    _payeSchemeObfuscator.Object,
                                                    _log.Object,
                                                    _hashingService.Object);


            var response = await _sut.FindPayeSchemeLevySubmissions(_accountId, _hashedPayeRef);

            _payeSchemeObfuscator
              .Verify(x => x.ObscurePayeScheme(_actualPayeRef), Times.Once);

            _hashingService
                .Verify(x => x.DecodeValueToString(_hashedPayeRef), Times.Once);

            _levySubmissionsRepository
                .Verify(x => x.Get(_actualPayeRef), Times.Once);

            Assert.NotNull(response);
            Assert.IsNotNull(response.LevySubmissions);
            Assert.AreEqual(2, response.LevySubmissions.Declarations.Count());
        }

    }
}
