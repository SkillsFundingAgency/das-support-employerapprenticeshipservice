using System.Threading.Tasks;
using NUnit.Framework;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository
{
    [TestFixture]
    public class WhenCallingFindAllDetails : WhenTestingAccountRepository
    {
        [Test]
        public async Task ItShouldTestTheBehaviour()
        {
            /*
             var results = new List<AccountDetailViewModel>();
            foreach (var account in await FindAll())
                try
                {
                    var viewModel = await _accountApiClient.GetAccount(account.AccountHashId);
                    results.Add(viewModel);
                }
                catch (HttpRequestException e)
                {
                    _logger.Warn($"The Account API Http request threw an exception:\r\n{e}");
                }
                catch (Exception e)
                {
                    _logger.Error(e, $"A general exception has been thrown while requesting employer account detals");
                }

            return results;
             */
            string id = null;
            var actual = await Unit.FindAllDetails();
            Assert.Fail();
        }
    }
}