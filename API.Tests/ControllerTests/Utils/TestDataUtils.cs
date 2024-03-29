using System.Security.Cryptography;
using System.Text;
using API.Entitities;
using Microsoft.AspNetCore.Routing.Tree;

namespace API.Tests.ControllerTests.Utils
{
    public static class TestDataUtils
    {
        static Byte[] PasswordHash = Encoding.ASCII.GetBytes("SecretPasswordHash");
        static Byte[] PasswordSalt = Encoding.ASCII.GetBytes("SecretPasswordSalt");
        static readonly Random random = new Random();

        public static List<AppUser> GetAppUserList(int numberOfItems)
        {
            List<AppUser> testData = new List<AppUser>();
            for (int i = 0; i < numberOfItems; i++)
            {
                testData.Add(CreateAppUser("testPassword"));
            }
            return testData;
        }

        public static AppUser CreateAppUser(string password)
        {
            using var hmac = new HMACSHA512(PasswordSalt);
            return new AppUser
            {
                Id = Faker.RandomNumber.Next(),
                UserName = Faker.Internet.UserName(),
                PasswordSalt = PasswordSalt,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                DateOfBirth = new DateOnly(
                    random.Next(1926, 2006),
                    random.Next(1, 13),
                    random.Next(1, 27)
                ),
                KnownAs = Faker.Name.First(),
                Created = DateTime.Now,
                LastActive = DateTime.Now,
                Gender = (random.Next(0, 2) == 1) ? "Male" : "Female",
                Introduction = Faker.Lorem.Sentence(),
                LookingFor = Faker.Lorem.Sentence(),
                Interests = Faker.Lorem.Sentence(),
                City = Faker.Address.City(),
                Country = Faker.Address.Country(),
                Photos = new List<Photo>()
            };
        }
    }
}
