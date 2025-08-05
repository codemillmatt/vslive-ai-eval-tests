using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;
using Microsoft.Extensions.Configuration;
using System.ClientModel;

namespace SimpleEvaluations;

[TestClass]
public sealed class EvaluateResponse
{
    private static ChatConfiguration? chatConfiguration;
    private static IConfiguration? configuration;

    private static readonly IList<ChatMessage> messages = [
        new ChatMessage(
            ChatRole.System,
            """
            You are an AI assistant that can answer questions related to astronomy.
            Keep your responses concise staying under 100 words as much as possible.
            Use the imperial measurement system for all measurements in your response.
            """),
        new ChatMessage(
            ChatRole.User,
            "How far is the planet Venus from the Earth at its closest and furthest points?")];

    private static ChatResponse response = new();

    [ClassInitialize]
    public static async Task InitializeAsync(TestContext _)
    {
        // Build configuration to access user secrets
        configuration = new ConfigurationBuilder()
            .AddUserSecrets<EvaluateResponse>()
            .Build();

        chatConfiguration = GetChatConfiguration();

        var chatOptions =
            new ChatOptions
            {
                Temperature = 0.0f,
                ResponseFormat = ChatResponseFormat.Text
            };

        response = await chatConfiguration.ChatClient.GetResponseAsync(messages, chatOptions);
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

    [TestMethod]
    public async Task TestCoherence()
    {
        IEvaluator coherenceEvaluator = new CoherenceEvaluator();

        EvaluationResult result = await coherenceEvaluator
            .EvaluateAsync(messages, response, chatConfiguration
        );

        using var _ = new AssertionScope();

        NumericMetric coherence = result.Get<NumericMetric>(CoherenceEvaluator.CoherenceMetricName);

        coherence.Interpretation!.Failed.Should().BeFalse(because: coherence.Interpretation.Reason);

        EvaluationRating[] expectedRatings = [EvaluationRating.Good, EvaluationRating.Exceptional];
        coherence.Interpretation.Rating.Should().BeOneOf(expectedRatings, because: coherence.Reason);

        coherence.ContainsDiagnostics(d => d.Severity >= EvaluationDiagnosticSeverity.Warning).Should().BeFalse();

        coherence.Value.Should().BeGreaterThanOrEqualTo(4, because: coherence.Reason);
    }

    [TestMethod]
    public async Task TestCoherenceAndRelevance()
    {
        IEvaluator coherenceEvaluator = new CoherenceEvaluator();
        IEvaluator relevanceEvaluator = new RelevanceEvaluator();
        IEvaluator compositeEvaluator = new CompositeEvaluator(coherenceEvaluator, relevanceEvaluator);

        EvaluationResult result = await compositeEvaluator
            .EvaluateAsync(messages, response, chatConfiguration);

        using var _ = new AssertionScope();

        EvaluationRating[] expectedRatings = [EvaluationRating.Good, EvaluationRating.Exceptional];

        /// Retrieve the score for coherence from the <see cref="EvaluationResult"/>.
        NumericMetric coherence = result.Get<NumericMetric>(CoherenceEvaluator.CoherenceMetricName);
        coherence.Interpretation!.Failed.Should().BeFalse(because: coherence.Interpretation.Reason);
        coherence.Interpretation.Rating.Should().BeOneOf(expectedRatings, because: coherence.Reason);
        coherence.ContainsDiagnostics(d => d.Severity >= EvaluationDiagnosticSeverity.Warning).Should().BeFalse();
        coherence.Value.Should().BeGreaterThanOrEqualTo(4, because: coherence.Reason);

        /// Retrieve the score for relevance from the <see cref="EvaluationResult"/>.
        NumericMetric relevance = result.Get<NumericMetric>(RelevanceEvaluator.RelevanceMetricName);
        relevance.Interpretation!.Failed.Should().BeFalse(because: relevance.Interpretation.Reason);
        relevance.Interpretation.Rating.Should().BeOneOf(expectedRatings, because: relevance.Reason);
        relevance.ContainsDiagnostics(d => d.Severity >= EvaluationDiagnosticSeverity.Warning).Should().BeFalse();
        relevance.Value.Should().BeGreaterThanOrEqualTo(4, because: relevance.Reason);
    }

}
