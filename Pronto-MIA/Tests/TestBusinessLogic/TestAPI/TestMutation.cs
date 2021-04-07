namespace Tests.TestBusinessLogic.TestAPI
{
    using Pronto_MIA.DataAccess;

    public class TestMutation
    {
        private ProntoMIADbContext dbContext;

        public TestMutation()
        {
            this.dbContext = TestHelpers.InMemoryDbContext;
        }
    }
}