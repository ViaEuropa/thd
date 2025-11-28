namespace Thd;

[CollectionDefinition("Api collection")]
public class ApiCollection : ICollectionFixture<ApiFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}