namespace Bitbucket.REST.API.Tests.Extensions
{
    public static class MockExtensions
    {
        public static CaptureExpression<T> Capture<T>(this T stub)
            where T : class
        {
            return new CaptureExpression<T>(stub);
        }
    }
}