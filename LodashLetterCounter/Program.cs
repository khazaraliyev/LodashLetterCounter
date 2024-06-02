using LodashLetterCounter;
string token;

Console.WriteLine("Please enter your GitHub token with 'repo' scope:");
Console.WriteLine("Authentication is required to avoid rate limits.");
Console.WriteLine("For instructions on how to generate a token, visit:");
Console.WriteLine("https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens");

do
{
    token = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(token))
    {
        Console.WriteLine("Token cannot be empty. Please try again.");
    }
} 
while (string.IsNullOrWhiteSpace(token));

var service = new LetterCounterService(token);

await service.GetLetters();