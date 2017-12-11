using Moq;
using NUnit.Framework;

namespace SFA.DAS.EAS.Support.ApplicationServices.Tests.AccountHandler
{
    [TestFixture]
    public class WhenTestingAccountHandler
    {
        [SetUp]
        public void Setup()
        {
            MockAccountRepository = new Mock<IAccountRepository>();
            Unit = new ApplicationServices.Services.AccountHandler(MockAccountRepository.Object);
        }

        protected ApplicationServices.Services.AccountHandler Unit;

        protected Mock<IAccountRepository> MockAccountRepository;
        protected readonly string Id = "123";
    }
}