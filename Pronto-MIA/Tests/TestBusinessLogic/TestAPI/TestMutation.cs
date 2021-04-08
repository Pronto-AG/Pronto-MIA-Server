namespace Tests.TestBusinessLogic.TestAPI
{
    using Pronto_MIA.DataAccess;

    public class TestMutation
    {
        private ProntoMiaDbContext dbContext;

        public TestMutation()
        {
            this.dbContext = TestHelpers.InMemoryDbContext;
        }
    }
}