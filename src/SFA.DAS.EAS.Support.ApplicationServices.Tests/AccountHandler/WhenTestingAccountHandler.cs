using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices.Services;

namespace SFA.DAS.EAS.Support.ApplicationServices.Tests.AccountHandler
{
    public abstract class WhenTestingAccountHandler
    {
        protected ApplicationServices.Services.AccountHandler Unit;
        protected Mock<IAccountRepository> MockAccountRepository;
        protected readonly string Id = "123";

        [SetUp]
        public void Setup()
        {
            MockAccountRepository = new Mock<IAccountRepository>();

            Unit = new ApplicationServices.Services.AccountHandler(MockAccountRepository.Object);
        }

      
    }
}