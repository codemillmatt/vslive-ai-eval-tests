# 🧪 VS Live AI Evaluation Testing Playground

**Welcome to the most fun you'll have testing AI models this side of the singularity!** 🤖✨

This repository is your comprehensive demo playground for the **Microsoft.Extensions.AI.Evaluation** framework - because even AI needs a report card, and yours is about to be _exceptional_! 

## 🎯 What's This All About?

This repo demonstrates the Microsoft.Extensions.AI.Evaluation framework in action, showing you how to:

- 🔍 **Evaluate AI responses like a pro** - Test for coherence, relevance, fluency, and much more
- 📊 **Get detailed metrics** - Because numbers don't lie (unlike that one AI that said pineapple belongs on pizza)
- 🧪 **Unit test your AI** - Integrate seamlessly with MSTest, xUnit, or your favorite testing framework
- 🚀 **Live evaluation** - Real-time assessment of AI responses in production apps
- 📈 **Generate reports** - Beautiful, actionable insights that make stakeholders happy

## 🏗️ Repository Structure

```
📁 Tests/
├── 📁 SimpleEvaluations/          # Unit test examples with MSTest
│   ├── EvaluateResponse.cs        # Basic evaluation examples
│   └── EvaluateResponseReport.cs  # Advanced reporting scenarios
└── 📁 GenAiLab.Web/              # Blazor web app with live evaluation
    └── Components/Pages/Chat/     # Real-time chat evaluation
```

## 🎪 Featured Evaluators

This demo showcases several evaluators from the Microsoft.Extensions.AI.Evaluation toolkit:

### ✅ **Quality Evaluators** (The Stars of the Show)
- **🎯 RelevanceEvaluator** - "Is this response actually answering my question?"
- **🧠 CoherenceEvaluator** - "Does this make logical sense, or did the AI have a stroke?"
- **💫 FluencyEvaluator** - "Grammar matters, even for robots!"

### 🔐 **Safety Evaluators** (The Guardians)
- **🛡️ ContentHarmEvaluator** - Keeps things family-friendly
- **🏛️ ProtectedMaterialEvaluator** - Respects copyright and privacy
- **⚡ GroundednessProEvaluator** - "Show me the receipts!"

### 📝 **NLP Evaluators** (The Classic Critics)
- **📊 BLEUEvaluator** - Traditional text similarity metrics
- **🎯 F1Evaluator** - Precision and recall analysis
- **✨ GLEUEvaluator** - Google's take on BLEU

_...and many more! See the [full evaluator lineup](https://learn.microsoft.com/en-us/dotnet/ai/conceptual/evaluation-libraries#comprehensive-evaluation-metrics) in the official docs._

## 🚀 Quick Start

### Prerequisites

- **.NET 9.0 SDK** - Because we're living in the future! 🚀
- **Azure OpenAI or OpenAI API access** - For the AI models to evaluate
- **Visual Studio 2022** or **Visual Studio Code** - Your code editor of choice

### 🏃‍♂️ Running the Unit Tests

1. **Clone this repo** (you've probably done this already, smarty!)
2. **Set up your secrets** for Azure OpenAI:
   ```bash
   cd Tests/SimpleEvaluations
   dotnet user-secrets set "FOUNDRY-API-KEY" "your-api-key-here"
   dotnet user-secrets set "FOUNDRY-URL-ENDPOINT" "your-endpoint-here"
   dotnet user-secrets set "DEPLOYED-MODEL-NAME" "gpt-4o"
   ```
3. **Run the tests**:
   ```bash
   dotnet test
   ```
4. **Watch the magic happen!** ✨

### 🌐 Running the Web Application

1. **Navigate to the web project**:
   ```bash
   cd Tests/GenAiLab.Web
   ```
2. **Configure your AI service** (same secrets as above)
3. **Run the app**:
   ```bash
   dotnet run
   ```
4. **Open your browser** and start chatting with AI while getting real-time evaluation scores!

## 🎭 What You'll See in Action

### Unit Test Evaluation Example
```csharp
// Create evaluators
IEvaluator coherenceEvaluator = new CoherenceEvaluator();
IEvaluator relevanceEvaluator = new RelevanceEvaluator();
IEvaluator compositeEvaluator = new CompositeEvaluator(coherenceEvaluator, relevanceEvaluator);

// Evaluate the response
EvaluationResult result = await compositeEvaluator.EvaluateAsync(messages, response, chatConfiguration);

// Get the scores
NumericMetric coherence = result.Get<NumericMetric>(CoherenceEvaluator.CoherenceMetricName);
NumericMetric relevance = result.Get<NumericMetric>(RelevanceEvaluator.RelevanceMetricName);
```

### Live Web App Evaluation
The Blazor web app demonstrates real-time evaluation where every chat response gets automatically evaluated for:
- Coherence (logical flow)
- Relevance (answers the question)
- Fluency (grammar and readability)

Results are stored and can generate comprehensive reports! 📊

## 🎨 Features Showcased

- **🔄 Composite Evaluators** - Combine multiple evaluators for comprehensive assessment
- **📈 Reporting & Caching** - Store results and generate beautiful reports
- **⚡ Real-time Evaluation** - Evaluate responses as they're generated
- **🧪 Test Integration** - Seamless integration with MSTest (works with xUnit and NUnit too!)
- **📦 Flexible Architecture** - Mix and match evaluators based on your needs

## 🌟 Why This Matters

In the wild west of AI development, evaluation is your sheriff's badge! This framework helps you:

- **Build trust** in your AI applications
- **Catch issues early** before your users do
- **Measure improvements** objectively
- **Ensure safety** and appropriateness
- **Generate reports** that make your boss think you're a wizard 🧙‍♂️

## 📚 Learn More

Want to dive deeper? Check out these resources:

- [📖 Microsoft.Extensions.AI.Evaluation Documentation](https://learn.microsoft.com/en-us/dotnet/ai/conceptual/evaluation-libraries)
- [🎓 Quickstart Tutorial](https://learn.microsoft.com/en-us/dotnet/ai/quickstarts/evaluate-ai-response)
- [📊 Advanced Reporting Tutorial](https://learn.microsoft.com/en-us/dotnet/ai/tutorials/evaluate-with-reporting)
- [🔐 Safety Evaluation Guide](https://learn.microsoft.com/en-us/dotnet/ai/tutorials/evaluate-safety)

## 🤝 Contributing

Found a bug? Want to add more examples? PRs welcome! Let's make AI evaluation even more awesome together! 🚀

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

*Remember: A well-evaluated AI is a happy AI, and a happy AI makes for happy users! Now go forth and evaluate responsibly!* 🎉🤖
