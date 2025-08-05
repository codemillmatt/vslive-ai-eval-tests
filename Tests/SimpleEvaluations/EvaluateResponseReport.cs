using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;
using Microsoft.Extensions.AI.Evaluation.Reporting;
using Microsoft.Extensions.AI.Evaluation.Reporting.Storage;
using Microsoft.Extensions.Configuration;
using System.ClientModel;

namespace SimpleEvaluations;

[TestClass]
public class EvaluateResponseReport
{
    public TestContext? TestContext { get; set; }

    private static ChatConfiguration? chatConfiguration;
    private static IConfiguration? configuration;
    private static ReportingConfiguration? reportingConfiguration;

    private static string? executionName;
    public static string ExecutionName
    {
        get
        {
            if (executionName is null)
            {
                executionName = $"{DateTime.Now:yyyyMMddTHHmmss}";
            }

            return executionName;
        }
    }
    private string ScenarioName => $"{TestContext!.FullyQualifiedTestClassName}.{TestContext.TestName}";

    [ClassInitialize]
    public static void InitializeAsync(TestContext _)
    {
        // Build configuration to access user secrets
        configuration = new ConfigurationBuilder()
            .AddUserSecrets<EvaluateResponseReport>()
            .Build();

        chatConfiguration = GetChatConfiguration();
        reportingConfiguration = GetReportingConfiguration();
    }

    private static ChatConfiguration GetChatConfiguration()
    {
        ApiKeyCredential key = new(configuration!["FOUNDRY-API-KEY"]!);
        Uri endpoint = new Uri(configuration!["FOUNDRY-URL-ENDPOINT"]!);
        string deployedModelName = configuration!["DEPLOYED-MODEL-NAME"]!;

        IChatClient client = new AzureOpenAIClient(endpoint, key)
            .GetChatClient(deployedModelName)
            .AsIChatClient();

        client = client.AsBuilder().UseFunctionInvocation().Build();

        return new ChatConfiguration(client);
    }

    private static ReportingConfiguration GetReportingConfiguration()
    {
        var diskReportConfiguration = DiskBasedReportingConfiguration.Create(
            storageRootPath: @"C:\TestReport",
            evaluators: GetEvaluators(),
            chatConfiguration: chatConfiguration,
            enableResponseCaching: true,
            executionName: ExecutionName,
            tags: ["simple-test"]
        );

        return diskReportConfiguration;
    }

    private static IEnumerable<IEvaluator> GetEvaluators()
    {
        IEvaluator coherenceEvaluator = new CoherenceEvaluator();
        yield return coherenceEvaluator;

        IEvaluator relevanceEvaluator = new RelevanceEvaluator();
        yield return relevanceEvaluator;
    }

    [TestMethod]
    public async Task CoherenceToReport()
    {
        await using ScenarioRun scenarioRun = await reportingConfiguration
            !.CreateScenarioRunAsync(ScenarioName);

        (IList<ChatMessage> messages, ChatResponse modelResponse) =
            await GetAstronomyConversationAsync(
                chatClient: scenarioRun.ChatConfiguration!.ChatClient,
                astronomyQuestion: "How far is the Moon from the Earth at its closest and furthest points?");

        EvaluationResult result = await scenarioRun.EvaluateAsync(messages, modelResponse);

        Validate(result);

        //TODO: Get a report written from AI Lab
    }

    private static async Task<(IList<ChatMessage> Messages, ChatResponse ModelResponse)> GetAstronomyConversationAsync(
        IChatClient chatClient,
        string astronomyQuestion)
    {
        const string SystemPrompt =
            """
            You are an AI assistant that can answer questions related to astronomy.
            Keep your responses concise staying under 100 words as much as possible.
            Use the imperial measurement system for all measurements in your response.
            """;

        IList<ChatMessage> messages =
            [
                new ChatMessage(ChatRole.System, SystemPrompt),
                new ChatMessage(ChatRole.User, astronomyQuestion)
            ];

        var chatOptions =
            new ChatOptions
            {
                Temperature = 0.0f,
                ResponseFormat = ChatResponseFormat.Text
            };

        ChatResponse response = await chatClient.GetResponseAsync(messages, chatOptions);
        return (messages, response);
    }

    private static void Validate(EvaluationResult result)
    {
        using var _ = new AssertionScope();

        EvaluationRating[] expectedRatings = [EvaluationRating.Good, EvaluationRating.Exceptional];

        NumericMetric coherence = result.Get<NumericMetric>(CoherenceEvaluator.CoherenceMetricName);
        coherence.Interpretation!.Failed.Should().BeFalse(because: coherence.Interpretation.Reason);
        coherence.Interpretation.Rating.Should().BeOneOf(expectedRatings, because: coherence.Reason);
        coherence.ContainsDiagnostics(d => d.Severity >= EvaluationDiagnosticSeverity.Warning).Should().BeFalse();
        coherence.Value.Should().BeGreaterThanOrEqualTo(4, because: coherence.Reason);

        NumericMetric relevance = result.Get<NumericMetric>(RelevanceEvaluator.RelevanceMetricName);
        relevance.Interpretation!.Failed.Should().BeFalse(because: relevance.Interpretation.Reason);
        relevance.Interpretation.Rating.Should().BeOneOf(expectedRatings, because: relevance.Reason);
        relevance.ContainsDiagnostics(d => d.Severity >= EvaluationDiagnosticSeverity.Warning).Should().BeFalse();
        relevance.Value.Should().BeGreaterThanOrEqualTo(4, because: relevance.Reason);
    }
}
