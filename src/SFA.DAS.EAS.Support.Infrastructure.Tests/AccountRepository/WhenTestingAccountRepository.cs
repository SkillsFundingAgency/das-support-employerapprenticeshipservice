using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Support.ApplicationServices;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Core.Services;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository
{
    public abstract class WhenTestingAccountRepository
    {
        protected IAccountRepository Unit;
        protected Mock<IAccountApiClient> AccountApiClient;
        protected Mock<IPayeSchemeObfuscator> PayeSchemeObfuscator;
        protected Mock<IDatetimeService> DatetimeService;
        protected Mock<ILog> Logger;

        [SetUp]
        public void Setup()
        {
            AccountApiClient = new Mock<IAccountApiClient>();
            DatetimeService = new Mock<IDatetimeService>();
            Logger = new Mock<ILog>();
            PayeSchemeObfuscator = new Mock<IPayeSchemeObfuscator>();
            Unit = new Services.AccountRepository(
                AccountApiClient.Object,
                PayeSchemeObfuscator.Object,
                DatetimeService.Object,
                Logger.Object
                );
        }
    }
}