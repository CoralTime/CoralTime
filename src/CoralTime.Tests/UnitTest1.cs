using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using CoralTime.ViewModels.Member;

namespace CoralTime.Tests
{
    public class UnitTest1 : IClassFixture<TestApplicationFactory>
    {
        private readonly TestApplicationFactory _factory;

        public UnitTest1(TestApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task TestMethodNonAuth()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/v1/Test/ping");
            response.EnsureSuccessStatusCode();

            // Test result content
            var result = await response.Content.ReadAsStringAsync();

            Assert.Equal("I'm alive!", result);
        }

        [Fact]
        public async Task TestMethodAuth()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/v1/odata/Members");
            response.EnsureSuccessStatusCode();

            // Test result content
            var result = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.GetProperty("value").GetRawText();
            var membersList = JsonSerializer.Deserialize<List<MemberView>>(result, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            Assert.NotEmpty(membersList);
        }
    }
}
