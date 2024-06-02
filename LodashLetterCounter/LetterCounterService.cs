using System.Collections.Concurrent;
using System.Text;
using Octokit;

namespace LodashLetterCounter;

public class LetterCounterService
{        
    private readonly ConcurrentDictionary<char, int> _letters = new();

    private readonly GitHubClient _client;
    private readonly object _lock = new object();
    public LetterCounterService(string token)
    {
        var product = new ProductHeaderValue(Constants.HeaderName);

        _client = new GitHubClient(product)
        {
            Credentials = new Credentials(token)
        };
    }
    
    public async Task GetLetters()
    {
        var contents = await _client.Repository.Content.GetAllContents(Constants.Owner, Constants.Repo);
        var fileUrls = await GetFileUrls(contents);
        var tasks = CreateFileProcessingTasks(fileUrls);
        
        await Task.WhenAll(tasks);
        SortLetters();
    }

    private async Task<List<string>> GetFileUrls(IEnumerable<RepositoryContent> contents)
    {
        var fileUrls = new List<string>();
        var extensions = Constants.Extensions;
        
        foreach (var content in contents)
        {
            if (content.Type == ContentType.File && extensions.Any(ext => content.Name.EndsWith(ext)))
            {
                fileUrls.Add(content.Path);
            }
            else if (content.Type == ContentType.Dir)
            {
                var subDirContents = await _client.Repository.Content.GetAllContents(Constants.Owner, Constants.Repo, content.Path);
                fileUrls.AddRange(await GetFileUrls(subDirContents));
            }
        }

        return fileUrls;
    }
    
    private Task[] CreateFileProcessingTasks(IReadOnlyCollection<string> fileUrls)
    {
        var processedFiles = 0;
        var totalFiles = fileUrls.Count;

        var tasks = fileUrls.Select(async fileUrl =>
        {
            var fileContent = await _client.Repository.Content.GetRawContent(Constants.Owner, Constants.Repo, fileUrl);
            var content = Encoding.UTF8.GetString(fileContent);

            var currentLetterCount = IncreaseLetterCount(content);
            foreach (var kv in currentLetterCount)
            {
                _letters.AddOrUpdate(kv.Key, kv.Value, (key, oldValue) => oldValue + kv.Value);
            }

            lock (_lock)
            {
                processedFiles++;
                var percentage = (int)((double)processedFiles / totalFiles * 100);
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write($"Progress: {processedFiles}/{totalFiles} files processed: {percentage}% completed!");
            }
        }).ToArray();

        return tasks;
    }

    private Dictionary<char, int> IncreaseLetterCount(string content)
    {
        var letterCount = new Dictionary<char, int>();
        foreach (var letter in content.ToLower().Where(char.IsLetter))
        {
            if (letterCount.TryGetValue(letter, out var value))
            {
                letterCount[letter] = ++value;
            }
            else
            {
                letterCount[letter] = 1;
            }
        }

        return letterCount;
    }
    
    private void SortLetters()
    {
        var sortedLetterCount = _letters.OrderByDescending(x => x.Value);

        Console.WriteLine();
        Console.WriteLine("Count of letters:");
        foreach (var kv in sortedLetterCount)
        {
            Console.WriteLine($"{kv.Key}: {kv.Value}");
        }
    }
}