using Moq;
using NUnit.Framework;

namespace SFA.DAS.EAS.Support.ApplicationServices.Tests
{
    [TestFixture]
    public class WhenTestingAccountHandler
    {
        [SetUp]
        public void Setup()
        {
            MockAccountRepository = new Mock<IAccountRepository>();
            Unit = new Services.AccountHandler(MockAccountRepository.Object);
        }

        protected Services.AccountHandler Unit;

        protected Mock<IAccountRepository> MockAccountRepository;
        protected readonly string Id = "123";
    }
}