using Thd.Request;

namespace Thd.Requests;

public class UrlFilterTests
{
    [Theory]
    [InlineData("https://www.google.com/", "www.google.com")]
    [InlineData("https://www.google.com/bar", "www.google.com/bar")]
    [InlineData("https://www.google.com/foo/bar", "www.google.com/foo/bar")]
    [InlineData("https://www.google.com/foo?a=1", "www.google.com/foo?a")]
    [InlineData("https://www.google.com/foo?a=1&b=2", "www.google.com/foo?a?b")]
    [InlineData("https://www.google.com/foo?b=1&a=2", "www.google.com/foo?a?b")]
    [InlineData("https://www.google.com/customer/1567", "www.google.com/customer/{id}")]
    [InlineData("https://www.google.com/customer/1567/bar", "www.google.com/customer/{id}/bar")]
    [InlineData("https://www.google.com/car/ABC-123/?extended=owners", "www.google.com/car/{id}?extended")]
    public void ShouldGenerateUniqueKey(string url, string expected)
    {
        Uri uri = new Uri(url);

        string key = UrlFilter.GenerateUniqueKey(uri);

        Assert.Equal(expected, key);
    }
}