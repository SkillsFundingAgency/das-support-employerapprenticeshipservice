using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository
{
    [TestFixture]
    public class WhenCallingGetWithAccountFieldsSelectionOrganisations : WhenTestingAccountRepository
    {
        public void ItShoudFailGracefullyAndLogErrorWhenClientThrowsException()
        {
            Assert.Fail("To Do");
        }

        [TestCase(EmployerAgreementStatus.Signed)]
        [TestCase(EmployerAgreementStatus.Pending)]
        [TestCase(EmployerAgreementStatus.Superseded)]
        public async Task ItShouldReturnTheMatchingAccountWithLegalEntitiesThatAreInScope(EmployerAgreementStatus scope)
        {
            string id = "123";

            var accountResponse = new AccountDetailViewModel()
            {
                LegalEntities = new ResourceList(
                    new List<ResourceViewModel>()
                    {
                        new ResourceViewModel(){ Href = "https://tempuri.org/legalEntity/{id}", Id = "ABC"}
                    } ),

            };

            AccountApiClient.Setup(x => x.GetResource<AccountDetailViewModel>($"/api/accounts/{id}"))
                .ReturnsAsync(accountResponse);

            var legalEntityResponse = new LegalEntityViewModel()
            {
                AgreementStatus =  scope
            };

            var legalEntity = accountResponse.LegalEntities[0];

            AccountApiClient.Setup(x => x.GetResource<LegalEntityViewModel>(legalEntity.Href))
                .ReturnsAsync(legalEntityResponse);

            var actual = await Unit.Get(id, AccountFieldsSelection.Organisations);

            
            Logger.Verify(x => x.Debug(It.IsAny<string>()), Times.Exactly(accountResponse.LegalEntities.Count +1));
            
            AccountApiClient.Verify(x=>x.GetResource<LegalEntityViewModel>(It.IsAny<string>()), Times.Exactly(accountResponse.LegalEntities.Count));

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.LegalEntities);
            Assert.AreEqual(accountResponse.LegalEntities.Count,  actual.LegalEntities.Count());

            Assert.IsNull(actual.PayeSchemes);
            Assert.IsNull(actual.Transactions);
            Assert.IsNull(actual.TeamMembers);
        }

        [TestCase(EmployerAgreementStatus.Expired)]
        [TestCase(EmployerAgreementStatus.Removed)]
        public async Task ItShouldReturnTheMatchingAccountWithOutLegalEntitiesThatAreOutOfScope(EmployerAgreementStatus scope)
        {
            string id = "123";

            var accountResponse = new AccountDetailViewModel()
            {
                LegalEntities = new ResourceList(
                    new List<ResourceViewModel>()
                    {
                        new ResourceViewModel(){ Href = "https://tempuri.org/legalEntity/{id}", Id = "ABC"}
                    }),

            };

            AccountApiClient.Setup(x => x.GetResource<AccountDetailViewModel>($"/api/accounts/{id}"))
                .ReturnsAsync(accountResponse);

            var legalEntityResponse = new LegalEntityViewModel()
            {
                AgreementStatus = scope
            };

            var legalEntity = accountResponse.LegalEntities[0];

            AccountApiClient.Setup(x => x.GetResource<LegalEntityViewModel>(legalEntity.Href))
                .ReturnsAsync(legalEntityResponse);

            var actual = await Unit.Get(id, AccountFieldsSelection.Organisations);


            Logger.Verify(x => x.Debug($"{nameof(IAccountApiClient)}.{nameof(IAccountApiClient.GetResource)}<{nameof(AccountDetailViewModel)}>(\"/api/accounts/{id}\");"), Times.Once);
            Logger.Verify(x => x.Debug($"{nameof(IAccountApiClient)}.{nameof(IAccountApiClient.GetResource)}<{nameof(LegalEntityViewModel)}>(\"{legalEntity.Href}\");"), Times.Once);

            AccountApiClient.Verify(x => x.GetResource<LegalEntityViewModel>(It.IsAny<string>()), Times.Exactly(accountResponse.LegalEntities.Count));

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.LegalEntities);
            Assert.AreEqual(0, actual.LegalEntities.Count());

            Assert.IsNull(actual.PayeSchemes);
            Assert.IsNull(actual.Transactions);
            Assert.IsNull(actual.TeamMembers);
        }


    }
}