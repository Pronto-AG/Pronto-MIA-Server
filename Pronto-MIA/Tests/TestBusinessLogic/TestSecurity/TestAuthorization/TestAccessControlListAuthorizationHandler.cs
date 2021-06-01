// namespace Tests.TestBusinessLogic.TestSecurity.TestAuthorization
// {
//     using System;
//     using System.Collections.Generic;
//     using System.Security.Claims;
//     using System.Threading.Tasks;
//     using HotChocolate.Resolvers;
//     using Microsoft.AspNetCore.Authorization;
//     using NSubstitute;
//     using Pronto_MIA.BusinessLogic.Security.Authorization;
//     using Pronto_MIA.DataAccess;
//     using Pronto_MIA.Domain.Entities;
//     using Xunit;
//
//     public class TestAccessControlListAuthorizationHandler
//     {
//         private readonly ProntoMiaDbContext dbContext;
//         private readonly AccessControlListAuthorizationHandler handler;
//
//         public TestAccessControlListAuthorizationHandler()
//         {
//             var name = Guid.NewGuid().ToString();
//             this.dbContext = TestHelpers.InMemoryDbContext(name);
//             TestDataProvider.InsertTestData(this.dbContext);
//             this.handler = new AccessControlListAuthorizationHandler(
//                 TestHelpers.TestConfiguration);
//             this.handler.SetDbOptions(TestHelpers.GetInMemoryDbOptions(name));
//         }
//
//         [Fact]
//         private async Task TestAllowed()
//         {
//             var user = await this.AddUserToDb();
//             var acl = new AccessControlList(user.Id)
//             {
//                 CanViewDeploymentPlans = true,
//             };
//             this.dbContext.AccessControlLists.Add(acl);
//             await this.dbContext.SaveChangesAsync();
//             var requirement = new AccessControlListRequirement(
//                 AccessControl.CanViewDeploymentPlans);
//             var context = new AuthorizationHandlerContext(
//                 new List<IAuthorizationRequirement>() { requirement },
//                 this.ClaimsPrincipalFromUser(user),
//                 Substitute.For<IResolverContext>());
//
//             await this.handler.HandleAsync(context);
//
//             Assert.True(context.HasSucceeded);
//
//             await this.RemoveUserFromDb(user);
//         }
//
//         [Fact]
//         private async Task TestForbidden()
//         {
//             var user = await this.AddUserToDb();
//             var acl = new AccessControlList(user.Id)
//             {
//                 CanViewDeploymentPlans = true,
//             };
//             this.dbContext.AccessControlLists.Add(acl);
//             await this.dbContext.SaveChangesAsync();
//             var requirement = new AccessControlListRequirement(
//                 AccessControl.CanEditUsers);
//             var context = new AuthorizationHandlerContext(
//                 new List<IAuthorizationRequirement>() { requirement },
//                 this.ClaimsPrincipalFromUser(user),
//                 Substitute.For<IResolverContext>());
//
//             await this.handler.HandleAsync(context);
//
//             Assert.False(context.HasSucceeded);
//
//             await this.RemoveUserFromDb(user);
//         }
//
//         private ClaimsPrincipal ClaimsPrincipalFromUser(User user)
//         {
//             var claims = new[]
//             {
//                 new Claim(
//                     ClaimTypes.NameIdentifier, user.Id.ToString()),
//                 new Claim(ClaimTypes.Name, user.UserName),
//             };
//             var id = new ClaimsIdentity();
//             id.AddClaims(claims);
//
//             var claimsPrincipal = new ClaimsPrincipal();
//             claimsPrincipal.AddIdentity(id);
//
//             return claimsPrincipal;
//         }
//
//         private async Task<User> AddUserToDb()
//         {
//             var user = new User(
//                 "Bob2",
//                 new byte[5],
//                 string.Empty,
//                 string.Empty);
//             this.dbContext.Users.Add(user);
//             await this.dbContext.SaveChangesAsync();
//             return user;
//         }
//
//         private async Task RemoveUserFromDb(User user)
//         {
//             this.dbContext.Remove(user);
//             await this.dbContext.SaveChangesAsync();
//         }
//     }
// }