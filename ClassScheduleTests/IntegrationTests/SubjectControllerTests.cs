using ClassSchedule;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassScheduleTests.IntegrationTests
{
    public class SubjectControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public SubjectControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();


        }
    }
}
